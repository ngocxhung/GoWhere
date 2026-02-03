using GoWhere_Api.Repo.IRepo;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GoWhere_Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Admin")]
    public class AdminUserController : ControllerBase
    {
        private readonly IUserRepo _userRepo;

        public AdminUserController(IUserRepo userRepo)
        {
            _userRepo = userRepo;
        }

        /// <summary>
        /// [ADMIN] Lấy tất cả users
        /// </summary>
        [HttpGet("GetAllUsers")]
        public async Task<IActionResult> GetAllUsers()
        {
            try
            {
                var users = await _userRepo.GetAllUsersAsync();
                
                // Không trả về password
                var usersDto = users.Select(u => new
                {
                    u.UserId,
                    u.Username,
                    u.Email,
                    u.FullName,
                    u.PhoneNumber,
                    u.RoleId,
                    RoleName = u.Role?.RoleName
                });
                
                return Ok(usersDto);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = "Error retrieving users", Details = ex.Message });
            }
        }

        /// <summary>
        /// [ADMIN] Lấy user theo ID
        /// </summary>
        [HttpGet("GetUserById/{id}")]
        public async Task<IActionResult> GetUserById(int id)
        {
            try
            {
                var user = await _userRepo.GetUserByIdAsync(id);
                
                if (user == null)
                {
                    return NotFound($"User with ID {id} not found.");
                }

                var userDto = new
                {
                    user.UserId,
                    user.Username,
                    user.Email,
                    user.FullName,
                    user.PhoneNumber,
                    user.RoleId,
                    RoleName = user.Role?.RoleName
                };
                
                return Ok(userDto);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = "Error retrieving user", Details = ex.Message });
            }
        }

        /// <summary>
        /// [ADMIN] Cập nhật thông tin user
        /// </summary>
        [HttpPut("UpdateUser/{id}")]
        public async Task<IActionResult> UpdateUser(int id, [FromBody] UpdateUserDto updateDto)
        {
            try
            {
                var user = await _userRepo.GetUserByIdAsync(id);
                
                if (user == null)
                {
                    return NotFound($"User with ID {id} not found.");
                }

                // Update fields
                if (!string.IsNullOrEmpty(updateDto.Email))
                    user.Email = updateDto.Email;
                    
                if (!string.IsNullOrEmpty(updateDto.FullName))
                    user.FullName = updateDto.FullName;
                    
                if (!string.IsNullOrEmpty(updateDto.PhoneNumber))
                    user.PhoneNumber = updateDto.PhoneNumber;
                    
                if (updateDto.RoleId.HasValue)
                    user.RoleId = updateDto.RoleId.Value;

                await _userRepo.UpdateUser(user);
                
                return Ok(new { Message = "User updated successfully." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = "Error updating user", Details = ex.Message });
            }
        }

        /// <summary>
        /// [ADMIN] Đổi role của user (User <-> Admin)
        /// </summary>
        [HttpPatch("ChangeRole/{id}")]
        public async Task<IActionResult> ChangeUserRole(int id, [FromBody] ChangeRoleDto dto)
        {
            try
            {
                var user = await _userRepo.GetUserByIdAsync(id);
                
                if (user == null)
                {
                    return NotFound($"User with ID {id} not found.");
                }

                user.RoleId = dto.RoleId;
                await _userRepo.UpdateUser(user);
                
                return Ok(new { Message = $"User role changed successfully to RoleId: {dto.RoleId}" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = "Error changing role", Details = ex.Message });
            }
        }
    }

    // DTOs
    public class UpdateUserDto
    {
        public string? Email { get; set; }
        public string? FullName { get; set; }
        public string? PhoneNumber { get; set; }
        public int? RoleId { get; set; }
    }

    public class ChangeRoleDto
    {
        public int RoleId { get; set; }
    }
}
