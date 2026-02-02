using GoWhere_Api.Controllers;
using GoWhere_Api.Model.DTOs.Request;
using GoWhere_Api.Model.DTOs.Response;

namespace GoWhere_Api.Service.IService
{
    public interface IScenicSpotService
    {
        // User endpoints - chỉ trả về IsViewed = true
        Task<IEnumerable<ScenicSpotDto>> GetAllScenicSpotsAsync();
        Task<ScenicSpotDto> GetScenicSpotByIdAsync(int id);
        Task<IEnumerable<ScenicSpotDto>> GetScenicSpotsByProvinceIdAsync(int provinceId);
        
        // Admin endpoints - trả về tất cả
        Task<IEnumerable<ScenicSpotDto>> GetAllScenicSpotsAdminAsync();
        Task AddScenicSpotAsync(ScenicSpotRequestDto scenicSpotCreateDto);
        Task UpdateScenicSpotAsync(int id, ScenicSpotRequestDto scenicSpotUpdateDto);
        Task ToggleScenicSpotStatusAsync(int id);
        Task DeleteScenicSpotAsync(int id);
    }
}