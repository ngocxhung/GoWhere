namespace GoWhere_Api.Service.IService
{
    public interface ITokenService
    {
        Task<string> GenerateTokenAsync(int userId, string username, string role);

    }
}