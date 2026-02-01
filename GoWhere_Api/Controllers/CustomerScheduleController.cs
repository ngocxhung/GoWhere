using GoWhere_Api.Model.DTOs.Request;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
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
            var customerId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(customerId))
            {
                return Unauthorized("Customer ID not found in token.");
            }
            var schedules = await _customerScheduleService.GetCustomerSchedulesByCustomerIdAsync(int.Parse(customerId));
            return Ok(schedules);


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
            var customerId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(customerId))
            {
                return Unauthorized("Customer ID not found in token.");
            }
            await _customerScheduleService.AddNewCustomerScheduleAsync(customerId, request);
            return CreatedAtAction(null, new { customerId }, "New customer schedule added successfully.");
        }
        [HttpPatch("UpdateCustomerScheduleStatus/{scheduleId}")]
        [Authorize]
        public async Task<IActionResult> UpdateCustomerScheduleStatus(int scheduleId, [FromQuery] string status)
        {
            var customerId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(customerId))
            {
                return Unauthorized("Customer ID not found in token.");
            }
            await _customerScheduleService.UpdateCustomerScheduleStatusAsync(int.Parse(customerId), scheduleId, status);
            return NoContent();
        }
    }
}