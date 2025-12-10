using CHNU_Connect.BLL.DTOs.Auth;
using CHNU_Connect.BLL.Services.Interfaces;
using CHNU_Connect.DAL.Entities;
using CHNU_Connect.DAL.Repositories.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace CHNU_Connect.BLL.Services
{
    public class AuthService : IAuthService
    {
        private readonly IUserRepository _userRepository;
        private readonly IConfiguration _configuration;
        private readonly ILogger<AuthService> _logger;

        public AuthService(IUserRepository userRepository, IConfiguration configuration, ILogger<AuthService> logger)
        {
            _userRepository = userRepository;
            _configuration = configuration;
            _logger = logger;
        }

        public async Task<LoginResponseDto> Login(LoginRequestDto loginRequestDto)
        {
            var user = await _userRepository.GetByEmailAsync(loginRequestDto.Email);

            // --- Вставляєш сюди блок з логами ---
            if (user == null)
            {
                _logger.LogWarning("Login failed: user with email {Email} not found", loginRequestDto.Email);
                throw new UnauthorizedAccessException("Invalid credentials");
            }

            _logger.LogInformation("User found: {Email}", user.Email);
            _logger.LogInformation("Stored password: {PasswordHash}", user.PasswordHash);
            _logger.LogInformation("Password from request: {RequestPassword}", loginRequestDto.Password);

            if (user.PasswordHash != loginRequestDto.Password)
            {
                _logger.LogWarning("Login failed: invalid password for {Email}", loginRequestDto.Email);
                throw new UnauthorizedAccessException("Invalid credentials");
            }
            // --- Кінець блока ---

            //  if (!user.IsEmailConfirmed)
            //  throw new UnauthorizedAccessException("Підтвердіть email перед входом.");

            _logger.LogInformation("Generating JWT for user {Email}", user.Email);
            var token = GenerateJwtToken(user);
            var refreshToken = GenerateRefreshToken();

            return new LoginResponseDto
            {
                UserId = user.Id,
                UserName = user.FullName ?? user.Email,
                Email = user.Email,
                Role = user.Role,
                AccessToken = token,
                RefreshToken = refreshToken,
                Token = token
            };
        }


        public async Task<bool> RegisterAsync(string username, string email, string password)
        {
            if (string.IsNullOrWhiteSpace(email) ||
                string.IsNullOrWhiteSpace(password) ||
                string.IsNullOrWhiteSpace(username))
                return false;

            if (!email.EndsWith("@chnu.edu.ua"))
                return false;

            var existingUser = await _userRepository.GetByEmailAsync(email);
            if (existingUser != null)
                return false; 

            var user = new User
            {
                Email = email,
                PasswordHash = password,
                FullName = username,
                Role = "student",
                CreatedAt = DateTime.UtcNow,
                IsEmailConfirmed = false,
                EmailConfirmationToken = Guid.NewGuid().ToString()
            };

            await _userRepository.InsertAsync(user);
            await _userRepository.SaveAsync();

            Console.WriteLine($"[EMAIL MOCK] Confirm your email: https://chnu-connect/confirm-email?userId={user.Id}&token={user.EmailConfirmationToken}");

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
                Issuer = _configuration["JwtConfig:Issuer"],     
                Audience = _configuration["JwtConfig:Audience"],
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
            if (!Guid.TryParse(userId, out var guid))
                return false;

            var user = await _userRepository.GetByIdAsync(guid);
            if (user == null)
                return false;

            if (user.EmailConfirmationToken != token)
                return false;

            user.IsEmailConfirmed = true;
            user.EmailConfirmationToken = null; 

            _userRepository.Update(user);
            await _userRepository.SaveAsync();

            return true;
        }

        public async Task<bool> ForgotPasswordAsync(string email)
        {
            var user = await _userRepository.GetByEmailAsync(email);
            if (user == null)
                return false; 

            user.PasswordResetToken = Guid.NewGuid().ToString();
            user.PasswordResetTokenExpiry = DateTime.UtcNow.AddHours(1); 

            _userRepository.Update(user);
            await _userRepository.SaveAsync();

            Console.WriteLine($"[EMAIL MOCK] Password reset link: https://chnu-connect/reset-password?email={email}&token={user.PasswordResetToken}");

            return true;
        }

        public async Task<bool> ResetPasswordAsync(string email, string token, string newPassword)
        {
            var user = await _userRepository.GetByEmailAsync(email);
            if (user == null)
                return false;

            if (user.PasswordResetToken == null || user.PasswordResetToken != token)
                return false;

            if (user.PasswordResetTokenExpiry == null || user.PasswordResetTokenExpiry < DateTime.UtcNow)
                return false;

            if (string.IsNullOrWhiteSpace(newPassword))
                return false;

            user.PasswordHash = newPassword;
            user.PasswordResetToken = null;
            user.PasswordResetTokenExpiry = null;

            _userRepository.Update(user);
            await _userRepository.SaveAsync();

            return true;
        }


        public async Task LogoutAsync()
        {
            await Task.CompletedTask;
        }

    }
}
