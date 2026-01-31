using GoWhere_Api.Model;
using GoWhere_Api.Repo.IRepo;
using Microsoft.EntityFrameworkCore;

namespace GoWhere_Api.Repo.Repo
{
    public class CCustomerScheduleRepo : ICustomerScheduleRepo
    {
        private readonly ApplicationDbContext _context;

        public CCustomerScheduleRepo(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<CustomerSchedule>> GetCustomerSchedulesAsync()
        {
            return await _context.CustomerSchedules.ToListAsync();
        }

        public async Task<IEnumerable<CustomerSchedule>> GetCustomerSchedulesByCustomerIdAsync(int customerId)
        {
            return await _context.CustomerSchedules
                .Where(cs => cs.UserId == customerId)
                .ToListAsync(); 
        }
    }
}
