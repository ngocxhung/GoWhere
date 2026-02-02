using GoWhere_Api.Model;
using GoWhere_Api.Repo.IRepo;
using Microsoft.EntityFrameworkCore;

namespace GoWhere_Api.Repo.Repo
{
    public class OrderRepo : IOrderRepo
    {
        private readonly ApplicationDbContext _context;

        public OrderRepo(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Order> CreateAsync(Order order)
        {
            _context.Orders.Add(order);
            await _context.SaveChangesAsync();
            return order;
        }

        public async Task<Order?> GetByIdAsync(int orderId)
        {
            return await _context.Orders
                .Include(o => o.User)
                .FirstOrDefaultAsync(o => o.OrderId == orderId);
        }

        public async Task<Order?> GetByUserAndStatusAsync(int userId, string status)
        {
            return await _context.Orders
                .FirstOrDefaultAsync(o => o.UserId == userId && o.Status == status);
        }

        public async Task<bool> UpdateAsync(Order order)
        {
            _context.Orders.Update(order);
            var result = await _context.SaveChangesAsync();
            return result > 0;
        }

        public async Task<List<Order>> GetByUserIdAsync(int userId)
        {
            return await _context.Orders
                .Where(o => o.UserId == userId)
                .OrderByDescending(o => o.OrderDate)
                .ToListAsync();
        }
    }
}
