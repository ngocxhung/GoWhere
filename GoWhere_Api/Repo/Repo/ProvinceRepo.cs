using GoWhere_Api.Model;
using GoWhere_Api.Repo.IRepo;
using Microsoft.EntityFrameworkCore;

namespace GoWhere_Api.Repo.Repo
{
    public class ProvinceRepo : IProvinceRepo
    {
        private readonly ApplicationDbContext _context;
        public ProvinceRepo(ApplicationDbContext context)
        {
            _context = context;
        }
        public async Task<IEnumerable<Province>> GetAllProvincesAsync()
        {
            return await _context.Provinces.ToListAsync();
        }

        public async Task<Province> GetProvinceByIdAsync(int provinceId)
        {
            return await _context.Provinces.FindAsync(provinceId);
        }
    }
}