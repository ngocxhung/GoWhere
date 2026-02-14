using GoWhere_Api.Model;
using GoWhere_Api.Model.DTOs.Request;
using GoWhere_Api.Model.DTOs.Response;

namespace GoWhere_Api.Service.IService
{
    public interface IAIChatService
    {
        Task<AIScheduleResponseDto> GenerateScheduleAsync(int userId, AIScheduleRequestDto request);
        Task<AIUsageLimitResponseDto> CheckUsageLimitAsync(int userId);
        Task<AIChatResponseDto> ChatWithAIAsync(AIChatRequestDto request);
        Task<List<AIScheduleHistory>> GetHistoryByUserIdAsync(int userId);
        Task<AIScheduleHistory?> GetHistoryByIdAsync(int userId, int historyId);
        Task<bool> DeleteHistoryAsync(int userId, int historyId);
        Task<AIPaymentOrderResponseDto> CreateAIPaymentOrderAsync(int userId);
    }
}