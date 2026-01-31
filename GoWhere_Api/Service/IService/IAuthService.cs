using Azure.Core;
using GoWhere_Api.Model.DTOs.Request;
using GoWhere_Api.Model.DTOs.Response;

namespace GoWhere_Api.Service.IService
{
    public interface IAuthService
    {
        Task RegisterAsync(UserRegisterDto userRegisterDto);
        Task<LoginResponse> LoginAsync(Model.DTOs.Request.LoginDto loginDto);
    }
}