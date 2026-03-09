using CHNU_Connect.BLL.DTOs.User;
using CHNU_Connect.BLL.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace CHNU_Connect.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly ILogger<UserController> _logger;

        public UserController(IUserService userService, ILogger<UserController> logger)
        {
            _userService = userService;
            _logger = logger;
        }

        [HttpGet("profile")]
        public async Task<IActionResult> GetProfile()
        {
            var userId = GetCurrentUserId();
            if (userId == null) return Unauthorized();

            var user = await _userService.GetByIdAsync(userId.Value);
            if (user == null) return NotFound(new { message = "User not found." });

            return Ok(user);
        }

        [HttpPut("profile")]
        public async Task<IActionResult> UpdateProfile([FromBody] UpdateProfileDto request)
        {
            var userId = GetCurrentUserId();
            if (userId == null) return Unauthorized();

            await _userService.UpdateProfileAsync(userId.Value, request);
            return Ok(new { message = "Profile updated successfully." });
        }

        [HttpPost("upload-photo")]
        public async Task<IActionResult> UploadPhoto(IFormFile photo)
        {
            var userId = GetCurrentUserId();
            if (userId == null) return Unauthorized();

            if (photo == null || photo.Length == 0)
                return BadRequest(new { message = "No photo provided." });

            // Перевірка типу файлу
            var allowedTypes = new[] { "image/jpeg", "image/jpg", "image/png", "image/gif" };
            if (!allowedTypes.Contains(photo.ContentType.ToLower()))
                return BadRequest(new { message = "Invalid file type. Only JPEG, PNG, and GIF are allowed." });

            // Перевірка розміру (макс 5MB)
            if (photo.Length > 5 * 1024 * 1024)
                return BadRequest(new { message = "File size too large. Maximum size is 5MB." });

            // Генеруємо унікальне ім'я
            var fileName = $"{userId}_{Guid.NewGuid()}{Path.GetExtension(photo.FileName)}";
            var uploadsPath = Path.Combine("wwwroot", "uploads", "photos");
            Directory.CreateDirectory(uploadsPath);
            var filePath = Path.Combine(uploadsPath, fileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await photo.CopyToAsync(stream);
            }

            var photoUrl = $"/uploads/photos/{fileName}";

            await _userService.UpdatePhotoAsync(userId.Value, photoUrl);

            _logger.LogInformation("Photo uploaded for user: {UserId}", userId);
            return Ok(new { message = "Photo uploaded successfully.", photoUrl });
        }

        [HttpDelete("photo")]
        public async Task<IActionResult> DeletePhoto()
        {
            var userId = GetCurrentUserId();
            if (userId == null) return Unauthorized();

            await _userService.UpdatePhotoAsync(userId.Value, null);
            _logger.LogInformation("Photo deleted for user: {UserId}", userId);

            return Ok(new { message = "Photo deleted successfully." });
        }

        [HttpGet("all")]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> GetAllUsers()
        {
            var users = await _userService.GetAllAsync();
            return Ok(users);
        }

        [HttpPost("{id}/block")]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> BlockUser(int id)
        {
            var success = await _userService.BlockUserAsync(id);
            if (success) return Ok(new { message = "User blocked successfully." });
            return NotFound(new { message = "User not found." });
        }

        [HttpPost("{id}/unblock")]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> UnblockUser(int id)
        {
            var success = await _userService.UnblockUserAsync(id);
            if (success) return Ok(new { message = "User unblocked successfully." });
            return NotFound(new { message = "User not found." });
        }

        private int? GetCurrentUserId()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            return int.TryParse(userIdClaim, out var userId) ? userId : null;
        }

        [HttpGet("{id}")]
        [Authorize] 
        public async Task<IActionResult> GetUserById(int id)
        {
            var user = await _userService.GetByIdAsync(id);
            if (user == null) return NotFound();

            var publicProfile = new PublicUserProfileDto
            {
                Id = user.Id,
                FullName = user.FullName,
                Faculty = user.Faculty,
                Course = user.Course,
                Bio = user.Bio,
                PhotoUrl = user.PhotoUrl
            };

            return Ok(publicProfile);
        }


    }
}
