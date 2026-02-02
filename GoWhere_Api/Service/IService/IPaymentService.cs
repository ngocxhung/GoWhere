using GoWhere_Api.Model.DTOs.Request;
using GoWhere_Api.Model.DTOs.Response;

namespace GoWhere_Api.Service.IService
{
    public interface IPaymentService
    {
        Task<PaymentResponseDto> CreateAIChatPaymentAsync(int userId);
        Task<bool> ConfirmPaymentAsync(int orderId);
        Task<List<PaymentResponseDto>> GetUserPaymentHistoryAsync(int userId);
    }
}
