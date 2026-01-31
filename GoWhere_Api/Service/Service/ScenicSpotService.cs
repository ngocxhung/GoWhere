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

        public async Task AddScenicSpotAsync(ScenicSpotRequestDto scenicSpotCreateDto)
        {
            // Validate input 
            if (scenicSpotCreateDto == null)
            {
                throw new ArgumentNullException(nameof(scenicSpotCreateDto), "Scenic spot data cannot be null.");
            }
            else if (string.IsNullOrEmpty(scenicSpotCreateDto.ScenicSpotName))
            {
                throw new ArgumentException("Scenic spot name cannot be null or empty.", nameof(scenicSpotCreateDto.ScenicSpotName));
            }
            else if (scenicSpotCreateDto.EstimatedVisitPrice < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(scenicSpotCreateDto.EstimatedVisitPrice), "Estimated visit price cannot be negative.");
            }
            else
            {
                try
                {
                    var scenicSpot = new Model.ScenicSpot
                    {
                        ScenicSpotName = scenicSpotCreateDto.ScenicSpotName,
                        Description = scenicSpotCreateDto.Description,
                        EstimatedVisitPrice = scenicSpotCreateDto.EstimatedVisitPrice,
                        ProvinceId = scenicSpotCreateDto.ProvinceId,
                        Medias = new List<Model.Media>()
                    };
                    foreach (var mediaFile in scenicSpotCreateDto.Medias)
                    {
                        var mediaUrl = await _fileService.UploadFileAsync(mediaFile, scenicSpot.ScenicSpotId);
                        
                        scenicSpot.Medias.Add(new Model.Media { MediaUrl = mediaUrl });
                    }
                    if(scenicSpotCreateDto.Medias.Sum(f => f.Length) > _maxFileSize)    
                    {
                        throw new Exception("Total media file size exceeds the maximum limit of 250 MB.");
                    }
                   

                    if (scenicSpot.Medias.Count == 0)
                    {
                        throw new Exception("At least one media file is required.");
                    }
                    await _scenicSpotRepo.AddScenicSpotAsync(scenicSpot);

                }
                catch (Exception ex)
                {
                    foreach(var mediaFile in scenicSpotCreateDto.Medias)
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
            }

        }

        public async Task<IEnumerable<ScenicSpotDto>> GetAllScenicSpotsAsync()
        {
            try
            {
                var scenicSpots = await _scenicSpotRepo.GetAllScenicSpotsAsync();
                if (scenicSpots == null || !scenicSpots.Any())
                {
                    throw new KeyNotFoundException("No scenic spots found.");
                }
                return scenicSpots.Select(spot => new ScenicSpotDto
                {
                    ScenicSpotName = spot.ScenicSpotName,
                    Description = spot.Description,
                    EstimatedVisitPrice = spot.EstimatedVisitPrice,
                    MediaUrls = spot.Medias.Select(media => media.MediaUrl).ToList(),
                    ProvinceId = spot.ProvinceId
                }).ToList();
            }
            catch (Exception ex)
            {
                // Log the exception (not implemented here)
                throw new ApplicationException("An error occurred while retrieving scenic spots.", ex);
            }

        }

        public async Task<ScenicSpotDto> GetScenicSpotByIdAsync(int id)
        {
            try { 
                var spot = await _scenicSpotRepo.GetScenicSpotByIdAsync(id);
                if (spot == null)
                {
                    throw new KeyNotFoundException($"Scenic spot with ID {id} not found.");
                }
                return new ScenicSpotDto
                {
                    ScenicSpotName = spot.ScenicSpotName,
                    Description = spot.Description,
                    EstimatedVisitPrice = spot.EstimatedVisitPrice,
                    MediaUrls = spot.Medias.Select(media => media.MediaUrl).ToList(),
                    ProvinceId = spot.ProvinceId
                };
            }
            catch (Exception ex)
            {
                // Log the exception (not implemented here)
                throw new ApplicationException($"An error occurred while retrieving scenic spot with ID {id}.", ex);
            }
        }

        public async Task<IEnumerable<ScenicSpotDto>> GetScenicSpotsByProvinceIdAsync(int provinceId)
        {
            try
            {
                var scenicSpots = await _scenicSpotRepo.GetScenicSpotsByProvinceIdAsync(provinceId);
                if (scenicSpots == null || !scenicSpots.Any())
                {
                    throw new KeyNotFoundException($"No scenic spots found for province ID {provinceId}.");
                }
                return scenicSpots.Select(spot => new ScenicSpotDto
                {
                    ScenicSpotName = spot.ScenicSpotName,
                    Description = spot.Description,
                    EstimatedVisitPrice = spot.EstimatedVisitPrice,
                    MediaUrls = spot.Medias.Select(media => media.MediaUrl).ToList(),
                    ProvinceId = spot.ProvinceId
                }).ToList();
            }
            catch (Exception ex)
            {
                // Log the exception (not implemented here)
                throw new ApplicationException($"An error occurred while retrieving scenic spots for province ID {provinceId}.", ex);
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

                // Update the properties
                existingSpot.ScenicSpotName = scenicSpotUpdateDto.ScenicSpotName;
                existingSpot.Description = scenicSpotUpdateDto.Description;
                existingSpot.EstimatedVisitPrice = scenicSpotUpdateDto.EstimatedVisitPrice;
                
                existingSpot.ProvinceId = scenicSpotUpdateDto.ProvinceId;

                await _scenicSpotRepo.UpdateSceneicSpotAsync(existingSpot);
            }
            catch (Exception ex)
            {
                // Log the exception (not implemented here)
                throw new ApplicationException($"An error occurred while updating scenic spot with ID {id}.", ex);
            }
        }
    }
}
