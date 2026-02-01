using GoWhere_Api.Model;
using GoWhere_Api.Repo.IRepo;
using Microsoft.EntityFrameworkCore;

namespace GoWhere_Api.Repo.Repo
{
    public class CustomerScheduleRepo : ICustomerScheduleRepo
    {
        private readonly ApplicationDbContext _context;

        public CustomerScheduleRepo(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task AddCustomerScheduleAsync(CustomerSchedule customerSchedule)
        {
            await _context.CustomerSchedules.AddAsync(customerSchedule);
            await _context.SaveChangesAsync();
        }

        public async Task<CustomerSchedule> GetCustomerScheduleByIdAsync(int scheduleId)
        {
            return await _context.CustomerSchedules.FindAsync(scheduleId);
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

        public async Task<ScenicSpot> GetScenicSpotByIdAsync(int spotId)
        {
            return await _context.ScenicSpots.FindAsync(spotId);
        }

        public bool IsScenicSpotExists(int spotId)
        {
            return _context.ScenicSpots.Any(spot => spot.ScenicSpotId == spotId);
        }

        public async Task UpdateCustomerScheduleAsync(CustomerSchedule schedule)
        {
            _context.CustomerSchedules.Update(schedule);
            await _context.SaveChangesAsync();
        }
    }
}
