namespace GoWhere_Api.Service.IService
{
    public interface IFileService
    {
        Task<string> UploadFileAsync(IFormFile file, int scenicSpotId);
        Task DeleteFileAsync(string fileUrl);
    }
}
