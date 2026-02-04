using GoWhere_Api.Model.DTOs.Request;
using GoWhere_Api.Model.DTOs.Response;

namespace GoWhere_Api.Service.IService
{
    public interface IAIChatService
    {
        Task<AIScheduleResponseDto> GenerateScheduleAsync(int userId, AIScheduleRequestDto request);
        Task<AIUsageLimitResponseDto> CheckUsageLimitAsync(int userId);
        Task<AIChatResponseDto> ChatWithAIAsync(AIChatRequestDto request);
    }
}