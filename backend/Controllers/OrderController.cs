using GoWhere_Api.Model;
using GoWhere_Api.Model.DTOs.Request;
using GoWhere_Api.Repo.IRepo;
using GoWhere_Api.Service.IService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace GoWhere_Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class OrderController : ControllerBase
    {
        private readonly IOrderRepo _orderRepo;
        private readonly IUserRepo _userRepo;
        private readonly IConfiguration _config;
        private readonly IFileService _fileService;

        public OrderController(IOrderRepo orderRepo, IUserRepo userRepo, IConfiguration config, IFileService fileService)
        {
            _orderRepo = orderRepo;
            _userRepo = userRepo;
            _config = config;
            _fileService = fileService;
        }

        /// <summary>
        /// [USER] Upload ảnh chứng từ chuyển khoản cho đơn hàng
        /// </summary>
        [HttpPost("UploadReceipt/{orderId}")]
        public async Task<IActionResult> UploadReceipt(int orderId, IFormFile file)
        {
            try
            {
                var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(userId))
                    return Unauthorized("User not found.");

                var order = await _orderRepo.GetByIdAsync(orderId);
                if (order == null)
                    return NotFound("Đơn hàng không tồn tại.");

                if (order.UserId != int.Parse(userId))
                    return Forbid("Bạn không có quyền upload ảnh cho đơn hàng này.");

                var fileUrl = await _fileService.UploadReceiptAsync(file, orderId);
                return Ok(new { Message = "Upload thành công", FileUrl = fileUrl });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { Message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = "Lỗi khi tải ảnh", Details = ex.Message });
            }
        }

        [AllowAnonymous]
        [HttpGet("GetPaymentInfo")]
        public IActionResult GetPaymentInfo()
        {
            var vietQr = _config.GetSection("VietQR");
            return Ok(new
            {
                BankBin = vietQr["BankBin"],
                AccountNo = vietQr["AccountNo"],
                AccountName = vietQr["AccountName"],
                Template = vietQr["Template"]
            });
        }

        [HttpPost("AddToCart")]
        public async Task<IActionResult> AddToCart([FromBody] AddToCartDto addToCartDto)
        {
            try
            {
                var username = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Name)?.Value;
                if (string.IsNullOrEmpty(username))
                {
                    return Unauthorized("User not found.");
                }

                var user = await _userRepo.GetUserByUsernameAsync(username);
                if (user == null)
                {
                    return Unauthorized("User not found.");
                }

                // Get current pending order (Cart)
                var order = await _orderRepo.GetByUserAndStatusAsync(user.UserId, "Pending");

                if (order == null)
                {
                    // Create new order
                    order = new Order
                    {
                        UserId = user.UserId,
                        OrderDate = DateTime.Now,
                        Status = "Pending",
                        TotalPrice = 0,
                        OrderItems = new List<OrderItem>()
                    };
                    await _orderRepo.CreateAsync(order);
                }

                var existingItem = order.OrderItems.FirstOrDefault(i => i.ItemName == addToCartDto.ItemName);
                if (existingItem != null)
                {
                    existingItem.Quantity += addToCartDto.Quantity;
                }
                else
                {
                    order.OrderItems.Add(new OrderItem
                    {
                        ItemName = addToCartDto.ItemName,
                        Price = addToCartDto.Price,
                        Quantity = addToCartDto.Quantity,
                        OrderId = order.OrderId,
                        ScenicSpotId = addToCartDto.ScenicSpotId,
                        ServiceType = addToCartDto.ServiceType,
                        ServiceId = addToCartDto.ServiceId,
                        ParentItemId = addToCartDto.ParentItemId
                    });
                }

                // Recalculate total price
                order.TotalPrice = order.OrderItems.Sum(i => i.Price * i.Quantity);
                
                await _orderRepo.UpdateAsync(order);

                return Ok(new { Message = "Item added to cart successfully", OrderId = order.OrderId, TotalPrice = order.TotalPrice });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = "Error adding to cart", Details = ex.Message });
            }
        }

        [HttpGet("GetMyOrders")]
        public async Task<IActionResult> GetMyOrders()
        {
            try
            {
                var username = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Name)?.Value;
                if (string.IsNullOrEmpty(username))
                {
                    return Unauthorized("User not found.");
                }

                var user = await _userRepo.GetUserByUsernameAsync(username);
                if (user == null)
                {
                    return Unauthorized("User not found.");
                }

                var orders = await _orderRepo.GetByUserIdAsync(user.UserId);
                
                return Ok(orders);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = "Error retrieving orders", Details = ex.Message });
            }
        }

        [HttpDelete("RemoveFromCart/{orderItemId}")]
        public async Task<IActionResult> RemoveFromCart(int orderItemId)
        {
            try
            {
               var username = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Name)?.Value;
               var user = await _userRepo.GetUserByUsernameAsync(username);

               var order = await _orderRepo.GetByUserAndStatusAsync(user.UserId, "Pending");
               if (order == null) return NotFound("Cart not found");

               var item = order.OrderItems.FirstOrDefault(i => i.OrderItemId == orderItemId);
               if (item == null) return NotFound("Item not found in cart");

               order.OrderItems.Remove(item);
               
               // Recalculate
               order.TotalPrice = order.OrderItems.Sum(i => i.Price * i.Quantity);
               await _orderRepo.UpdateAsync(order);

               return Ok(new { Message = "Item removed", TotalPrice = order.TotalPrice });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = "Error removing item", Details = ex.Message });
            }
        }

        [HttpPost("Checkout")]
        public async Task<IActionResult> Checkout([FromBody] List<int> orderItemIds)
        {
            try
            {
                var username = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Name)?.Value;
                var user = await _userRepo.GetUserByUsernameAsync(username);
                
                var pendingOrder = await _orderRepo.GetByUserAndStatusAsync(user.UserId, "Pending");
                if (pendingOrder == null) return BadRequest("No pending order to checkout");

                var itemsToCheckout = pendingOrder.OrderItems.Where(i => orderItemIds.Contains(i.OrderItemId)).ToList();
                
                if (!itemsToCheckout.Any()) return BadRequest("No items selected");

                // Create new WaitPayment Order
                var newOrder = new Order
                {
                    UserId = user.UserId,
                    OrderDate = DateTime.Now,
                    Status = "WaitPayment",
                    OrderItems = new List<OrderItem>(),
                    TotalPrice = 0
                };
                
                // Move items (In EF, re-assigning parent changes the FK) - *Wait, deeply nested structure issues*
                // Simpler: Set OrderId of items to new ID? No, we need to create the order first.
                // We will Remove from old list and Add to new list.
                
                foreach (var item in itemsToCheckout)
                {
                   // pendingOrder.OrderItems.Remove(item); // Don't remove yet, concurrent mod likely
                   // Actually, EF tracking handles this if we just reassign.
                   // But let's follow the standard Create then Move.
                }

                // Since we need to save the new order first to get ID? Not necessarily with nav props
                // Let's rely on EF relationship fixup.
                // We need to clone them or move them? 
                // "Move" means changing the OrderId. 
                
                // Strategy: 
                // 1. Create Payed Order.
                // 2. Assign items to it.
                // 3. Update Old Order total.

                await _orderRepo.CreateAsync(newOrder); // Get ID

                foreach (var item in itemsToCheckout)
                {
                    item.OrderId = newOrder.OrderId;
                }

                newOrder.TotalPrice = itemsToCheckout.Sum(i => i.Price * i.Quantity);
                await _orderRepo.UpdateAsync(newOrder);

                // Update old order
                // items are moved, so they shouldn't be in old order (by FK)
                // But pendingOrder.OrderItems might still have them in memory?
                // Safe bet: Fetch pending order again or filter.
                
                // Actually, let's just create new items for history to keep it clean? 
                // No, moving is better for 'Cart' -> 'Order'.
                
                // We need to re-calc pending total.
                // Since we changed FKs, pendingOrder *in DB* has fewer items. 
                // But we need to update TotalPrice.
                
                var remainingItems = pendingOrder.OrderItems.Where(i => !orderItemIds.Contains(i.OrderItemId)).ToList();
                pendingOrder.TotalPrice = remainingItems.Sum(i => i.Price * i.Quantity);
                await _orderRepo.UpdateAsync(pendingOrder);

                return Ok(new { Message = "Checkout successful", NewOrderId = newOrder.OrderId });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = "Error checking out", Details = ex.Message });
            }
        }
    }
}
