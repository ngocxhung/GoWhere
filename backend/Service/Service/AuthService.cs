using GoWhere_Api.Model;
using GoWhere_Api.Model.DTOs.Request;
using GoWhere_Api.Model.DTOs.Response;
using GoWhere_Api.Repo.IRepo;
using GoWhere_Api.Service.IService;

namespace GoWhere_Api.Service.Service
{
    public class AuthService : IAuthService
    {
        private readonly IUserRepo _userRepo;
        private readonly IService.ITokenService _tokenService;
        public AuthService(IUserRepo userRepo, IService.ITokenService tokenService)
        {
            _userRepo = userRepo;
            _tokenService = tokenService;
        }
        public async Task<LoginResponse> LoginAsync(LoginDto loginDto)
        {
            try
            {
                var user = await _userRepo.GetUserByUsernameAsync(loginDto.Username);
                if (user == null || !BCrypt.Net.BCrypt.Verify(loginDto.Password, user.Password))
                {
                    throw new UnauthorizedAccessException("Invalid username or password.");
                }
                var token = await _tokenService.GenerateTokenAsync(user.UserId, user.Username, user.Role.RoleName);
                return new LoginResponse 
                { 
                    Token = token,
                    FullName = user.FullName,
                    UserId = user.UserId,
                    Role = user.Role.RoleName  // Trả về RoleName ("Admin" hoặc "User") thay vì RoleId
                };

            }
            catch (Exception ex)
            {
                throw new Exception($"Login failed: {ex.Message}");
            }
        }
        [Obsolete("This method is for demo purposes only.")]
        public async Task<LoginResponse> LoginDemoAsync(LoginDto loginDto)
        {
            try
            {
                var user = await _userRepo.GetUserByUsernameAsync(loginDto.Username);
                if (user.Username.ToLower().Equals("regularuser") || user.Username.ToLower().Equals("adminuser"))
                {
                    var token = await _tokenService.GenerateTokenAsync(user.UserId, user.Username, user.Role.RoleName);
                    return new LoginResponse { Token = token };
                }
                else
                {
                    throw new UnauthorizedAccessException("Invalid demo user.");
                }

            }
            catch (Exception ex)
            {
                throw new Exception($"Login failed: {ex.Message}");
            }
        }

        public async Task RegisterAsync(UserRegisterDto userRegisterDto)
        {
            try
            {
                var isUserExist = await _userRepo.IsUserNameExistAsync(userRegisterDto.Username);
                if (isUserExist)
                {
                    throw new InvalidCastException("Username already exists.");
                }
                var user = new User
                {
                    Username = userRegisterDto.Username,
                    Password = BCrypt.Net.BCrypt.HashPassword(userRegisterDto.Password),
                    FullName = userRegisterDto.FullName,
                    Email = userRegisterDto.Email,
                    PhoneNumber = userRegisterDto.PhoneNumber,
                    RoleId = userRegisterDto.RoleId
                };
                await _userRepo.CreateUser(user);

            }
            catch (Exception ex)
            {
                throw new Exception($"Registration failed: {ex.Message}");
            }
        }
    }
}