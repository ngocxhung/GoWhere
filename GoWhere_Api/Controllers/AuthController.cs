using GoWhere_Api.Model;
using GoWhere_Api.Model.DTOs.Request;
using GoWhere_Api.Model.DTOs.Response;
using GoWhere_Api.Service.IService;
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
        private readonly IAuthService _authService;
        public AuthController(IConfiguration configuration, ApplicationDbContext context, IAuthService authService)
        {
            _configuration = configuration;
            _context = context;
            _authService = authService;
        }
        [HttpGet("Register")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]

        public async Task<IActionResult> Register([FromForm] UserRegisterDto userRegisterDto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);

                }
                await _authService.RegisterAsync(userRegisterDto);
                return Ok(new { Message = "User registered successfully" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = "An error occurred during registration", Details = ex.Message });
            }
        }
        [HttpPost("Login")]
        public async Task<IActionResult> Login([FromForm] LoginDto loginDto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }
                var loginResponse = await _authService.LoginAsync(loginDto);
                return Ok(new LoginResponse { Token = loginResponse.Token });
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(new { Message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = "An error occurred during login", Details = ex.Message });
            }


        }
        [HttpPost("LoginDemo")]
        [Obsolete("This endpoint is for demo purposes only and should not be used in production.")]
        public async Task<IActionResult> LoginDemo([FromForm] LoginDto loginDto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }
                var loginResponse = await _authService.LoginDemoAsync(loginDto);
                return Ok(new LoginResponse { Token = loginResponse.Token });
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(new { Message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = "An error occurred during login", Details = ex.Message });
            }
        }
    }
}
