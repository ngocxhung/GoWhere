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
                    IsViewed = spot.IsViewed,
                    ImageUrl = spot.ImageUrl
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
                    IsViewed = spot.IsViewed,
                    ImageUrl = spot.ImageUrl
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
                    IsViewed = spot.IsViewed,
                    ImageUrl = spot.ImageUrl
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
                    IsViewed = spot.IsViewed,
                    ImageUrl = spot.ImageUrl
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
            
            var uploadedMediaUrls = new List<string>();
            
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
                
                // ✅ FIX 1: INSERT TRƯỚC để có ScenicSpotId
                await _scenicSpotRepo.AddScenicSpotAsync(scenicSpot);
                // Giờ scenicSpot.ScenicSpotId đã có giá trị!
                
                // Upload files SAU KHI đã có ID
                if (scenicSpotCreateDto.Medias != null && scenicSpotCreateDto.Medias.Any())
                {
                    if (scenicSpotCreateDto.Medias.Sum(f => f.Length) > _maxFileSize)
                    {
                        throw new Exception("Total media file size exceeds the maximum limit of 250 MB.");
                    }
                    
                    foreach (var mediaFile in scenicSpotCreateDto.Medias)
                    {
                        // Upload với ID đúng
                        var mediaUrl = await _fileService.UploadFileAsync(mediaFile, scenicSpot.ScenicSpotId);
                        uploadedMediaUrls.Add(mediaUrl);
                        
                        scenicSpot.Medias.Add(new Media 
                        { 
                            MediaUrl = mediaUrl,
                            ScenicSpotId = scenicSpot.ScenicSpotId
                        });
                    }
                    
                    // Update lại với Media
                    await _scenicSpotRepo.UpdateSceneicSpotAsync(scenicSpot);
                }
            }
            catch (Exception ex)
            {
                // ✅ FIX 3: Cleanup với URL đã upload (ĐÚNG)
                if (uploadedMediaUrls.Any())
                {
                    foreach (var mediaUrl in uploadedMediaUrls)
                    {
                        try
                        {
                            var fileName = Path.GetFileName(mediaUrl);
                            await _fileService.DeleteFileAsync(fileName);
                        }
                        catch (Exception deleteEx)
                        {
                            var fileNameForLog = Path.GetFileName(mediaUrl);
                            Debug.WriteLine($"Failed to delete file {fileNameForLog}: {deleteEx.Message}");
                        }
                    }
                }
                throw;
            }
        }

        public async Task UpdateScenicSpotAsync(int id, ScenicSpotRequestDto scenicSpotUpdateDto)
        {
            var uploadedMediaUrls = new List<string>();
            
            try
            {
                var existingSpot = await _scenicSpotRepo.GetScenicSpotByIdAsync(id);
                
                if (existingSpot == null)
                {
                    throw new KeyNotFoundException($"Scenic spot with ID {id} not found.");
                }

                // Update text properties
                existingSpot.ScenicSpotName = scenicSpotUpdateDto.ScenicSpotName;
                existingSpot.Description = scenicSpotUpdateDto.Description;
                existingSpot.EstimatedVisitPrice = scenicSpotUpdateDto.EstimatedVisitPrice;
                existingSpot.ProvinceId = scenicSpotUpdateDto.ProvinceId;

                // ✅ FIX 2: XỬ LÝ MEDIA NẾU CÓ
                if (scenicSpotUpdateDto.Medias != null && scenicSpotUpdateDto.Medias.Any())
                {
                    if (scenicSpotUpdateDto.Medias.Sum(f => f.Length) > _maxFileSize)
                    {
                        throw new Exception("Total media file size exceeds the maximum limit of 250 MB.");
                    }
                    
                    // Option 1: XÓA ẢNH CŨ VÀ THÊM MỚI (Replace all)
                    // Delete old media files
                    if (existingSpot.Medias != null && existingSpot.Medias.Any())
                    {
                        foreach (var media in existingSpot.Medias.ToList())
                        {
                            try
                            {
                                var fileName = Path.GetFileName(media.MediaUrl);
                                await _fileService.DeleteFileAsync(fileName);
                            }
                            catch (Exception ex)
                            {
                                Debug.WriteLine($"Failed to delete old media file: {ex.Message}");
                            }
                        }
                        existingSpot.Medias.Clear();
                    }
                    
                    // Upload new media files
                    foreach (var mediaFile in scenicSpotUpdateDto.Medias)
                    {
                        var mediaUrl = await _fileService.UploadFileAsync(mediaFile, id);
                        uploadedMediaUrls.Add(mediaUrl);
                        
                        existingSpot.Medias.Add(new Media 
                        { 
                            MediaUrl = mediaUrl,
                            ScenicSpotId = id
                        });
                    }
                }

                await _scenicSpotRepo.UpdateSceneicSpotAsync(existingSpot);
            }
            catch (Exception ex)
            {
                // Cleanup newly uploaded files if error
                if (uploadedMediaUrls.Any())
                {
                    foreach (var mediaUrl in uploadedMediaUrls)
                    {
                        try
                        {
                            var fileName = Path.GetFileName(mediaUrl);
                            await _fileService.DeleteFileAsync(fileName);
                        }
                        catch (Exception deleteEx)
                        {
                            var fileNameForLog = Path.GetFileName(mediaUrl);
                            Debug.WriteLine($"Failed to cleanup uploaded file {fileNameForLog}: {deleteEx.Message}");
                        }
                    }
                }
                
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
