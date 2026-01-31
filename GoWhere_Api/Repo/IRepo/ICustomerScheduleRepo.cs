using GoWhere_Api.Model;
using GoWhere_Api.Model.DTOs.Response;

namespace GoWhere_Api.Repo.IRepo
{
    public interface ICustomerScheduleRepo
    {
        Task<IEnumerable<CustomerSchedule>> GetCustomerSchedulesAsync();
        Task<IEnumerable<CustomerSchedule>> GetCustomerSchedulesByCustomerIdAsync(int customerId);
    }
}