using GoWhere_Api.Service.IService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.EntityFrameworkCore;

namespace GoWhere_Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class Provincecontroller : ControllerBase
    {
        private readonly IProvinceService _provinceService;
        public Provincecontroller(IProvinceService provinceService)
        {
            _provinceService = provinceService;
        }
        [HttpGet("GetAllProvinces")]
        public async Task<IActionResult> GetProvinces()
        {
            var provinces = await _provinceService.GetAllProvincesAsync();
            if (provinces == null || !provinces.Any())
            {
                return NotFound("No provinces found.");
            }
            return Ok(provinces);
        }
        [HttpGet("GetProviceById/{id}")]
        public async Task<IActionResult> GetProvinceById(int provinceId)
        {
            var province = await _provinceService.GetProvinceByIdAsync(provinceId);
            if (province == null)
            {
                return NotFound($"Province with ID {provinceId} not found.");
            }
            return Ok(province);


        }
    }
}
