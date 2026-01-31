using GoWhere_Api.Controllers;
using GoWhere_Api.Model.DTOs.Request;
using GoWhere_Api.Model.DTOs.Response;

namespace GoWhere_Api.Service.IService
{
    public interface IScenicSpotService
    {
        Task AddScenicSpotAsync(ScenicSpotRequestDto scenicSpotCreateDto);
        Task<IEnumerable<Model.DTOs.Response.ScenicSpotDto>> GetAllScenicSpotsAsync();
        Task<ScenicSpotDto> GetScenicSpotByIdAsync(int id);
        Task<IEnumerable<ScenicSpotDto>> GetScenicSpotsByProvinceIdAsync(int provinceId);
        Task UpdateScenicSpotAsync(int id, ScenicSpotRequestDto scenicSpotUpdateDto);
    }
}