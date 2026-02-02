
using GoWhere_Api.Model.DTOs.Request;

namespace GoWhere_Api.Service.IService
{
    public interface ICustomerScheduleService
    {
        Task AddFeedbackAsync(int customerId, FeedbackRequestDto request);
        Task AddNewCustomerScheduleAsync(int customerId, Model.DTOs.Request.CustomerScheduleRequestDto request);
        Task<IEnumerable<Model.DTOs.Response.CustomerScheduleRespnse>> GetCustomerSchedulesAsync();
        Task<IEnumerable<Model.DTOs.Response.CustomerScheduleRespnse>> GetCustomerSchedulesByCustomerIdAsync(int customerId);
        Task UpdateCustomerScheduleStatusAsync(int v, int scheduleId, string status);
    }
}