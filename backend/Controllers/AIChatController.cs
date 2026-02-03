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
    }
}
