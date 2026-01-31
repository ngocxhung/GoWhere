
namespace GoWhere_Api.Service.IService
{
    public interface ICustomerScheduleService
    {
        Task<IEnumerable<Model.DTOs.Response.CustomerScheduleRespnse>> GetCustomerSchedulesAsync();
        Task<IEnumerable<Model.DTOs.Response.CustomerScheduleRespnse>> GetCustomerSchedulesByCustomerIdAsync(int customerId);
    }
}