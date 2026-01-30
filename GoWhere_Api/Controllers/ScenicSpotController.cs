using GoWhere_Api.Model;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.EntityFrameworkCore;

namespace GoWhere_Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ScenicSpotController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        public ScenicSpotController(ApplicationDbContext context)
        {
            _context = context;
        }
        [HttpGet("GetAllScenicSpots")]
        [EnableQuery]
        public async Task<IActionResult> GetScenicSpots()
        {
            var scenicSpots = await _context.ScenicSpots
                .Select(s => new
                {
                    Id = s.ScenicSpotId,
                    Name = s.ScenicSpotName,
                    Description = s.Description,
                    EstimatedVisitPrice = s.EstimatedVisitPrice,
                    Medias = s.Medias.Select(m => new
                    {
                        MediaId = m.MediaId,
                        MediaUrl = m.MediaUrl,
                    }).ToList(),
                    ProvinceId = s.ProvinceId
                }).ToListAsync();
            if (scenicSpots == null || scenicSpots.Count == 0)
            {
                return NotFound("No scenic spots found.");
            }
            return Ok(scenicSpots);
        }
        [HttpGet("GetScenicSpotById/{id}")]
        public async Task<IActionResult> GetScenicSpotById(int id)
        {
            var scenicSpot = await _context.ScenicSpots
                .Select(s => new
                {
                    Id = s.ScenicSpotId,
                    Name = s.ScenicSpotName,
                    Description = s.Description,
                    EstimatedVisitPrice = s.EstimatedVisitPrice,
                    Medias = s.Medias.Select(m => new
                    {
                        MediaId = m.MediaId,
                        MediaUrl = m.MediaUrl,
                    }).ToList(),
                    ProvinceId = s.ProvinceId
                }).FirstOrDefaultAsync(s => s.Id == id);
            if (scenicSpot == null)
            {
                return NotFound($"Scenic Spot with Id: {id} not found");
            }
            return Ok(scenicSpot);
        }
        [HttpGet("GetScenicSpotsByProvinceId/{provinceId}")]
        public async Task<IActionResult> GetScenicSpotsByProvinceId(int provinceId)
        {
            var scenicSpots = await _context.ScenicSpots
                .Where(s => s.ProvinceId == provinceId)
                .Select(s => new
                {
                    Id = s.ScenicSpotId,
                    Name = s.ScenicSpotName,
                    Description = s.Description,
                    EstimatedVisitPrice = s.EstimatedVisitPrice,
                    Medias = s.Medias.Select(m => new
                    {
                        MediaId = m.MediaId,
                        MediaUrl = m.MediaUrl,
                    }).ToList(),
                    ProvinceId = s.ProvinceId
                }).ToListAsync();
            if (scenicSpots == null || scenicSpots.Count == 0)
            {
                return NotFound($"No scenic spots found for Province Id: {provinceId}");
            }
            return Ok(scenicSpots);
        }
        [HttpPut("AddScenicSpot")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> AddScenicSpot([FromForm] ScenicSpotDto scenicSpotCreateDto)
        {
            if (scenicSpotCreateDto == null)
                return BadRequest("Request body is required.");
            if (string.IsNullOrWhiteSpace(scenicSpotCreateDto.ScenicSpotName))
                return BadRequest("ScenicSpotName is required.");
            var existingScenicSpot = await _context.ScenicSpots.FindAsync(scenicSpotCreateDto.ScenicSpotName);
            if (existingScenicSpot != null)
                return BadRequest($"There is already scenic spot name: {scenicSpotCreateDto.ScenicSpotName}");
            

            // basic validations
            const long maxTotalUploadBytes = 50L * 2L * 1024 * 1024; // 100 MB total per request
            long totalBytes = scenicSpotCreateDto.Medias?.Sum(f => f?.Length ?? 0) ?? 0;
            if (totalBytes > maxTotalUploadBytes)
                return BadRequest("Total upload size exceeds limit (50 MB). Please upload smaller files.");

            var savedFilePaths = new List<string>();
            Microsoft.EntityFrameworkCore.Storage.IDbContextTransaction transaction = null;
            try
            {
                transaction = await _context.Database.BeginTransactionAsync();

                var scenic = new ScenicSpot
                {
                    ScenicSpotName = scenicSpotCreateDto.ScenicSpotName,
                    Description = scenicSpotCreateDto.Description,
                    EstimatedVisitPrice = scenicSpotCreateDto.EstimatedVisitPrice,
                    ProvinceId = scenicSpotCreateDto.ProvinceId
                };

                _context.ScenicSpots.Add(scenic);
                await _context.SaveChangesAsync(); // get ScenicSpotId

                // prepare folder to save files: wwwroot/uploads/scenicspots/{id}
                var baseFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads", "scenicspots", scenic.ScenicSpotId.ToString());
                if (!Directory.Exists(baseFolder))
                    Directory.CreateDirectory(baseFolder);

                var mediaEntities = new List<Media>();

                var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif", ".bmp", ".mp4", ".mov", ".avi", ".mkv", ".webm" };

                if (scenicSpotCreateDto.Medias != null)
                {
                    foreach (var file in scenicSpotCreateDto.Medias)
                    {
                        if (file == null || file.Length == 0)
                            continue;

                        // validate filename
                        var originalFileName = Path.GetFileName(file.FileName);
                        if (string.IsNullOrWhiteSpace(originalFileName))
                            throw new InvalidOperationException("Invalid file name.");
                        if (originalFileName.Contains("..") || originalFileName.IndexOfAny(Path.GetInvalidFileNameChars()) >= 0)
                            throw new InvalidOperationException("Suspicious file name detected.");

                        var ext = Path.GetExtension(originalFileName).ToLowerInvariant();
                        if (!allowedExtensions.Contains(ext))
                            return BadRequest($"File type not allowed: {originalFileName}");

                        // validate content type
                        if (!(file.ContentType.StartsWith("image/") || file.ContentType.StartsWith("video/")))
                            return BadRequest($"Content type not allowed: {file.ContentType}");

                        // check disk space on target drive
                        var root = Path.GetPathRoot(baseFolder);
                        if (string.IsNullOrEmpty(root))
                            throw new InvalidOperationException("Unable to determine target drive for uploads.");
                        var drive = new DriveInfo(root);
                        if (drive.AvailableFreeSpace < file.Length)
                            throw new IOException("Insufficient disk space to save uploaded files.");

                        // create unique filename and save
                        var uniqueName = $"{Guid.NewGuid()}{ext}";
                        var filePath = Path.Combine(baseFolder, uniqueName);

                        using (var stream = System.IO.File.Create(filePath))
                        {
                            await file.CopyToAsync(stream);
                        }

                        savedFilePaths.Add(filePath);

                        // store relative url
                        var mediaUrl = $"/uploads/scenicspots/{scenic.ScenicSpotId}/{uniqueName}";
                        mediaEntities.Add(new Media { MediaUrl = mediaUrl, ScenicSpotId = scenic.ScenicSpotId });
                    }
                }

                if (mediaEntities.Count > 0)
                {
                    _context.Medias.AddRange(mediaEntities);
                    await _context.SaveChangesAsync();
                }

                await transaction.CommitAsync();

                return CreatedAtAction(nameof(GetScenicSpotById), new { id = scenic.ScenicSpotId }, new { Id = scenic.ScenicSpotId });
            }
            catch(UnauthorizedAccessException)
            {
                return Unauthorized();
            }
            catch (InvalidOperationException invEx)
            {
                // suspicious file name or invalid path
                // cleanup saved files
                foreach (var p in savedFilePaths)
                {
                    try { if (System.IO.File.Exists(p)) System.IO.File.Delete(p); } catch { }
                }
                if (transaction != null)
                    await transaction.RollbackAsync();
                return BadRequest(invEx.Message);
            }
            catch (IOException ioEx)
            {
                foreach (var p in savedFilePaths)
                {
                    try { if (System.IO.File.Exists(p)) System.IO.File.Delete(p); } catch { }
                }
                if (transaction != null)
                    await transaction.RollbackAsync();
                return StatusCode(507, ioEx.Message); // 507 Insufficient Storage
            }
            catch (Exception ex)
            {
                foreach (var p in savedFilePaths)
                {
                    try { if (System.IO.File.Exists(p)) System.IO.File.Delete(p); } catch { }
                }
                if (transaction != null)
                    await transaction.RollbackAsync();
                return StatusCode(500, "An error occurred while creating the scenic spot.");
            }
        }
    }
    public class ScenicSpotDto
    {
        public string ScenicSpotName { get; set; }
        public string Description { get; set; }
        public double EstimatedVisitPrice { get; set; }
        public ICollection<IFormFile> Medias { get; set; }
        public int ProvinceId { get; set; }
    }
}
