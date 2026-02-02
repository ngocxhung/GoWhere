using GoWhere_Api.Model;
using GoWhere_Api.Model.DTOs.Request;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.EntityFrameworkCore;

namespace GoWhere_Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ScenicSpotController : ControllerBase
    {
        private readonly Service.IService.IScenicSpotService _scenicSpotService;
        
        public ScenicSpotController(Service.IService.IScenicSpotService scenicSpotService)
        {
            _scenicSpotService = scenicSpotService;
        }

        // ============ USER ENDPOINTS (IsViewed = true only) ============

        /// <summary>
        /// Lấy tất cả địa điểm đã được duyệt (IsViewed = true)
        /// </summary>
        [HttpGet("GetAllScenicSpots")]
        [EnableQuery]
        public async Task<IActionResult> GetScenicSpots()
        {
            try
            {
                var scenicSpots = await _scenicSpotService.GetAllScenicSpotsAsync();
                return Ok(scenicSpots);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = "An error occurred", Details = ex.Message });
            }
        }

        /// <summary>
        /// Lấy địa điểm theo ID (chỉ IsViewed = true)
        /// </summary>
        [HttpGet("GetScenicSpotById/{id}")]
        public async Task<IActionResult> GetScenicSpotById(int id)
        {
            try
            {
                var scenicSpot = await _scenicSpotService.GetScenicSpotByIdAsync(id);   
                return Ok(scenicSpot);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = "An error occurred", Details = ex.Message });
            }
        }

        /// <summary>
        /// Lấy địa điểm theo tỉnh (chỉ IsViewed = true)
        /// </summary>
        [HttpGet("GetScenicSpotsByProvinceId/{provinceId}")]
        public async Task<IActionResult> GetScenicSpotsByProvinceId(int provinceId)
        {
            try
            {
                var scenicSpots = await _scenicSpotService.GetScenicSpotsByProvinceIdAsync(provinceId);
                return Ok(scenicSpots);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = "An error occurred", Details = ex.Message });
            }
        }

        // ============ ADMIN ENDPOINTS ============

        /// <summary>
        /// [ADMIN] Lấy TẤT CẢ địa điểm (bao gồm cả IsViewed = false)
        /// </summary>
        [HttpGet("Admin/GetAllScenicSpots")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetAllScenicSpotsAdmin()
        {
            try
            {
                var scenicSpots = await _scenicSpotService.GetAllScenicSpotsAdminAsync();
                return Ok(scenicSpots);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = "An error occurred", Details = ex.Message });
            }
        }

        /// <summary>
        /// [ADMIN] Thêm địa điểm mới (mặc định IsViewed = false)
        /// </summary>
        [HttpPost("AddScenicSpot")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> AddScenicSpot([FromForm] ScenicSpotRequestDto scenicSpotCreateDto)
        {
            try
            {
                await _scenicSpotService.AddScenicSpotAsync(scenicSpotCreateDto);
                return CreatedAtAction(nameof(GetScenicSpotById), null, new { Message = "Scenic spot created successfully. Status: Not viewed yet." });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { Message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = "Error adding scenic spot", Details = ex.Message });
            }
        }

        /// <summary>
        /// [ADMIN] Cập nhật thông tin địa điểm
        /// </summary>
        [HttpPut("UpdateScenicSpot/{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateScenicSpot(int id, [FromForm] ScenicSpotRequestDto scenicSpotUpdateDto)
        {
            try
            {
                await _scenicSpotService.UpdateScenicSpotAsync(id, scenicSpotUpdateDto);
                return Ok(new { Message = "Scenic spot updated successfully." });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = "Error updating scenic spot", Details = ex.Message });
            }
        }

        /// <summary>
        /// [ADMIN] Bật/tắt trạng thái hiển thị (IsViewed)
        /// </summary>
        [HttpPatch("ToggleStatus/{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> ToggleScenicSpotStatus(int id)
        {
            try
            {
                await _scenicSpotService.ToggleScenicSpotStatusAsync(id);
                return Ok(new { Message = "Scenic spot status toggled successfully." });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = "Error toggling status", Details = ex.Message });
            }
        }

        /// <summary>
        /// [ADMIN] Xóa địa điểm
        /// </summary>
        [HttpDelete("DeleteScenicSpot/{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteScenicSpot(int id)
        {
            try
            {
                await _scenicSpotService.DeleteScenicSpotAsync(id);
                return Ok(new { Message = "Scenic spot deleted successfully." });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = "Error deleting scenic spot", Details = ex.Message });
            }
        }
    }
}
