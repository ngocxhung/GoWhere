using GoWhere_Api.Model;
using GoWhere_Api.Model;
using GoWhere_Api.Repo.IRepo;
using Microsoft.EntityFrameworkCore;

namespace GoWhere_Api.Repo.Repo
{
    public class AIScheduleHistoryRepo : IAIScheduleHistoryRepo
    {
        private readonly ApplicationDbContext _context;

        public AIScheduleHistoryRepo(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<AIScheduleHistory> CreateAsync(AIScheduleHistory history)
        {
            _context.AIScheduleHistories.Add(history);
            await _context.SaveChangesAsync();
            return history;
        }

        public async Task<List<AIScheduleHistory>> GetByUserIdAsync(int userId)
        {
            return await _context.AIScheduleHistories
                .Where(h => h.UserId == userId)
                .Include(h => h.Province)
                .OrderByDescending(h => h.CreatedAt)
                .ToListAsync();
        }

        public async Task<AIScheduleHistory?> GetByIdAsync(int id)
        {
            return await _context.AIScheduleHistories
                .Include(h => h.Province)
                .FirstOrDefaultAsync(h => h.Id == id);
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var history = await _context.AIScheduleHistories.FindAsync(id);
            if (history == null) return false;

            _context.AIScheduleHistories.Remove(history);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
