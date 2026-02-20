using GoWhere_Api.Model.DTOs.Request;
using GoWhere_Api.Service.IService;
using GoWhere_Api.Service.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace GoWhere_Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PaymentController : ControllerBase
    {
        private readonly IPaymentService _paymentService;
        private readonly ILogger<PaymentController> _logger;
        private readonly IEmailService _emailService;

        public PaymentController(IPaymentService paymentService, ILogger<PaymentController> logger, IEmailService emailService)
        {
            _logger = logger;
            _paymentService = paymentService;
            _emailService = emailService;
        }

        /// <summary>
        /// Tạo đơn hàng thanh toán cho dịch vụ AI Chat
        /// </summary>
        [HttpPost("CreateAIChatPayment")]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> CreateAIChatPayment()
        {
            try
            {
                var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(userId))
                {
                    return Unauthorized("User ID not found in token.");
                }

                var result = await _paymentService.CreateAIChatPaymentAsync(int.Parse(userId));
                return Ok(new
                {
                    Success = true,
                    Message = "Đơn hàng đã được tạo. Vui lòng quét mã QR để thanh toán.",
                    Data = result
                });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { Success = false, Message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Success = false, Message = "Đã xảy ra lỗi", Details = ex.Message });
            }
        }

        /// <summary>
        /// Xác nhận thanh toán (sử dụng sau khi khách hàng đã chuyển khoản)
        /// </summary>
        [HttpPost("ConfirmPayment/{orderId}")]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> ConfirmPayment(int orderId)
        {
            try
            {
                var result = await _paymentService.ConfirmPaymentAsync(orderId);
                return Ok(new
                {
                    Success = true,
                    Message = "Thanh toán đã được xác nhận thành công.",
                    Data = result
                });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { Success = false, Message = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { Success = false, Message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Success = false, Message = "Đã xảy ra lỗi", Details = ex.Message });
            }
        }

        /// <summary>
        /// Lấy lịch sử thanh toán của người dùng
        /// </summary>
        [HttpGet("PaymentHistory")]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> GetPaymentHistory()
        {
            try
            {
                var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(userId))
                {
                    return Unauthorized("User ID not found in token.");
                }

                var result = await _paymentService.GetUserPaymentHistoryAsync(int.Parse(userId));
                return Ok(new
                {
                    Success = true,
                    Data = result
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Success = false, Message = "Đã xảy ra lỗi", Details = ex.Message });
            }
        }
        /// <summary>
        /// Test gửi email hướng dẫn thanh toán (chỉ dùng cho mục đích test)
        /// </summary>
        /// <summary>
        /// [TEST] Test gửi email
        /// </summary>
        [HttpPost("TestEmail")]
        [Obsolete("Chỉ dùng cho mục đích test, không sử dụng trong môi trường production")]
        [Authorize]
        public async Task<IActionResult> TestEmail([FromQuery] string toEmail)
        {
            try
            {
                _logger.LogInformation($"Testing email to: {toEmail}");
                //Find username 
                var username = User.FindFirst(ClaimTypes.Name)?.Value;



                await _emailService.SendPaymentCreatedEmailAsync(
                    toEmail,
                    username,
                    99999,
                    50000,
                    "https://img.vietqr.io/image/970422-083001022003-compact2.png?amount=50000&addInfo=TEST&accountName=Test",
                    "TEST99999"
                );

                return Ok(new
                {
                    Success = true,
                    Message = $"✅ Email sent to {toEmail}"
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Test email failed");
                return StatusCode(500, new
                {
                    Success = false,
                    Error = ex.Message,
                    Inner = ex.InnerException?.Message
                });
            }
        }
    }
}