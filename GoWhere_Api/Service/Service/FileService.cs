using GoWhere_Api.Service.IService;

namespace GoWhere_Api.Service.Service
{
    public class FileService : IFileService
    {
        private readonly string[] _allowExtension = new[] { ".jpg", ".jpeg", ".png", ".gif", ".mp4", ".mov", ".avi", ".mkv" };
        public async Task DeleteFileAsync(string fileUrl)
        {
            try
            {
                if (string.IsNullOrEmpty(fileUrl))
                {
                    throw new ArgumentException("File URL is null or empty.");
                }
                var filePath = Path.Combine("wwwroot", fileUrl.TrimStart('/').Replace('/', Path.DirectorySeparatorChar));
                if (File.Exists(filePath))
                {
                    File.Delete(filePath);
                }
            }
            catch (Exception ex)
            {
                throw new ApplicationException("An error occurred while deleting the file.", ex);
            }
        }

        public async Task<string> UploadFileAsync(IFormFile file, int scenicSpotId)
        {
            try
            {
                // Validate image and video file
                if (file == null || file.Length == 0)
                {
                    throw new ArgumentException("File is null or empty.");
                }
                var fileExtension = Path.GetExtension(file.FileName).ToLower();
                if (!_allowExtension.Contains(fileExtension))
                {
                    throw new ArgumentException("Unsupported file type.");
                }
                // Check disk space 
                var drive = new DriveInfo(Path.GetPathRoot(Directory.GetCurrentDirectory()));
                if (drive.AvailableFreeSpace < file.Length)
                {
                    throw new IOException("Insufficient disk space to upload the file.");
                }

                var subFolder = $"scenicspots/{scenicSpotId}/";
                var uploadsFolder = Path.Combine("wwwroot", "uploads", subFolder);
                if (!Directory.Exists(uploadsFolder))
                {
                    Directory.CreateDirectory(uploadsFolder);
                }
                var uniqueFileName = $"{Guid.NewGuid()}{fileExtension}";
                var filePath = Path.Combine(uploadsFolder, uniqueFileName);
                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    await file.CopyToAsync(fileStream);
                }
                var fileUrl = $"/uploads/{subFolder}{uniqueFileName}";
                return fileUrl;

            }
            catch (Exception ex)
            {
                throw new ApplicationException("An error occurred while uploading the file.", ex);
            }

        }
    }
}
