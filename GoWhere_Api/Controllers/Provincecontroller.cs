using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GoWhere_Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class Provincecontroller : ControllerBase
    {
        private readonly Model.ApplicationDbContext _context;
        public Provincecontroller(Model.ApplicationDbContext context)
        {
            _context = context;
        }
        [HttpGet("GetAllProvinces")]

        public async Task<IActionResult> GetProvinces()
        {
            var provinces = await _context.Provinces
                .Select(p => new
                {
                    Id = p.ProvinceId,
                    Name = p.ProvinceName,



                }).ToListAsync();
            if (provinces == null || provinces.Count == 0)
            {
                return NotFound("No provinces found.");


            }
            return Ok(provinces);
        }
        [HttpGet("GetProviceById/{id}")]
        public async Task<IActionResult> GetProvinceById(int provinceId)
        {
            var province = await _context.Provinces
                .Select(p => new
                {
                    Id = p.ProvinceId,
                    Name = p.ProvinceName
                }).FirstOrDefaultAsync(p => p.Id == provinceId);
            if(province == null)
            {
                return NotFound($"Province with Id: {provinceId} not found");
            }
            return Ok(province);

        }
    }
}
