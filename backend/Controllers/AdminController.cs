using GoWhere_Api.Model;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GoWhere_Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Admin")] // Require role name "Admin"
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
            // Set Province to null to avoid EF trying to create/update the province
            spot.Province = null;
            
            _context.ScenicSpots.Add(spot);
            await _context.SaveChangesAsync();
            return Ok(spot);
        }

        [HttpPut("ScenicSpots/{id}")]
        public async Task<IActionResult> UpdateScenicSpot(int id, [FromBody] ScenicSpot spot)
        {
            if (id != spot.ScenicSpotId) return BadRequest();

            var existingSpot = await _context.ScenicSpots
                .Include(s => s.Medias)
                .FirstOrDefaultAsync(s => s.ScenicSpotId == id);

            if (existingSpot == null) return NotFound();

            // Update simple properties
            existingSpot.ScenicSpotName = spot.ScenicSpotName;
            existingSpot.Description = spot.Description;
            existingSpot.EstimatedVisitPrice = spot.EstimatedVisitPrice;
            existingSpot.IsViewed = spot.IsViewed;
            existingSpot.ImageUrl = spot.ImageUrl;
            existingSpot.ProvinceId = spot.ProvinceId;

            // Update Medias: Delete all and re-add for simplicity
            if (existingSpot.Medias != null)
            {
                _context.Set<Media>().RemoveRange(existingSpot.Medias);
            }

            if (spot.Medias != null)
            {
                existingSpot.Medias = spot.Medias.Select(m => new Media { 
                    MediaUrl = m.MediaUrl, 
                    ScenicSpotId = id 
                }).ToList();
            }

            await _context.SaveChangesAsync();
            return Ok(existingSpot);
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
            
            Console.WriteLine($"[DEBUG] Confirming Order #{id}...");

            try
            {
                // 1. Get User ID from Order (using a fresh look)
                var orderInfo = await _context.Orders.AsNoTracking().FirstOrDefaultAsync(o => o.OrderId == id);
                if (orderInfo == null) return NotFound("Order not found");
                int userId = orderInfo.UserId;
                Console.WriteLine($"[DEBUG] Order #{id} belongs to User {userId}");

                // 2. Update Order Status - Direct SQL
                await _context.Database.ExecuteSqlRawAsync(
                    "UPDATE Orders SET Status = 'Payed' WHERE OrderId = {0}", id);

                // 3. Update AI Turns - Direct SQL with brackets for [Count]
                // Setting to 25 to be absolutely sure we see a change
                var affected = await _context.Database.ExecuteSqlRawAsync(
                    @"UPDATE [AISuggestionRequests] 
                      SET [AllowedCount] = 25, 
                          [OrderId] = {0}, 
                          [Price] = 55555 
                      WHERE [UserId] = {1}", id, userId);

                Console.WriteLine($"[DEBUG] SQL Update affected {affected} rows for User {userId}");

                // 4. VERIFY IMMEDIATELY
                var verify = await _context.AISuggestionRequests.AsNoTracking().FirstOrDefaultAsync(r => r.UserId == userId);
                if (verify != null) {
                    Console.WriteLine($"[DEBUG] VERIFIED IN DB: UserId={userId}, Count={verify.Count}, AllowedCount={verify.AllowedCount}, OrderId={verify.OrderId}");
                } else {
                    Console.WriteLine($"[DEBUG] VERIFY FAILED: No record found for User {userId} after update!");
                }

                if (affected == 0)
                {
                    Console.WriteLine($"[DEBUG] Creating new record as fallback...");
                    var newReq = new AISuggestionRequest
                    {
                        UserId = userId,
                        Count = 0,
                        AllowedCount = 25,
                        OrderId = id,
                        Price = 50000
                    };
                    _context.AISuggestionRequests.Add(newReq);
                    await _context.SaveChangesAsync();
                }

                return Ok(new { Message = "Success", UserId = userId, CurrentAllowed = verify?.AllowedCount });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ERROR] {ex.Message}");
                return StatusCode(500, ex.Message);
            }
        }
    }
}
