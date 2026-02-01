using GoWhere_Api.Model;
using GoWhere_Api.Model.DTOs.Response;

namespace GoWhere_Api.Repo.IRepo
{
    public interface ICustomerScheduleRepo
    {
        Task AddCustomerScheduleAsync(CustomerSchedule customerSchedule);
        Task<CustomerSchedule> GetCustomerScheduleByIdAsync(int scheduleId);
        Task<IEnumerable<CustomerSchedule>> GetCustomerSchedulesAsync();
        Task<IEnumerable<CustomerSchedule>> GetCustomerSchedulesByCustomerIdAsync(int customerId);
        Task<ScenicSpot> GetScenicSpotByIdAsync(int spotId);
        bool IsScenicSpotExists(int spotId);
        Task UpdateCustomerScheduleAsync(CustomerSchedule schedule);
    }
}