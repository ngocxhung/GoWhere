using GoWhere_Api.Model;
using GoWhere_Api.Repo.IRepo;
using Microsoft.EntityFrameworkCore;

namespace GoWhere_Api.Repo.Repo
{
    public class ScenicSpotRepo : IScenicSpotRepo
    {
        private readonly ApplicationDbContext _context;
        
        public ScenicSpotRepo(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task AddScenicSpotAsync(ScenicSpot scenicSpot)
        {
            using (var transaction = await _context.Database.BeginTransactionAsync())
            {
                try
                {
                    await _context.ScenicSpots.AddAsync(scenicSpot);
                    foreach (var media in scenicSpot.Medias)
                    {
                        media.ScenicSpot = scenicSpot;
                        await _context.Medias.AddAsync(media);
                    }
                    await _context.SaveChangesAsync();
                    await transaction.CommitAsync();
                }
                catch (Exception ex)
                {
                    await transaction.RollbackAsync();
                    throw;
                }
            }
        }

        public async Task<IEnumerable<ScenicSpot>> GetAllScenicSpotsAsync()
        {
            return await _context.ScenicSpots
                .Include(s => s.Medias)
                .ToListAsync();
        }
        
        public Task<ScenicSpot> GetScenicSpotByIdAsync(int id)
        {
            return _context.ScenicSpots
                .Include(s => s.Medias)
                .FirstOrDefaultAsync(s => s.ScenicSpotId == id);
        }

        public async Task<IEnumerable<ScenicSpot>> GetScenicSpotsByProvinceIdAsync(int provinceId)
        {
            return await _context.ScenicSpots
                .Include(s => s.Medias)
                .Where(spot => spot.ProvinceId == provinceId)
                .ToListAsync();
        }

        public async Task UpdateSceneicSpotAsync(ScenicSpot scenicSpot)
        {
            _context.ScenicSpots.Update(scenicSpot);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteScenicSpotAsync(int id)
        {
            var scenicSpot = await _context.ScenicSpots
                .Include(s => s.Medias)
                .FirstOrDefaultAsync(s => s.ScenicSpotId == id);
            
            if (scenicSpot != null)
            {
                // Delete related medias first
                if (scenicSpot.Medias != null && scenicSpot.Medias.Any())
                {
                    _context.Medias.RemoveRange(scenicSpot.Medias);
                }
                
                _context.ScenicSpots.Remove(scenicSpot);
                await _context.SaveChangesAsync();
            }
        }
    }
}
