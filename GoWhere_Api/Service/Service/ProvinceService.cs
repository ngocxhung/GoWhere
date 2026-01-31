using GoWhere_Api.Model.DTOs.Response;
using GoWhere_Api.Service.IService;

namespace GoWhere_Api.Service.Service
{
    public class ProvinceService : IProvinceService
    {
        private readonly Repo.IRepo.IProvinceRepo _provinceRepo;
        public ProvinceService(Repo.IRepo.IProvinceRepo provinceRepo)
        {
            _provinceRepo = provinceRepo;
        }
        public async Task<IEnumerable<ProvinceResponseDto>> GetAllProvincesAsync() 
        
        {
            try
            {
                var provinces = await _provinceRepo.GetAllProvincesAsync();
                if (provinces == null || !provinces.Any())
                {
                    throw new KeyNotFoundException("No provinces found.");
                }

                return provinces.Select(p => new ProvinceResponseDto
                {
                    ProvinceId = p.ProvinceId,
                    ProvinceName = p.ProvinceName
                }).ToList();
            }
            catch (Exception ex)
            {
                // Log the exception (logging mechanism not shown here)
                throw new ApplicationException("An error occurred while retrieving provinces.", ex);
            }
        }
        public async Task<ProvinceResponseDto> GetProvinceByIdAsync(int provinceId) {
            try
            {
                var province = await _provinceRepo.GetProvinceByIdAsync(provinceId);
                if (province == null)
                {
                    throw new KeyNotFoundException($"Province with ID {provinceId} not found.");
                }
                return new ProvinceResponseDto
                {
                    ProvinceId = province.ProvinceId,
                    ProvinceName = province.ProvinceName
                };
            }
            catch (Exception ex)
            {
                // Log the exception (logging mechanism not shown here)
                throw new ApplicationException($"An error occurred while retrieving province with ID {provinceId}.", ex);
            }   
        }
    }
}