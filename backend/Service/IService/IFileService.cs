namespace GoWhere_Api.Service.IService
{
    public interface IFileService
    {
        Task<string> UploadFileAsync(IFormFile file, int scenicSpotId);
        Task<string> UploadReceiptAsync(IFormFile file, int orderId);
        Task DeleteFileAsync(string fileUrl);
    }
}
