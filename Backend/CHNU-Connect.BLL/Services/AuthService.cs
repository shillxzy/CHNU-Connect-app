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
using Microsoft.AspNetCore.Identity;
using CHNU_Connect.BLL.Settings;
using System.Net;
using System.Net.Mail;

namespace CHNU_Connect.BLL.Services
{
    public class AuthService : IAuthService
    {
        private readonly IUserRepository _userRepository;
        private readonly IConfiguration _configuration;
        private readonly ILogger<AuthService> _logger;
        private readonly PasswordHasher<User> _passwordHasher = new PasswordHasher<User>();
        private readonly EmailSettings _emailSettings;

        public AuthService(IUserRepository userRepository, IConfiguration configuration, ILogger<AuthService> logger,
    EmailSettings emailSettings)
        {
            _userRepository = userRepository;
            _configuration = configuration;
            _logger = logger;
            _emailSettings = emailSettings;
        }

        public async Task<LoginResponseDto> Login(LoginRequestDto loginRequestDto)
        {
            var user = await _userRepository.GetByEmailAsync(loginRequestDto.Email);

            if (user == null)
            {
                _logger.LogWarning("Login failed: user with email {Email} not found", loginRequestDto.Email);
                throw new UnauthorizedAccessException("Invalid credentials");
            }

            _logger.LogInformation("User found: {Email}", user.Email);
            _logger.LogInformation("Stored password: {PasswordHash}", user.PasswordHash);
            _logger.LogInformation("Password from request: {RequestPassword}", loginRequestDto.Password);

            var verificationResult = _passwordHasher.VerifyHashedPassword(user, user.PasswordHash, loginRequestDto.Password);
            if (verificationResult == PasswordVerificationResult.Failed)
                throw new UnauthorizedAccessException("Invalid credentials");

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
                Role = user.Role.ToString(),
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
                PasswordHash = _passwordHasher.HashPassword(null, password),
                FullName = username,
                Role = UserRole.student,
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
                AccessToken = "new-dummy-jwt-token",
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
                    new Claim(ClaimTypes.Role, user.Role.ToString())
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

            var random = new Random();
            var code = random.Next(100000, 999999).ToString();

            user.PasswordResetToken = code;
            user.PasswordResetTokenExpiry = DateTime.UtcNow.AddHours(1);

            _userRepository.Update(user);
            await _userRepository.SaveAsync();

            var body = $@"
<html>
<head>
    <style>
        .container {{
            font-family: Arial, sans-serif;
            max-width: 600px;
            margin: auto;
            padding: 20px;
            border: 1px solid #ddd;
            border-radius: 8px;
        }}
        .code {{
            font-size: 24px;
            font-weight: bold;
            color: #0078D7;
        }}
        .footer {{
            margin-top: 20px;
            font-size: 12px;
            color: #666;
        }}
    </style>
</head>
<body>
    <div class='container'>
        <h2>CHNU-Connect</h2>
        <p>Ви запросили скидання паролю.</p>
        <p>Ваш код для відновлення паролю:</p>
        <p class='code'>{code}</p>
        <p>Якщо ви не робили запит, ігноруйте цей лист.</p>
        <div class='footer'>
            &copy; {DateTime.UtcNow.Year} CHNU-Connect. Всі права захищені.
        </div>
    </div>
</body>
</html>";

            await SendEmailAsync(email, "Скидання паролю CHNU-Connect", body);

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

            user.PasswordHash = _passwordHasher.HashPassword(user, newPassword);
            user.PasswordResetToken = null;
            user.PasswordResetTokenExpiry = null;

            _userRepository.Update(user);
            await _userRepository.SaveAsync();

            return true;
        }

        private async Task SendEmailAsync(string toEmail, string subject, string body)
        {
            using var client = new SmtpClient(_emailSettings.SmtpHost, _emailSettings.SmtpPort)
            {
                Credentials = new NetworkCredential(_emailSettings.SenderEmail, _emailSettings.SenderPassword),
                EnableSsl = true
            };

            var mailMessage = new MailMessage
            {
                From = new MailAddress(_emailSettings.SenderEmail, _emailSettings.SenderName),
                Subject = subject,
                Body = body,
                IsBodyHtml = true
            };

            mailMessage.To.Add(toEmail);
            await client.SendMailAsync(mailMessage);
        }






        public async Task LogoutAsync()
        {
            await Task.CompletedTask;
        }

    }
}
