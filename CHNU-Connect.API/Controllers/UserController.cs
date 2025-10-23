using CHNU_Connect.BLL.DTOs.User;
using CHNU_Connect.BLL.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
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
            try
            {
                var userId = GetCurrentUserId();
                if (userId == null)
                    return Unauthorized();

                var user = await _userService.GetByIdAsync(userId.Value);
                if (user == null)
                    return NotFound(new { message = "User not found." });

                return Ok(user);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting user profile");
                return StatusCode(500, new { message = "An error occurred while retrieving profile." });
            }
        }

        [HttpPut("profile")]
        public async Task<IActionResult> UpdateProfile([FromBody] UpdateProfileDto request)
        {
            try
            {
                var userId = GetCurrentUserId();
                if (userId == null)
                    return Unauthorized();

                var updateDto = new CreateUserDto
                {
                    Email = request.Email,
                    FullName = request.FullName,
                    Faculty = request.Faculty,
                    Course = request.Course,
                    Bio = request.Bio
                };

                var updatedUser = await _userService.UpdateUserAsync(userId.Value, updateDto);
                
                _logger.LogInformation("User profile updated: {UserId}", userId);
                return Ok(updatedUser);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating user profile for user: {UserId}", GetCurrentUserId());
                return StatusCode(500, new { message = "An error occurred while updating profile." });
            }
        }

        [HttpPost("upload-photo")]
        public async Task<IActionResult> UploadPhoto(IFormFile photo)
        {
            try
            {
                var userId = GetCurrentUserId();
                if (userId == null)
                    return Unauthorized();

                if (photo == null || photo.Length == 0)
                    return BadRequest(new { message = "No photo provided." });

                // Validate file type
                var allowedTypes = new[] { "image/jpeg", "image/jpg", "image/png", "image/gif" };
                if (!allowedTypes.Contains(photo.ContentType.ToLower()))
                    return BadRequest(new { message = "Invalid file type. Only JPEG, PNG, and GIF images are allowed." });

                // Validate file size (max 5MB)
                if (photo.Length > 5 * 1024 * 1024)
                    return BadRequest(new { message = "File size too large. Maximum size is 5MB." });

                // Generate unique filename
                var fileName = $"{userId}_{Guid.NewGuid()}{Path.GetExtension(photo.FileName)}";
                var uploadsPath = Path.Combine("wwwroot", "uploads", "photos");
                Directory.CreateDirectory(uploadsPath);
                var filePath = Path.Combine(uploadsPath, fileName);

                // Save file
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await photo.CopyToAsync(stream);
                }

                // Update user photo URL
                var photoUrl = $"/uploads/photos/{fileName}";
                var user = await _userService.GetByIdAsync(userId.Value);
                if (user != null)
                {
                    var updateDto = new CreateUserDto
                    {
                        Email = user.Email,
                        FullName = user.FullName,
                        Faculty = user.Faculty,
                        Course = user.Course,
                        Bio = user.Bio,
                        PhotoUrl = photoUrl
                    };

                    await _userService.UpdateUserAsync(userId.Value, updateDto);
                }

                _logger.LogInformation("Photo uploaded for user: {UserId}", userId);
                return Ok(new { message = "Photo uploaded successfully.", photoUrl });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error uploading photo for user: {UserId}", GetCurrentUserId());
                return StatusCode(500, new { message = "An error occurred while uploading photo." });
            }
        }

        [HttpDelete("photo")]
        public async Task<IActionResult> DeletePhoto()
        {
            try
            {
                var userId = GetCurrentUserId();
                if (userId == null)
                    return Unauthorized();

                var user = await _userService.GetByIdAsync(userId.Value);
                if (user == null)
                    return NotFound(new { message = "User not found." });

                // Remove photo URL from user profile
                var updateDto = new CreateUserDto
                {
                    Email = user.Email,
                    FullName = user.FullName,
                    Faculty = user.Faculty,
                    Course = user.Course,
                    Bio = user.Bio,
                    PhotoUrl = null
                };

                await _userService.UpdateUserAsync(userId.Value, updateDto);

                _logger.LogInformation("Photo deleted for user: {UserId}", userId);
                return Ok(new { message = "Photo deleted successfully." });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting photo for user: {UserId}", GetCurrentUserId());
                return StatusCode(500, new { message = "An error occurred while deleting photo." });
            }
        }

        [HttpGet("all")]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> GetAllUsers()
        {
            try
            {
                var users = await _userService.GetAllAsync();
                return Ok(users);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting all users");
                return StatusCode(500, new { message = "An error occurred while retrieving users." });
            }
        }

        [HttpPost("{id}/block")]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> BlockUser(int id)
        {
            try
            {
                var success = await _userService.BlockUserAsync(id);
                if (success)
                {
                    _logger.LogInformation("User blocked: {UserId}", id);
                    return Ok(new { message = "User blocked successfully." });
                }
                return NotFound(new { message = "User not found." });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error blocking user: {UserId}", id);
                return StatusCode(500, new { message = "An error occurred while blocking user." });
            }
        }

        [HttpPost("{id}/unblock")]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> UnblockUser(int id)
        {
            try
            {
                var success = await _userService.UnblockUserAsync(id);
                if (success)
                {
                    _logger.LogInformation("User unblocked: {UserId}", id);
                    return Ok(new { message = "User unblocked successfully." });
                }
                return NotFound(new { message = "User not found." });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error unblocking user: {UserId}", id);
                return StatusCode(500, new { message = "An error occurred while unblocking user." });
            }
        }

        private int? GetCurrentUserId()
        {
            var userIdClaim = User.FindFirst("userId")?.Value;
            return int.TryParse(userIdClaim, out var userId) ? userId : null;
        }
    }

    public class UpdateProfileDto
    {
        public string? Email { get; set; }
        public string? FullName { get; set; }
        public string? Faculty { get; set; }
        public int? Course { get; set; }
        public string? Bio { get; set; }
    }
}
