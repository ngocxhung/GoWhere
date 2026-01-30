using GoWhere_Api.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.ComponentModel.DataAnnotations;
using System.Security.Claims;

namespace GoWhere_Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly ApplicationDbContext _context;
        public AuthController(IConfiguration configuration, ApplicationDbContext context)
        {
            _configuration = configuration;
            _context = context;
        }
        [HttpPost("Register")]
        public async Task<IActionResult> Register([FromForm] RegisterModel register)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest("Invalid user data.");
            }
            var existingUser = await _context.Users
                .FirstOrDefaultAsync(u => u.Username == register.Username);
            if (existingUser != null)
            {
                return Conflict("Username already exists.");
            }
            var user = new User
            {
                Username = register.Username,
                Password = BCrypt.Net.BCrypt.HashPassword(register.Password),
                Email = register.Email,
                FullName = register.FullName,
                PhoneNumber = register.PhoneNumber,
                RoleId = 1 // Default role ID
            };
            _context.Users.Add(user);
            await _context.SaveChangesAsync();
            return Ok("User registered successfully.");
        }
        [HttpPost("Login")]
        public async Task<IActionResult> Login([FromForm] LoginModel login)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest("Invalid login data.");
            }
            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.Username == login.Username);
            if (user == null || !BCrypt.Net.BCrypt.Verify(login.Password, user.Password))
            {
                return Unauthorized("Invalid username or password.");
            }
            // Generate JWT token
            var accessKey = _configuration["Jwt:Key"];
            var claim = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, login.Username),
                new Claim(ClaimTypes.Name, user.Username),
                new Claim(ClaimTypes.Role, user.RoleId.ToString())
            };
            var key = new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(accessKey));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var token = new System.IdentityModel.Tokens.Jwt.JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                claims: claim,
                expires: DateTime.Now.AddHours(2),
                signingCredentials: creds
            );
            return Ok(new
            {
                token = new System.IdentityModel.Tokens.Jwt.JwtSecurityTokenHandler().WriteToken(token),
                message = "Login successfully."
            });

        }
        [HttpPost("LoginDemo")]
        [Obsolete("This endpoint is for demo purposes only and should not be used in production.")]
        public async Task<IActionResult> LoginDemo()
        {
            var demoUser = await _context.Users
                .FirstOrDefaultAsync(u => u.Username == "regularuser");
            if (demoUser == null)
            {
                return NotFound("Demo user not found.");
            }
            // Generate JWT token
            var accessKey = _configuration["Jwt:Key"];
            var claim = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, demoUser.Username),
                new Claim(ClaimTypes.Name, demoUser.Username),
                new Claim(ClaimTypes.Role, demoUser.RoleId.ToString())
            };
            var key = new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(accessKey));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var token = new System.IdentityModel.Tokens.Jwt.JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                claims: claim,
                expires: DateTime.Now.AddHours(2),
                signingCredentials: creds
            );
            return Ok(new
            {
                token = new System.IdentityModel.Tokens.Jwt.JwtSecurityTokenHandler().WriteToken(token),
                message = "Demo login successfully."
            });
        }

    }
    public class RegisterModel
    {
        [Required(ErrorMessage = "Username is required.")]
        public string Username { get; set; }
        [Required(ErrorMessage = "Password is required.")]
        public string Password { get; set; }
        [Required(ErrorMessage = "Email is required.")]
        public string Email { get; set; }
        [Required(ErrorMessage = "Full name is required.")]
        public string FullName { get; set; }
        [Required(ErrorMessage = "Phone number is required.")]
        public string PhoneNumber { get; set; }
        [Required(ErrorMessage = "Role ID is required.")]
        public int RoleId { get; set; }

    }
    public class LoginModel
    {
        [Required(ErrorMessage = "Username is required.")]
        public string Username { get; set; }
        [Required(ErrorMessage = "Password is required.")]
        public string Password { get; set; }
    }
}
