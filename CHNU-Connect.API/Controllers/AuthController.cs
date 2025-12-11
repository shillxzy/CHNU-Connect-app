using CHNU_Connect.API.Logging;
using CHNU_Connect.BLL.DTOs.Auth;
using CHNU_Connect.BLL.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace CHNU_Connect.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;
        private readonly IUserService _userService;
        private readonly IConfiguration _configuration;
        private readonly ILogger<AuthController> _logger;

        public AuthController(
            IAuthService authService, 
            IUserService userService, 
            IConfiguration configuration,
            ILogger<AuthController> logger)
        {
            _authService = authService;
            _userService = userService;
            _configuration = configuration;
            _logger = logger;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterRequestDto request)
        {
            try
            {
                if (!IsValidChnuEmail(request.Email))
                {
                    return BadRequest(new { message = "Only @chnu.edu.ua email addresses are allowed for registration." });
                }

                var existingUser = await _userService.GetByEmailAsync(request.Email);
                if (existingUser != null)
                {
                    return BadRequest(new { message = "User with this email already exists." });
                }

                var success = await _authService.RegisterAsync(request.Username, request.Email, request.Password);
                if (!success)
                {
                    return BadRequest(new { message = "Registration failed. Please try again." });
                }

                _logger.LogInformation("New user registered: {Email}", request.Email);
                return Ok(new { message = "Registration successful. Please check your email for confirmation." });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during registration for email: {Email}", request.Email);
                return StatusCode(500, new { message = "An error occurred during registration." });
            }
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequestDto request)
        {
            try
            {
                var loginResponse = await _authService.Login(request);
                
                _logger.LogInformation("User logged in: {Email}", request.Email);
                return Ok(loginResponse);
            }
            catch (UnauthorizedAccessException)
            {
                return Unauthorized(new { message = "Invalid email or password." });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during login for email: {Email}", request.Email);
                return StatusCode(500, new { message = "An error occurred during login." });
            }
        }


        [HttpPost("refresh-token")]
        public async Task<IActionResult> RefreshToken([FromBody] RefreshTokenRequestDto request)
        {
            try
            {
                var refreshResponse = await _authService.RefreshTokenAsync(request.IpAddress);
                return Ok(refreshResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during token refresh");
                return StatusCode(500, new { message = "An error occurred during token refresh." });
            }
        }

        [HttpPost("forgot-password")]
        public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordRequestDto request)
        {
            try
            {
                var success = await _authService.ForgotPasswordAsync(request.Email);
                if (success)
                {
                    return Ok(new { message = "Password reset instructions have been sent to your email." });
                }
                return BadRequest(new { message = "Email not found." });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during forgot password for email: {Email}", request.Email);
                return StatusCode(500, new { message = "An error occurred." });
            }
        }

        [HttpPost("reset-password")]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordRequestDto request)
        {
            try
            {
                var success = await _authService.ResetPasswordAsync(request.Email, request.Token, request.NewPassword);
                if (success)
                {
                    return Ok(new { message = "Password has been reset successfully." });
                }
                return BadRequest(new { message = "Invalid token or email." });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during password reset for email: {Email}", request.Email);
                return StatusCode(500, new { message = "An error occurred." });
            }
        }

        [HttpPost("logout")]
        [Microsoft.AspNetCore.Authorization.Authorize]
        public async Task<IActionResult> Logout()
        {
            try
            {
                await _authService.LogoutAsync();
                return Ok(new { message = "Logged out successfully." });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during logout");
                return StatusCode(500, new { message = "An error occurred during logout." });
            }
        }

        private bool IsValidChnuEmail(string email)
        {
            if (string.IsNullOrEmpty(email))
                return false;

            return email.EndsWith("@chnu.edu.ua", StringComparison.OrdinalIgnoreCase);
        }
    }
}
