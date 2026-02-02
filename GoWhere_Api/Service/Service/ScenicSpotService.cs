using GoWhere_Api.Model;
using GoWhere_Api.Model.DTOs.Request;
using GoWhere_Api.Model.DTOs.Response;
using GoWhere_Api.Service.IService;
using System.Diagnostics;

namespace GoWhere_Api.Service.Service
{
    public class ScenicSpotService : IScenicSpotService
    {
        private readonly Repo.IRepo.IScenicSpotRepo _scenicSpotRepo;
        private readonly IFileService _fileService;
        private readonly long _maxFileSize = 250 * 1024 * 1024; // 250 MB
        
        public ScenicSpotService(Repo.IRepo.IScenicSpotRepo scenicSpotRepo, IService.IFileService fileService)
        {
            _scenicSpotRepo = scenicSpotRepo;
            _fileService = fileService;
        }

        // ============ USER ENDPOINTS (IsViewed = true only) ============
        
        public async Task<IEnumerable<ScenicSpotDto>> GetAllScenicSpotsAsync()
        {
            try
            {
                var scenicSpots = await _scenicSpotRepo.GetAllScenicSpotsAsync();
                
                // Filter only IsViewed = true for normal users
                var viewedSpots = scenicSpots.Where(s => s.IsViewed).ToList();
                
                if (!viewedSpots.Any())
                {
                    throw new KeyNotFoundException("No scenic spots found.");
                }
                
                return viewedSpots.Select(spot => new ScenicSpotDto
                {
                    Id = spot.ScenicSpotId,
                    ScenicSpotName = spot.ScenicSpotName,
                    Description = spot.Description,
                    EstimatedVisitPrice = spot.EstimatedVisitPrice,
                    MediaUrls = spot.Medias.Select(media => media.MediaUrl).ToList(),
                    ProvinceId = spot.ProvinceId,
                    IsViewed = spot.IsViewed
                }).ToList();
            }
            catch (Exception ex)
            {
                throw new ApplicationException("An error occurred while retrieving scenic spots.", ex);
            }
        }

        public async Task<ScenicSpotDto> GetScenicSpotByIdAsync(int id)
        {
            try
            { 
                var spot = await _scenicSpotRepo.GetScenicSpotByIdAsync(id);
                
                if (spot == null)
                {
                    throw new KeyNotFoundException($"Scenic spot with ID {id} not found.");
                }
                
                // Only return if IsViewed = true (for normal users)
                if (!spot.IsViewed)
                {
                    throw new KeyNotFoundException($"Scenic spot with ID {id} not found.");
                }
                
                return new ScenicSpotDto
                {
                    Id = spot.ScenicSpotId,
                    ScenicSpotName = spot.ScenicSpotName,
                    Description = spot.Description,
                    EstimatedVisitPrice = spot.EstimatedVisitPrice,
                    MediaUrls = spot.Medias.Select(media => media.MediaUrl).ToList(),
                    ProvinceId = spot.ProvinceId,
                    IsViewed = spot.IsViewed
                };
            }
            catch (Exception ex)
            {
                throw new ApplicationException($"An error occurred while retrieving scenic spot with ID {id}.", ex);
            }
        }

        public async Task<IEnumerable<ScenicSpotDto>> GetScenicSpotsByProvinceIdAsync(int provinceId)
        {
            try
            {
                var scenicSpots = await _scenicSpotRepo.GetScenicSpotsByProvinceIdAsync(provinceId);
                
                // Filter only IsViewed = true
                var viewedSpots = scenicSpots.Where(s => s.IsViewed).ToList();
                
                if (!viewedSpots.Any())
                {
                    throw new KeyNotFoundException($"No scenic spots found for province ID {provinceId}.");
                }
                
                return viewedSpots.Select(spot => new ScenicSpotDto
                {
                    Id = spot.ScenicSpotId,
                    ScenicSpotName = spot.ScenicSpotName,
                    Description = spot.Description,
                    EstimatedVisitPrice = spot.EstimatedVisitPrice,
                    MediaUrls = spot.Medias.Select(media => media.MediaUrl).ToList(),
                    ProvinceId = spot.ProvinceId,
                    IsViewed = spot.IsViewed
                }).ToList();
            }
            catch (Exception ex)
            {
                throw new ApplicationException($"An error occurred while retrieving scenic spots for province ID {provinceId}.", ex);
            }
        }

        // ============ ADMIN ENDPOINTS (All spots) ============
        
        public async Task<IEnumerable<ScenicSpotDto>> GetAllScenicSpotsAdminAsync()
        {
            try
            {
                var scenicSpots = await _scenicSpotRepo.GetAllScenicSpotsAsync();
                
                if (!scenicSpots.Any())
                {
                    throw new KeyNotFoundException("No scenic spots found.");
                }
                
                // Return all spots (including IsViewed = false)
                return scenicSpots.Select(spot => new ScenicSpotDto
                {
                    Id = spot.ScenicSpotId,
                    ScenicSpotName = spot.ScenicSpotName,
                    Description = spot.Description,
                    EstimatedVisitPrice = spot.EstimatedVisitPrice,
                    MediaUrls = spot.Medias.Select(media => media.MediaUrl).ToList(),
                    ProvinceId = spot.ProvinceId,
                    IsViewed = spot.IsViewed
                }).ToList();
            }
            catch (Exception ex)
            {
                throw new ApplicationException("An error occurred while retrieving all scenic spots for admin.", ex);
            }
        }

        public async Task AddScenicSpotAsync(ScenicSpotRequestDto scenicSpotCreateDto)
        {
            if (scenicSpotCreateDto == null)
            {
                throw new ArgumentNullException(nameof(scenicSpotCreateDto), "Scenic spot data cannot be null.");
            }
            
            if (string.IsNullOrEmpty(scenicSpotCreateDto.ScenicSpotName))
            {
                throw new ArgumentException("Scenic spot name cannot be null or empty.", nameof(scenicSpotCreateDto.ScenicSpotName));
            }
            
            if (scenicSpotCreateDto.EstimatedVisitPrice < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(scenicSpotCreateDto.EstimatedVisitPrice), "Estimated visit price cannot be negative.");
            }
            
            try
            {
                var scenicSpot = new ScenicSpot
                {
                    ScenicSpotName = scenicSpotCreateDto.ScenicSpotName,
                    Description = scenicSpotCreateDto.Description,
                    EstimatedVisitPrice = scenicSpotCreateDto.EstimatedVisitPrice,
                    ProvinceId = scenicSpotCreateDto.ProvinceId,
                    IsViewed = false, // Mặc định là false (chưa được duyệt)
                    Medias = new List<Media>()
                };
                
                if (scenicSpotCreateDto.Medias != null && scenicSpotCreateDto.Medias.Any())
                {
                    if (scenicSpotCreateDto.Medias.Sum(f => f.Length) > _maxFileSize)
                    {
                        throw new Exception("Total media file size exceeds the maximum limit of 250 MB.");
                    }
                    
                    foreach (var mediaFile in scenicSpotCreateDto.Medias)
                    {
                        var mediaUrl = await _fileService.UploadFileAsync(mediaFile, scenicSpot.ScenicSpotId);
                        scenicSpot.Medias.Add(new Media { MediaUrl = mediaUrl });
                    }
                }
                
                await _scenicSpotRepo.AddScenicSpotAsync(scenicSpot);
            }
            catch (Exception ex)
            {
                // Cleanup uploaded files if error
                if (scenicSpotCreateDto.Medias != null)
                {
                    foreach (var mediaFile in scenicSpotCreateDto.Medias)
                    {
                        try
                        {
                            await _fileService.DeleteFileAsync(mediaFile.FileName);
                        }
                        catch (Exception deleteEx)
                        {
                            Debug.WriteLine($"Failed to delete file {mediaFile.FileName}: {deleteEx.Message}");
                        }
                    }
                }
                throw;
            }
        }

        public async Task UpdateScenicSpotAsync(int id, ScenicSpotRequestDto scenicSpotUpdateDto)
        {
            try
            {
                var existingSpot = await _scenicSpotRepo.GetScenicSpotByIdAsync(id);
                
                if (existingSpot == null)
                {
                    throw new KeyNotFoundException($"Scenic spot with ID {id} not found.");
                }

                // Update properties
                existingSpot.ScenicSpotName = scenicSpotUpdateDto.ScenicSpotName;
                existingSpot.Description = scenicSpotUpdateDto.Description;
                existingSpot.EstimatedVisitPrice = scenicSpotUpdateDto.EstimatedVisitPrice;
                existingSpot.ProvinceId = scenicSpotUpdateDto.ProvinceId;

                await _scenicSpotRepo.UpdateSceneicSpotAsync(existingSpot);
            }
            catch (Exception ex)
            {
                throw new ApplicationException($"An error occurred while updating scenic spot with ID {id}.", ex);
            }
        }

        public async Task ToggleScenicSpotStatusAsync(int id)
        {
            try
            {
                var spot = await _scenicSpotRepo.GetScenicSpotByIdAsync(id);
                
                if (spot == null)
                {
                    throw new KeyNotFoundException($"Scenic spot with ID {id} not found.");
                }
                
                // Toggle status
                spot.IsViewed = !spot.IsViewed;
                
                await _scenicSpotRepo.UpdateSceneicSpotAsync(spot);
            }
            catch (Exception ex)
            {
                throw new ApplicationException($"An error occurred while toggling status for scenic spot with ID {id}.", ex);
            }
        }

        public async Task DeleteScenicSpotAsync(int id)
        {
            try
            {
                var spot = await _scenicSpotRepo.GetScenicSpotByIdAsync(id);
                
                if (spot == null)
                {
                    throw new KeyNotFoundException($"Scenic spot with ID {id} not found.");
                }
                
                // Delete media files first
                if (spot.Medias != null && spot.Medias.Any())
                {
                    foreach (var media in spot.Medias)
                    {
                        try
                        {
                            var fileName = Path.GetFileName(media.MediaUrl);
                            await _fileService.DeleteFileAsync(fileName);
                        }
                        catch (Exception ex)
                        {
                            Debug.WriteLine($"Failed to delete media file: {ex.Message}");
                        }
                    }
                }
                
                await _scenicSpotRepo.DeleteScenicSpotAsync(id);
            }
            catch (Exception ex)
            {
                throw new ApplicationException($"An error occurred while deleting scenic spot with ID {id}.", ex);
            }
        }
    }
}
