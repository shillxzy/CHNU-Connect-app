using CHNU_Connect.BLL.DTOs.Auth;
using CHNU_Connect.BLL.Services.Interfaces;
using CHNU_Connect.DAL.Repositories.Interfaces;
using CHNU_Connect.API.Logging;
using Microsoft.Extensions.Configuration;
using Mapster;

namespace CHNU_Connect.BLL.Services
{
    public class AuthService : IAuthService
    {
        private readonly IUserRepository _userRepository;
        private readonly IConfiguration _configuration;

        public AuthService(IUserRepository userRepository, IConfiguration configuration)
        {
            _userRepository = userRepository;
            _configuration = configuration;
        }

        public async Task<LoginResponseDto> Login(LoginRequestDto loginRequestDto)
        {
            var user = await _userRepository.GetByEmailAsync(loginRequestDto.Email);
            if (user == null || user.PasswordHash != loginRequestDto.Password) // In real app, use proper password hashing
            {
                throw new UnauthorizedAccessException("Invalid credentials");
            }

            var token = JwtConfig.GenerateJwtToken(user.Id.ToString(), user.Email, user.Role, _configuration);
            
            return new LoginResponseDto
            {
                Token = token,
                RefreshToken = "dummy-refresh-token", // In real app, generate proper refresh token
                UserId = user.Id,
                Email = user.Email,
                Role = user.Role
            };
        }

        public async Task<bool> RegisterAsync(string username, string email, string password)
        {
            var existingUser = await _userRepository.GetByEmailAsync(email);
            if (existingUser != null)
            {
                return false; // User already exists
            }

            var user = new CHNU_Connect.DAL.Entities.User
            {
                Email = email,
                PasswordHash = password, // In real app, hash the password
                Role = "student", // Default role
                FullName = username,
                CreatedAt = DateTime.UtcNow
            };

            await _userRepository.AddAsync(user);
            await _userRepository.SaveChangesAsync();
            return true;
        }

        public async Task<RefreshTokenResponseDto> RefreshTokenAsync(string ipAddress)
        {
            // In real app, implement proper refresh token logic
            return new RefreshTokenResponseDto
            {
                Token = "new-dummy-jwt-token",
                RefreshToken = "new-dummy-refresh-token"
            };
        }

        public async Task<bool> ConfirmEmailAsync(string userId, string token)
        {
            // In real app, implement email confirmation logic
            return true;
        }

        public async Task<bool> ForgotPasswordAsync(string email)
        {
            var user = await _userRepository.GetByEmailAsync(email);
            if (user == null)
                return false;

            // In real app, send password reset email
            return true;
        }

        public async Task<bool> ResetPasswordAsync(string email, string token, string newPassword)
        {
            var user = await _userRepository.GetByEmailAsync(email);
            if (user == null)
                return false;

            // In real app, validate token and hash new password
            user.PasswordHash = newPassword;
            await _userRepository.UpdateAsync(user);
            await _userRepository.SaveChangesAsync();
            return true;
        }

        public async Task LogoutAsync()
        {
            // In real app, invalidate tokens
            await Task.CompletedTask;
        }
    }
}
