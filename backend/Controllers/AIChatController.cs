using GoWhere_Api.Model.DTOs.Request;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace GoWhere_Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AIChatController : ControllerBase
    {
        private readonly Service.IService.IAIChatService _aiChatService;

        public AIChatController(Service.IService.IAIChatService aiChatService)
        {
            _aiChatService = aiChatService;
        }

        /// <summary>
        /// Kiểm tra số lần sử dụng AI còn lại
        /// </summary>
        [HttpGet("CheckUsageLimit")]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> CheckUsageLimit()
        {
            try
            {
                var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(userId))
                {
                    return Unauthorized("User ID not found in token.");
                }

                var result = await _aiChatService.CheckUsageLimitAsync(int.Parse(userId));
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = "An error occurred", Details = ex.Message });
            }
        }

        /// <summary>
        /// Tạo lịch trình du lịch bằng AI
        /// </summary>
        [HttpPost("GenerateSchedule")]
        // [Authorize]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status402PaymentRequired)]
        public async Task<IActionResult> GenerateSchedule([FromBody] AIScheduleRequestDto request)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(userId))
                {
                    userId = "1"; // Default for debug
                }

                var result = await _aiChatService.GenerateScheduleAsync(int.Parse(userId), request);
                return Ok(result);
            }
            catch (InvalidOperationException ex) when (ex.Message.Contains("thanh toán"))
            {
                // Payment required
                return StatusCode(402, new 
                { 
                    Message = ex.Message,
                    RequiresPayment = true,
                    PaymentAmount = 50000
                });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { Message = ex.Message });
            }
            catch (HttpRequestException ex)
            {
                return StatusCode(503, new { Message = "AI service temporarily unavailable", Details = ex.Message });
            }
            catch (Exception ex)
            {
                Console.WriteLine("CRITICAL CONTROLLER ERROR: " + ex.ToString());
                return StatusCode(500, new { Message = "An error occurred while generating schedule", Details = ex.Message, Stack = ex.StackTrace });
            }
        }

        /// <summary>
        /// Chat với AI trợ lý du lịch
        /// </summary>
        [HttpPost("Chat")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Chat([FromBody] AIChatRequestDto request)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var result = await _aiChatService.ChatWithAIAsync(request);
                return Ok(result);
            }
            catch (HttpRequestException ex)
            {
                return StatusCode(503, new { Message = "AI service temporarily unavailable", Details = ex.Message });
            }
            catch (Exception ex)
            {
                Console.WriteLine("CRITICAL CHAT ERROR: " + ex.ToString());
                return StatusCode(500, new { Message = "An error occurred while chatting with AI", Details = ex.Message });
            }
        }

        /// <summary>
        /// Lấy lịch sử lịch trình AI của user
        /// </summary>
        [HttpGet("History")]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> GetHistory()
        {
            try
            {
                var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(userId))
                {
                    return Unauthorized("User ID not found in token.");
                }

                var historyRepo = HttpContext.RequestServices.GetRequiredService<Repo.IRepo.IAIScheduleHistoryRepo>();
                var history = await historyRepo.GetByUserIdAsync(int.Parse(userId));
                
                return Ok(history);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = "An error occurred", Details = ex.Message });
            }
        }

        /// <summary>
        /// Lấy chi tiết 1 lịch trình từ lịch sử
        /// </summary>
        [HttpGet("History/{id}")]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetHistoryById(int id)
        {
            try
            {
                var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(userId))
                {
                    return Unauthorized("User ID not found in token.");
                }

                var historyRepo = HttpContext.RequestServices.GetRequiredService<Repo.IRepo.IAIScheduleHistoryRepo>();
                var history = await historyRepo.GetByIdAsync(id);
                
                if (history == null || history.UserId != int.Parse(userId))
                {
                    return NotFound(new { Message = "Không tìm thấy lịch trình" });
                }

                return Ok(history);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = "An error occurred", Details = ex.Message });
            }
        }

        /// <summary>
        /// Xóa lịch trình khỏi lịch sử
        /// </summary>
        [HttpDelete("History/{id}")]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteHistory(int id)
        {
            try
            {
                var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(userId))
                {
                    return Unauthorized("User ID not found in token.");
                }

                var historyRepo = HttpContext.RequestServices.GetRequiredService<Repo.IRepo.IAIScheduleHistoryRepo>();
                var history = await historyRepo.GetByIdAsync(id);
                
                if (history == null || history.UserId != int.Parse(userId))
                {
                    return NotFound(new { Message = "Không tìm thấy lịch trình" });
                }

                await historyRepo.DeleteAsync(id);
                return Ok(new { Message = "Đã xóa lịch trình" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = "An error occurred", Details = ex.Message });
            }
        }

        /// <summary>
        /// Tạo order thanh toán cho AI unlimited
        /// </summary>
        [HttpPost("CreateAIPaymentOrder")]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> CreateAIPaymentOrder()
        {
            try
            {
                var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(userId))
                {
                    return Unauthorized("User ID not found in token.");
                }

                var aiSuggestionRepo = HttpContext.RequestServices.GetRequiredService<Repo.IRepo.IAISuggestionRequestRepo>();
                var orderRepo = HttpContext.RequestServices.GetRequiredService<Repo.IRepo.IOrderRepo>();

                var userIdInt = int.Parse(userId);
                var aiRequest = await aiSuggestionRepo.GetByUserIdAsync(userIdInt);

                // Check if already paid
                if (aiRequest?.Order != null && aiRequest.Order.Status == "Payed")
                {
                    return BadRequest(new { Message = "Bạn đã thanh toán rồi!" });
                }

                // Check if already has pending payment order
                if (aiRequest?.Order != null && aiRequest.Order.Status == "WaitPayment")
                {
                    return Ok(new 
                    { 
                        Message = "Đơn hàng đã tồn tại", 
                        OrderId = aiRequest.Order.OrderId,
                        Amount = 50000
                    });
                }

                // Create new order for AI payment
                var order = new Model.Order
                {
                    UserId = userIdInt,
                    OrderDate = DateTime.Now,
                    Status = "WaitPayment",
                    TotalPrice = 50000,
                    OrderItems = new List<Model.OrderItem>
                    {
                        new Model.OrderItem
                        {
                            ItemName = "AI Unlimited - Tạo lịch trình không giới hạn",
                            Price = 50000,
                            Quantity = 1,
                            ServiceType = "AI"
                        }
                    }
                };

                await orderRepo.CreateAsync(order);

                // Link order to AI request
                if (aiRequest == null)
                {
                    aiRequest = new Model.AISuggestionRequest
                    {
                        UserId = userIdInt,
                        Count = 0,
                        Price = 50000,
                        OrderId = order.OrderId
                    };
                    await aiSuggestionRepo.CreateAsync(aiRequest);
                }
                else
                {
                    aiRequest.OrderId = order.OrderId;
                    aiRequest.Price = 50000;
                    await aiSuggestionRepo.UpdateAsync(aiRequest);
                }

                return Ok(new 
                { 
                    Message = "Tạo đơn hàng thành công", 
                    OrderId = order.OrderId,
                    Amount = 50000
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = "An error occurred", Details = ex.Message });
            }
        }
    }
}
