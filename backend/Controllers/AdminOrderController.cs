using GoWhere_Api.Repo.IRepo;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GoWhere_Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Admin")]
    public class AdminOrderController : ControllerBase
    {
        private readonly IOrderRepo _orderRepo;
        private readonly IUserRepo _userRepo;

        public AdminOrderController(IOrderRepo orderRepo, IUserRepo userRepo)
        {
            _orderRepo = orderRepo;
            _userRepo = userRepo;
        }

        /// <summary>
        /// [ADMIN] Lấy tất cả đơn hàng
        /// </summary>
        [HttpGet("GetAllOrders")]
        public async Task<IActionResult> GetAllOrders()
        {
            try
            {
                var orders = await _orderRepo.GetAllAsync();
                
                var ordersDto = orders.Select(o => new
                {
                    o.OrderId,
                    o.UserId,
                    UserName = o.User?.Username,
                    UserEmail = o.User?.Email,
                    o.TotalPrice,
                    o.Status,
                    o.OrderDate
                }).OrderByDescending(o => o.OrderDate);
                
                return Ok(ordersDto);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = "Error retrieving orders", Details = ex.Message });
            }
        }

        /// <summary>
        /// [ADMIN] Lấy đơn hàng theo ID
        /// </summary>
        [HttpGet("GetOrderById/{id}")]
        public async Task<IActionResult> GetOrderById(int id)
        {
            try
            {
                var order = await _orderRepo.GetByIdAsync(id);
                
                if (order == null)
                {
                    return NotFound($"Order with ID {id} not found.");
                }

                var orderDto = new
                {
                    order.OrderId,
                    order.UserId,
                    UserName = order.User?.Username,
                    UserEmail = order.User?.Email,
                    UserPhone = order.User?.PhoneNumber,
                    order.TotalPrice,
                    order.Status,
                    order.OrderDate,
                    Items = order.OrderItems?.Select(item => new
                    {
                        item.OrderItemId,
                        item.ItemName,
                        item.Price,
                        item.Quantity
                    })
                };
                
                return Ok(orderDto);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = "Error retrieving order", Details = ex.Message });
            }
        }

        /// <summary>
        /// [ADMIN] Lấy đơn hàng theo User ID
        /// </summary>
        [HttpGet("GetOrdersByUserId/{userId}")]
        public async Task<IActionResult> GetOrdersByUserId(int userId)
        {
            try
            {
                var orders = await _orderRepo.GetByUserIdAsync(userId);
                
                if (!orders.Any())
                {
                    return NotFound($"No orders found for user ID {userId}.");
                }

                var ordersDto = orders.Select(o => new
                {
                    o.OrderId,
                    o.TotalPrice,
                    o.Status,
                    o.OrderDate
                }).OrderByDescending(o => o.OrderDate);
                
                return Ok(ordersDto);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = "Error retrieving user orders", Details = ex.Message });
            }
        }

        /// <summary>
        /// [ADMIN] Lấy đơn hàng theo trạng thái
        /// </summary>
        [HttpGet("GetOrdersByStatus/{status}")]
        public async Task<IActionResult> GetOrdersByStatus(string status)
        {
            try
            {
                var allOrders = await _orderRepo.GetAllAsync();
                var orders = allOrders.Where(o => o.Status.Equals(status, StringComparison.OrdinalIgnoreCase));
                
                if (!orders.Any())
                {
                    return NotFound($"No orders found with status: {status}");
                }

                var ordersDto = orders.Select(o => new
                {
                    o.OrderId,
                    o.UserId,
                    UserName = o.User?.Username,
                    o.TotalPrice,
                    o.Status,
                    o.OrderDate
                }).OrderByDescending(o => o.OrderDate);
                
                return Ok(ordersDto);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = "Error retrieving orders", Details = ex.Message });
            }
        }

        /// <summary>
        /// [ADMIN] Thống kê đơn hàng
        /// </summary>
        [HttpGet("GetOrderStatistics")]
        public async Task<IActionResult> GetOrderStatistics()
        {
            try
            {
                var orders = await _orderRepo.GetAllAsync();
                
                var stats = new
                {
                    TotalOrders = orders.Count(),
                    PendingOrders = orders.Count(o => o.Status == "Pending"),
                    PayedOrders = orders.Count(o => o.Status == "Payed"),
                    TotalRevenue = orders.Where(o => o.Status == "Payed").Sum(o => o.TotalPrice),
                    AverageOrderValue = orders.Any() ? orders.Average(o => o.TotalPrice) : 0,
                    OrdersByDate = orders.GroupBy(o => o.OrderDate.Date)
                        .Select(g => new
                        {
                            Date = g.Key,
                            Count = g.Count(),
                            TotalAmount = g.Sum(o => o.TotalPrice)
                        })
                        .OrderByDescending(g => g.Date)
                        .Take(30)
                };
                
                return Ok(stats);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = "Error calculating statistics", Details = ex.Message });
            }
        }

        /// <summary>
        /// [ADMIN] Cập nhật trạng thái đơn hàng
        /// </summary>
        [HttpPatch("UpdateOrderStatus/{id}")]
        public async Task<IActionResult> UpdateOrderStatus(int id, [FromBody] string newStatus)
        {
            try
            {
                var order = await _orderRepo.GetByIdAsync(id);
                
                if (order == null)
                {
                    return NotFound($"Order with ID {id} not found.");
                }

                // Không cho phép thay đổi status nếu đã Payed
                if (order.Status == "Payed")
                {
                    return BadRequest("Cannot update status of a paid order.");
                }

                // Validate status
                var validStatuses = new[] { "Pending", "WaitPayment", "Payed", "Cancelled" };
                if (!validStatuses.Contains(newStatus))
                {
                    return BadRequest($"Invalid status. Valid values: {string.Join(", ", validStatuses)}");
                }

                order.Status = newStatus;
                await _orderRepo.UpdateAsync(order);
                
                return Ok(new { Message = "Order status updated successfully.", NewStatus = newStatus });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = "Error updating order status", Details = ex.Message });
            }
        }

        /// <summary>
        /// [ADMIN] Hủy đơn hàng
        /// </summary>
        [HttpPatch("CancelOrder/{id}")]
        public async Task<IActionResult> CancelOrder(int id)
        {
            try
            {
                var order = await _orderRepo.GetByIdAsync(id);
                
                if (order == null)
                {
                    return NotFound($"Order with ID {id} not found.");
                }

                if (order.Status == "Payed")
                {
                    return BadRequest("Cannot cancel an order that has been paid.");
                }

                order.Status = "Cancelled";
                await _orderRepo.UpdateAsync(order);
                
                return Ok(new { Message = "Order cancelled successfully." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = "Error cancelling order", Details = ex.Message });
            }
        }

        /// <summary>
        /// [ADMIN] Xóa đơn hàng
        /// </summary>
        [HttpDelete("DeleteOrder/{id}")]
        public async Task<IActionResult> DeleteOrder(int id)
        {
            try
            {
                var order = await _orderRepo.GetByIdAsync(id);
                
                if (order == null)
                {
                    return NotFound($"Order with ID {id} not found.");
                }

                await _orderRepo.DeleteAsync(id);
                
                return Ok(new { Message = "Order deleted successfully." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = "Error deleting order", Details = ex.Message });
            }
        }
    }
}
