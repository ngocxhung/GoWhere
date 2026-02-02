

using GoWhere_Api.Model;

namespace GoWhere_Api.Repo.IRepo
{
    public interface IScenicSpotRepo
    {
        Task<IEnumerable<ScenicSpot>> GetAllScenicSpotsAsync();
        Task<ScenicSpot> GetScenicSpotByIdAsync(int id);
        Task AddScenicSpotAsync(ScenicSpot scenicSpot);
        Task<IEnumerable<ScenicSpot>> GetScenicSpotsByProvinceIdAsync(int provinceId);
        Task UpdateSceneicSpotAsync(ScenicSpot scenicSpot);
        Task DeleteScenicSpotAsync(int id);
    }
}