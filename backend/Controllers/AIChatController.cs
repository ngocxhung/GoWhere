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

                var history = await _aiChatService.GetHistoryByUserIdAsync(int.Parse(userId));
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

                var history = await _aiChatService.GetHistoryByIdAsync(int.Parse(userId), id);
                
                if (history == null)
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

                var deleted = await _aiChatService.DeleteHistoryAsync(int.Parse(userId), id);
                
                if (!deleted)
                {
                    return NotFound(new { Message = "Không tìm thấy lịch trình" });
                }

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

                var result = await _aiChatService.CreateAIPaymentOrderAsync(int.Parse(userId));
                return Ok(result);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { Message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = "An error occurred", Details = ex.Message });
            }
        }

    }
}
