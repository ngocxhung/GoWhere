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
        [HttpGet("GetAllScenicSpots")]
        [EnableQuery]
        public async Task<IActionResult> GetScenicSpots()
        {
            var scenicSpots = await _scenicSpotService.GetAllScenicSpotsAsync();
            if (scenicSpots == null || !scenicSpots.Any())
            {
                return NotFound("No scenic spots found.");
            }
            return Ok(scenicSpots);
        }
        [HttpGet("GetScenicSpotById/{id}")]
        public async Task<IActionResult> GetScenicSpotById(int id)
        {
            var scenicSpot = await _scenicSpotService.GetScenicSpotByIdAsync(id);   
            if (scenicSpot == null)
            {
                return NotFound($"Scenic spot with ID {id} not found.");
            }
            return Ok(scenicSpot);

        }
        [HttpGet("GetScenicSpotsByProvinceId/{provinceId}")]
        public async Task<IActionResult> GetScenicSpotsByProvinceId(int provinceId)
        {
            var scenicSpots = await _scenicSpotService.GetScenicSpotsByProvinceIdAsync(provinceId);
            if (scenicSpots == null || !scenicSpots.Any())
            {
                return NotFound($"No scenic spots found for province ID {provinceId}.");
            }
            return Ok(scenicSpots);

        }
        [HttpPost("AddScenicSpot")]
        public async Task<IActionResult> AddScenicSpot([FromForm] ScenicSpotRequestDto scenicSpotCreateDto)
        {
            try
            {
                await _scenicSpotService.AddScenicSpotAsync(scenicSpotCreateDto);
                return CreatedAtAction(null, "Create ScenicSpot successful");

            }
            catch (Exception ex)
            {
                return BadRequest($"Error adding scenic spot: {ex.Message}");
            }
        }
            
        
    }
    
}
