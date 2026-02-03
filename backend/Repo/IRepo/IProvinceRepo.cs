using GoWhere_Api.Model;
using GoWhere_Api.Model.DTOs.Response;
using GoWhere_Api.Repo.IRepo;
using Microsoft.EntityFrameworkCore;

namespace GoWhere_Api.Repo.IRepo
{
    public interface IProvinceRepo
    {
        Task<IEnumerable<Province>> GetAllProvincesAsync();
        Task<Province> GetProvinceByIdAsync(int provinceId);
    }
}




