using GoWhere_Api.Model;
using GoWhere_Api.Repo.IRepo;
using Microsoft.EntityFrameworkCore;

namespace GoWhere_Api.Repo.Repo
{
    public class AISuggestionRequestRepo : IAISuggestionRequestRepo
    {
        private readonly ApplicationDbContext _context;

        public AISuggestionRequestRepo(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<AISuggestionRequest?> GetByUserIdAsync(int userId)
        {
            return await _context.AISuggestionRequests
                .Include(a => a.User)
                .Include(a => a.Order)
                .FirstOrDefaultAsync(a => a.UserId == userId);
        }

        public async Task<AISuggestionRequest> CreateAsync(AISuggestionRequest request)
        {
            await _context.AISuggestionRequests.AddAsync(request);
            await _context.SaveChangesAsync();
            return request;
        }

        public async Task UpdateAsync(AISuggestionRequest request)
        {
            if (request == null) return;

            _context.AISuggestionRequests.Update(request);
            await _context.SaveChangesAsync();
        }

        public async Task IncrementCountAsync(int userId)
        {
            await _context.Database.ExecuteSqlRawAsync(
                "UPDATE [AISuggestionRequests] SET [Count] = [Count] + 1 WHERE [UserId] = {0}",
                userId);
        }

        public async Task<int> GetRequestCountAsync(int userId)
        {
            var request = await GetByUserIdAsync(userId);
            return request?.Count ?? 0;
        }

        public async Task<bool> CanMakeFreeRequestAsync(int userId)
        {
            var count = await GetRequestCountAsync(userId);
            return count < 2;  // Free limit is 2
        }
    }
}
