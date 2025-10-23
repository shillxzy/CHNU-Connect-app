using CHNU_Connect.BLL.DTOs.Auth;
using CHNU_Connect.BLL.Services.Interfaces;
using CHNU_Connect.DAL.Entities;
using CHNU_Connect.DAL.Repositories.Interfaces;
using Microsoft.Extensions.Configuration;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using Microsoft.IdentityModel.Tokens;
using System.Text;

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
            if (user == null || user.PasswordHash != loginRequestDto.Password) // TODO: Hash passwords in production
            {
                throw new UnauthorizedAccessException("Invalid credentials");
            }

            var token = GenerateJwtToken(user);
            var refreshToken = GenerateRefreshToken();

            return new LoginResponseDto
            {
                Token = token,
                RefreshToken = refreshToken,
                UserId = user.Id.ToString(),
                Email = user.Email,
                Role = user.Role
            };
        }

        public async Task<bool> RegisterAsync(string username, string email, string password)
        {
            // User story: only @chnu.edu.ua allowed
            if (!email.EndsWith("@chnu.edu.ua"))
                return false;

            var existingUser = await _userRepository.GetByEmailAsync(email);
            if (existingUser != null)
                return false; // User already exists

            var user = new User
            {
                Email = email,
                PasswordHash = password, // TODO: Hash passwords in production
                Role = "student",
                FullName = username,
                CreatedAt = DateTime.UtcNow
            };

            await _userRepository.InsertAsync(user);
            await _userRepository.SaveAsync();

            // TODO: send confirmation email

            return true;
        }

        public async Task<bool> UpdateProfileAsync(Guid userId, string fullName, string faculty, int? course, byte[]? photo)
        {
            var user = await _userRepository.GetByIdAsync(userId);
            if (user == null) return false;

            user.FullName = fullName ?? user.FullName;
            user.Faculty = faculty ?? user.Faculty;
            user.Course = course ?? user.Course;

            _userRepository.Update(user);
            await _userRepository.SaveAsync();
            return true;
        }

        public async Task<RefreshTokenResponseDto> RefreshTokenAsync(string currentRefreshToken)
        {
            // TODO: implement proper refresh token validation & storage
            return new RefreshTokenResponseDto
            {
                Token = "new-dummy-jwt-token",
                RefreshToken = GenerateRefreshToken()
            };
        }

        private string GenerateJwtToken(User user)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_configuration["JwtConfig:Key"]);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                    new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                    new Claim(ClaimTypes.Name, user.FullName ?? user.Email),
                    new Claim(ClaimTypes.Role, user.Role)
                }),
                Expires = DateTime.UtcNow.AddMinutes(int.Parse(_configuration["JwtConfig:TokenValidityMins"])),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }

        private string GenerateRefreshToken()
        {
            return Convert.ToBase64String(RandomNumberGenerator.GetBytes(64));
        }

        public async Task<bool> ConfirmEmailAsync(string userId, string token)
        {
            // TODO: Реальна логіка підтвердження email
            return true;
        }

        public async Task<bool> ForgotPasswordAsync(string email)
        {
            // TODO: Надіслати email для відновлення пароля
            var user = await _userRepository.GetByEmailAsync(email);
            return user != null;
        }

        public async Task<bool> ResetPasswordAsync(string email, string token, string newPassword)
        {
            // TODO: Перевірити токен та зберегти новий пароль
            var user = await _userRepository.GetByEmailAsync(email);
            if (user == null) return false;

            user.PasswordHash = newPassword;
            _userRepository.Update(user);
            await _userRepository.SaveAsync();
            return true;
        }

        public async Task LogoutAsync()
        {
            // TODO: Інактивація токенів, якщо є логіка
            await Task.CompletedTask;
        }

    }
}
