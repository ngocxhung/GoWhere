using GoWhere_Api.Model;
using GoWhere_Api.Model.DTOs.Request;
using GoWhere_Api.Model.DTOs.Response;
using GoWhere_Api.Service.IService;
using Microsoft.IdentityModel.Tokens;

namespace GoWhere_Api.Service.Service
{
    public class CustomerScheduleService : ICustomerScheduleService
    {
        private readonly Repo.IRepo.ICustomerScheduleRepo _customerScheduleRepo;
        public CustomerScheduleService(Repo.IRepo.ICustomerScheduleRepo customerScheduleRepo)
        {
            _customerScheduleRepo = customerScheduleRepo;
        }

        public async Task AddFeedbackAsync(int customerId, FeedbackRequestDto request)
        {
            try
            {
                var isCustomerOwnsSchedule = await _customerScheduleRepo.IsCustomerOwnsScheduleAsync(customerId, request.ScheduleId);

                if (!isCustomerOwnsSchedule)
                {
                    throw new UnauthorizedAccessException("You do not have permission to add feedback to this schedule.");
                }
                // Check if the schedule is completed
                var schedule = await _customerScheduleRepo.GetCustomerScheduleByIdAsync(request.ScheduleId);
                if (schedule == null || schedule.Status != ScheduleStatus.Completed.ToString())
                {
                    throw new InvalidOperationException("Feedback can only be added to completed schedules.");
                }
                var feedback = new Feedback
                {
                    ScheduleId = request.ScheduleId,
                    Comments = request.Comments,
                    Rating = request.Rating
                };
                await _customerScheduleRepo.AddFeedbackAsync(feedback);
            }
            catch (Exception ex)
            {
                // Log the exception (logging mechanism not shown here)
                throw new ApplicationException("An error occurred while adding feedback.", ex);
            }
        }

        public async Task AddNewCustomerScheduleAsync(int customerId, CustomerScheduleRequestDto request)
        {
            if (request == null)
                throw new ArgumentNullException(nameof(request));

            if (customerId <= 0)
                throw new ArgumentException("Invalid customer id.", nameof(customerId));

            if (request.EndTime <= request.StartTime)
                throw new ArgumentException("EndTime must be after StartTime.", nameof(request));

           
            try
            {
                var customerSchedule = new Model.CustomerSchedule
                {
                    UserId = customerId,
                    StartTime = request.StartTime,
                    EndTime = request.EndTime,
                    Status = ScheduleStatus.Pending.ToString(),
                    ScenicSpots = new List<ScenicSpot>()
                };

                if (request.ScenicSpots != null && request.ScenicSpots.Any())
                {
                    foreach (var spotId in request.ScenicSpots)
                    {
                        if (_customerScheduleRepo.IsScenicSpotExists(spotId))
                        {
                            var scenicSpot = await _customerScheduleRepo.GetScenicSpotByIdAsync(spotId);
                            if (scenicSpot == null)
                                throw new KeyNotFoundException($"Scenic spot with ID {spotId} does not exist.");

                            customerSchedule.ScenicSpots.Add(scenicSpot);
                        }
                        else
                        {
                            throw new KeyNotFoundException($"Scenic spot with ID {spotId} does not exist.");
                        }
                    }
                }

                await _customerScheduleRepo.AddCustomerScheduleAsync(customerSchedule);
            }
            catch (Exception ex)
            {
                // Log the exception (logging mechanism not shown here)
                throw new ApplicationException("An error occurred while adding a new customer schedule.", ex);
            }
        }

       

        public async Task AddNewCustomerScheduleAsync(string customerId, CustomerScheduleRequestDto request)
        {
            if (request == null)
                throw new ArgumentNullException(nameof(request));

            if (string.IsNullOrWhiteSpace(customerId) || !int.TryParse(customerId, out var parsedUserId))
                throw new ArgumentException("Invalid customer id.", nameof(customerId));

            if (request.EndTime <= request.StartTime)
                throw new ArgumentException("EndTime must be after StartTime.", nameof(request));

            try
            {
                var customerSchedule = new Model.CustomerSchedule
                {
                    UserId = parsedUserId,
                    StartTime = request.StartTime,
                    EndTime = request.EndTime,
                    Status = ScheduleStatus.Pending.ToString(),
                    ScenicSpots = new List<ScenicSpot>()
                };

                if (request.ScenicSpots != null && request.ScenicSpots.Any())
                {
                    foreach (var spotId in request.ScenicSpots)
                    {
                        if (_customerScheduleRepo.IsScenicSpotExists(spotId))
                        {
                            var scenicSpot = await _customerScheduleRepo.GetScenicSpotByIdAsync(spotId);
                            if (scenicSpot == null)
                                throw new KeyNotFoundException($"Scenic spot with ID {spotId} does not exist.");

                            customerSchedule.ScenicSpots.Add(scenicSpot);
                        }
                        else
                        {
                            throw new KeyNotFoundException($"Scenic spot with ID {spotId} does not exist.");
                        }
                    }
                }

                await _customerScheduleRepo.AddCustomerScheduleAsync(customerSchedule);
            }
            catch (Exception ex)
            {
                // Log the exception (logging mechanism not shown here)
                throw new ApplicationException("An error occurred while adding a new customer schedule.", ex);
            }
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
                    }).ToList(),
                    Feedback = new FeedbackDto
                    {
                        FeedbackId = schedule.Feedback?.FeedbackId ?? 0,
                        UserId = schedule.UserId,
                        Comments = schedule.Feedback?.Comments ?? string.Empty,
                        Rating = schedule.Feedback != null ? schedule.Feedback.Rating : 0
                    }
                   
                    
                  
                 
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

                    }).ToList(),
                    Feedback = schedule.Feedback != null ? new FeedbackDto
                    {
                        FeedbackId = schedule.Feedback.FeedbackId,
                        Comments = schedule.Feedback.Comments,
                        Rating = schedule.Feedback.Rating
                    } : null
                }).ToList();
                return response;

            }
            catch (Exception ex)
            {
                // Log the exception (logging mechanism not shown here)
                throw new ApplicationException("An error occurred while retrieving customer schedules.", ex);
            }


        }

        public async Task UpdateCustomerScheduleStatusAsync(int customerId, int scheduleId, string status)
        {
            try
            {
                var schedule = await _customerScheduleRepo.GetCustomerScheduleByIdAsync(scheduleId);
                if (schedule == null)
                {
                    throw new KeyNotFoundException($"Schedule with ID {scheduleId} not found.");
                }
                if (schedule.UserId != customerId)
                {
                    throw new UnauthorizedAccessException("You do not have permission to update this schedule.");
                }
                var validStatuses = new List<string> { ScheduleStatus.Pending.ToString(), ScheduleStatus.Confirmed.ToString(), ScheduleStatus.Cancelled.ToString(), ScheduleStatus.Completed.ToString() };
                if (!validStatuses.Contains(status))
                {
                    throw new ArgumentException($"Invalid status. Valid statuses are: {string.Join(", ", validStatuses)}");
                }
                schedule.Status = status;
                await _customerScheduleRepo.UpdateCustomerScheduleAsync(schedule);
            }
            catch (Exception ex)
            {
                // Log the exception (logging mechanism not shown here)
                throw new ApplicationException("An error occurred while updating customer schedule status.", ex);
            }
        }
    }
}
