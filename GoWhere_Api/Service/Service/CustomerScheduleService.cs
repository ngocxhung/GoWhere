using GoWhere_Api.Model.DTOs.Response;
using GoWhere_Api.Service.IService;

namespace GoWhere_Api.Service.Service
{
    public class CustomerScheduleService : ICustomerScheduleService
    {
        private readonly Repo.IRepo.ICustomerScheduleRepo _customerScheduleRepo;
        public CustomerScheduleService(Repo.IRepo.ICustomerScheduleRepo customerScheduleRepo)
        {
            _customerScheduleRepo = customerScheduleRepo;
        }

        public async Task<IEnumerable<CustomerScheduleRespnse>> GetCustomerSchedulesAsync()
        {
            try
            {
                var schedules = await _customerScheduleRepo.GetCustomerSchedulesAsync();
                var response = schedules.Select(schedule => new CustomerScheduleRespnse
                {
                    ScheduleId = schedule.ScheduleId,
                    StartTime = schedule.StartTime,
                    EndTime = schedule.EndTime,
                    Status = schedule.Status,
                    ScenicSpots = schedule.ScenicSpots.Select(spot => new ScenicSpotDto
                    {
                        Id = spot.ScenicSpotId,
                        ScenicSpotName = spot.ScenicSpotName,
                        Description = spot.Description,
                        EstimatedVisitPrice = spot.EstimatedVisitPrice,
                    }).ToList()
                }).ToList();
                return response;

            }
            catch (Exception ex)
            {
                // Log the exception (logging mechanism not shown here)
                throw new ApplicationException("An error occurred while retrieving customer schedules.", ex);
            }
           
        }

        public async Task<IEnumerable<CustomerScheduleRespnse>> GetCustomerSchedulesByCustomerIdAsync(int customerId)
        {
            try
            {
                var schedules = await _customerScheduleRepo.GetCustomerSchedulesByCustomerIdAsync(customerId);
                if(schedules == null || !schedules.Any())
                {
                    throw new KeyNotFoundException($"No schedules found for customer with ID {customerId}.");
                }
                var response = schedules.Select(schedule => new CustomerScheduleRespnse
                {
                    ScheduleId = schedule.ScheduleId,
                    StartTime = schedule.StartTime,
                    EndTime = schedule.EndTime,
                    Status = schedule.Status,
                    ScenicSpots = schedule.ScenicSpots.Select(spot => new ScenicSpotDto
                    {
                        Id = spot.ScenicSpotId,
                        ScenicSpotName = spot.ScenicSpotName,
                        Description = spot.Description,
                        EstimatedVisitPrice = spot.EstimatedVisitPrice,
                    }).ToList()
                });
                return response;

            }
            catch (Exception ex)
            {
                // Log the exception (logging mechanism not shown here)
                throw new ApplicationException("An error occurred while retrieving customer schedules.", ex);
            }


        }
    }
}
