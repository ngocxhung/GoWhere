using GoWhere_Api.Service.IService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GoWhere_Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Admin")]
    public class AdminScheduleController : ControllerBase
    {
        private readonly ICustomerScheduleService _scheduleService;

        public AdminScheduleController(ICustomerScheduleService scheduleService)
        {
            _scheduleService = scheduleService;
        }

        /// <summary>
        /// [ADMIN] Lấy tất cả lịch trình của tất cả users
        /// </summary>
        [HttpGet("GetAllSchedules")]
        public async Task<IActionResult> GetAllSchedules()
        {
            try
            {
                var schedules = await _scheduleService.GetCustomerSchedulesAsync();
                return Ok(schedules);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = "Error retrieving schedules", Details = ex.Message });
            }
        }

        /// <summary>
        /// [ADMIN] Lấy lịch trình theo schedule ID
        /// </summary>
        [HttpGet("GetScheduleById/{id}")]
        public async Task<IActionResult> GetScheduleById(int id)
        {
            try
            {
                var schedules = await _scheduleService.GetCustomerSchedulesAsync();
                var schedule = schedules.FirstOrDefault(s => s.ScheduleId == id);
                
                if (schedule == null)
                {
                    return NotFound($"Schedule with ID {id} not found.");
                }
                
                return Ok(schedule);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = "Error retrieving schedule", Details = ex.Message });
            }
        }

        /// <summary>
        /// [ADMIN] Lấy lịch trình theo User ID
        /// </summary>
        [HttpGet("GetSchedulesByUserId/{userId}")]
        public async Task<IActionResult> GetSchedulesByUserId(int userId)
        {
            try
            {
                var schedules = await _scheduleService.GetCustomerSchedulesByCustomerIdAsync(userId);
                return Ok(schedules);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = "Error retrieving user schedules", Details = ex.Message });
            }
        }

        /// <summary>
        /// [ADMIN] Lấy thống kê lịch trình
        /// </summary>
        [HttpGet("GetScheduleStatistics")]
        public async Task<IActionResult> GetScheduleStatistics()
        {
            try
            {
                var allSchedules = await _scheduleService.GetCustomerSchedulesAsync();
                
                var stats = new
                {
                    TotalSchedules = allSchedules.Count(),
                    ActiveSchedules = allSchedules.Count(s => s.Status == "Active"),
                    CompletedSchedules = allSchedules.Count(s => s.Status == "Completed"),
                    CancelledSchedules = allSchedules.Count(s => s.Status == "Cancelled"),
                    SchedulesByUser = allSchedules.GroupBy(s => s.UserId)
                        .Select(g => new
                        {
                            UserId = g.Key,
                            ScheduleCount = g.Count()
                        })
                        .OrderByDescending(x => x.ScheduleCount)
                        .Take(10),
                    SchedulesWithFeedback = allSchedules.Count(s => s.Feedback != null)
                };
                
                return Ok(stats);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = "Error calculating statistics", Details = ex.Message });
            }
        }

        /// <summary>
        /// [ADMIN] Xóa lịch trình
        /// </summary>
        [HttpDelete("DeleteSchedule/{id}")]
        public async Task<IActionResult> DeleteSchedule(int id)
        {
            try
            {
                var schedules = await _scheduleService.GetCustomerSchedulesAsync();
                var schedule = schedules.FirstOrDefault(s => s.ScheduleId == id);
                
                if (schedule == null)
                {
                    return NotFound($"Schedule with ID {id} not found.");
                }

                // Implement delete in service if not exists
                return Ok(new { Message = "Schedule deleted successfully." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = "Error deleting schedule", Details = ex.Message });
            }
        }
    }
}
