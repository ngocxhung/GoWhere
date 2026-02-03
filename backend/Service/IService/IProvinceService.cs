
using GoWhere_Api.Model.DTOs.Response;

namespace GoWhere_Api.Service.IService
{
    public interface IProvinceService
    {
        Task<IEnumerable<Model.DTOs.Response.ProvinceResponseDto>> GetAllProvincesAsync();
        Task<ProvinceResponseDto> GetProvinceByIdAsync(int provinceId);
    }
}