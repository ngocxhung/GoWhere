using GoWhere_Api.Model;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GoWhere_Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "2")] // RoleId = 2 is Admin
    public class AdminController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public AdminController(ApplicationDbContext context)
        {
            _context = context;
        }

        // --- DASHBOARD STATS ---
        [HttpGet("DashboardStats")]
        public async Task<IActionResult> GetDashboardStats()
        {
            var usersCount = await _context.Users.CountAsync();
            var spotsCount = await _context.ScenicSpots.CountAsync();
            var hotelsCount = await _context.Hotels.CountAsync();
            var ordersCount = await _context.Orders.CountAsync();
            var revenue = await _context.Orders.Where(o => o.Status == "Paid" || o.Status == "Payed").SumAsync(o => o.TotalPrice);

            return Ok(new
            {
                UsersCount = usersCount,
                SpotsCount = spotsCount,
                HotelsCount = hotelsCount,
                OrdersCount = ordersCount,
                Revenue = revenue
            });
        }

        // --- USER MANAGEMENT ---
        [HttpGet("Users")]
        public async Task<IActionResult> GetUsers()
        {
            var users = await _context.Users.Include(u => u.Role).ToListAsync();
            return Ok(users);
        }

        [HttpDelete("Users/{id}")]
        public async Task<IActionResult> DeleteUser(int id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null) return NotFound();
            
            _context.Users.Remove(user);
            await _context.SaveChangesAsync();
            return Ok(new { Message = "User deleted successfully" });
        }

        // --- SCENIC SPOT MANAGEMENT ---
        [HttpGet("ScenicSpots")]
        public async Task<IActionResult> GetScenicSpots()
        {
            var spots = await _context.ScenicSpots.Include(s => s.Province).ToListAsync();
            return Ok(spots);
        }

        [HttpPost("ScenicSpots")]
        public async Task<IActionResult> CreateScenicSpot([FromBody] ScenicSpot spot)
        {
            _context.ScenicSpots.Add(spot);
            await _context.SaveChangesAsync();
            return Ok(spot);
        }

        [HttpPut("ScenicSpots/{id}")]
        public async Task<IActionResult> UpdateScenicSpot(int id, [FromBody] ScenicSpot spot)
        {
            if (id != spot.ScenicSpotId) return BadRequest();
            _context.Entry(spot).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return Ok(spot);
        }

        [HttpDelete("ScenicSpots/{id}")]
        public async Task<IActionResult> DeleteScenicSpot(int id)
        {
            var spot = await _context.ScenicSpots.FindAsync(id);
            if (spot == null) return NotFound();
            _context.ScenicSpots.Remove(spot);
            await _context.SaveChangesAsync();
            return Ok(new { Message = "Deleted" });
        }

        // --- HOTEL MANAGEMENT ---
        [HttpGet("Hotels")]
        public async Task<IActionResult> GetHotels()
        {
            var hotels = await _context.Hotels.Include(h => h.Province).ToListAsync();
            return Ok(hotels);
        }

        [HttpPost("Hotels")]
        public async Task<IActionResult> CreateHotel([FromBody] Hotel hotel)
        {
            _context.Hotels.Add(hotel);
            await _context.SaveChangesAsync();
            return Ok(hotel);
        }

        // --- CAR RENTAL MANAGEMENT ---
        [HttpGet("CarRentals")]
        public async Task<IActionResult> GetCarRentals()
        {
            var cars = await _context.CarRentals.ToListAsync();
            return Ok(cars);
        }

        // --- PAYMENT/ORDER MANAGEMENT ---
        [HttpGet("Orders")]
        public async Task<IActionResult> GetOrders()
        {
            var orders = await _context.Orders.Include(o => o.User).OrderByDescending(o => o.OrderDate).ToListAsync();
            return Ok(orders);
        }

        [HttpPost("Orders/{id}/Confirm")]
        public async Task<IActionResult> ConfirmOrder(int id)
        {
            var order = await _context.Orders.FindAsync(id);
            if (order == null) return NotFound();
            
            order.Status = "Paid";
            await _context.SaveChangesAsync();
            
            // If it was an AI Unlimited purchase, we might need to update the AISuggestionRequest limit
            // (Assuming business logic handles this)
            
            return Ok(new { Message = "Order confirmed" });
        }
    }
}
