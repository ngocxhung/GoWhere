using GoWhere_Api.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GoWhere_Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DiscoveryController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public DiscoveryController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet("Hotels")]
        public async Task<IActionResult> GetHotels([FromQuery] string? location, [FromQuery] string? provinceIds)
        {
            var query = _context.Hotels.AsQueryable();

            if (!string.IsNullOrEmpty(provinceIds))
            {
                var ids = provinceIds.Split(',').Select(s => int.TryParse(s, out var i) ? i : (int?)null).Where(i => i.HasValue).Select(i => i.Value).ToList();
                if (ids.Any())
                {
                    query = query.Where(h => h.ProvinceId.HasValue && ids.Contains(h.ProvinceId.Value));
                }
            }
            else if (!string.IsNullOrEmpty(location))
            {
                var locs = location.Split(',').Select(l => l.Trim()).ToList();
                query = query.Where(h => locs.Any(l => h.Location.Contains(l)));
            }

            var results = await query.Include(h => h.Province).ToListAsync();
            return Ok(results);
        }

        [HttpGet("Flights")]
        public async Task<IActionResult> GetFlights([FromQuery] string? destinationCode)
        {
            var query = _context.Flights.AsQueryable();
            if (!string.IsNullOrEmpty(destinationCode))
            {
                query = query.Where(f => f.ToCode == destinationCode);
            }
            return Ok(await query.ToListAsync());
        }

        [HttpGet("Cars")]
        public async Task<IActionResult> GetCars()
        {
            return Ok(await _context.CarRentals.ToListAsync());
        }

        [HttpGet("Activities")]
        public async Task<IActionResult> GetActivities([FromQuery] string? location)
        {
            var query = _context.Activities.AsQueryable();
            if (!string.IsNullOrEmpty(location))
            {
                var locs = location.Split(',').Select(l => l.Trim()).ToList();
                query = query.Where(a => locs.Any(l => a.Location.Contains(l)));
            }
            return Ok(await query.ToListAsync());
        }
    }
}
