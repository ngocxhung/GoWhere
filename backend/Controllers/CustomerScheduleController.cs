using GoWhere_Api.Model.DTOs.Request;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Query;
using System.Security.Claims;

namespace GoWhere_Api.Controllers
{
    [Route("api/[controller]")]

    [ApiController]
    public class CustomerScheduleController : ControllerBase
    {
        private readonly Service.IService.ICustomerScheduleService _customerScheduleService;

        public CustomerScheduleController(Service.IService.ICustomerScheduleService customerScheduleService)
        {
            _customerScheduleService = customerScheduleService;
        }
        [HttpGet("GetCustomerSchedulesByCustomerId")]
        [Authorize]
        public async Task<IActionResult> GetCustomerSchedulesByCustomerId()
        {
            try
            {
                var customerId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(customerId))
                {
                    return Unauthorized("Customer ID not found in token.");
                }
                var schedules = await _customerScheduleService.GetCustomerSchedulesByCustomerIdAsync(int.Parse(customerId));
                return Ok(schedules);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
        [HttpGet("GetAllCustomerSchedules")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetAllCustomerSchedule()
        {
            try
            {
                var schedules = await _customerScheduleService.GetCustomerSchedulesAsync();
                if (schedules == null || !schedules.Any())
                {
                    return NotFound("No customer schedules found.");
                }
                return Ok(schedules);

            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
        [HttpPost("AddNewCustomerSchedule")]
        [Authorize]
        public async Task<IActionResult> AddNewCustomerSchedule([FromBody] CustomerScheduleRequestDto request)
        {
           try {
                var customerId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(customerId))
                {
                    return Unauthorized("Customer ID not found in token.");
                }
                if(!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }
                await _customerScheduleService.AddNewCustomerScheduleAsync(int.Parse(customerId), request);
                return CreatedAtAction(null, new { customerId }, "Customer schedule created successfully.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
        [HttpPatch("UpdateCustomerScheduleStatus/{scheduleId}")]
        [Authorize]
        public async Task<IActionResult> UpdateCustomerScheduleStatus(int scheduleId, [FromBody] string status)
        {
            try
            {
                var customerId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(customerId))
                {
                    return Unauthorized("Customer ID not found in token.");
                }
                await _customerScheduleService.UpdateCustomerScheduleStatusAsync(int.Parse(customerId), scheduleId, status);
                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }

        }
        [HttpPost("AddFeedback")]
        [Authorize]
        public async Task<IActionResult> AddFeedback([FromBody] FeedbackRequestDto request)
        {
            try
            {
                var customerId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(customerId))
                {
                    return Unauthorized("Customer ID not found in token.");
                }
                if(!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }
                await _customerScheduleService.AddFeedbackAsync(int.Parse(customerId), request);
                return CreatedAtAction(null, new { customerId }, "Feedback added successfully.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }

        }
    }
}