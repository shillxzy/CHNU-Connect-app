using CHNU_Connect.BLL.DTOs.AdminAction;
using CHNU_Connect.BLL.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace CHNU_Connect.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "admin")]
    public class AdminController : ControllerBase
    {
        private readonly IAdminActionService _adminActionService;
        private readonly IUserService _userService;
        private readonly ILogger<AdminController> _logger;

        public AdminController(
            IAdminActionService adminActionService, 
            IUserService userService, 
            ILogger<AdminController> logger)
        {
            _adminActionService = adminActionService;
            _userService = userService;
            _logger = logger;
        }

        [HttpGet("actions")]
        public async Task<IActionResult> GetAllAdminActions()
        {
            try
            {
                var actions = await _adminActionService.GetAllAsync();
                return Ok(actions);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting all admin actions");
                return StatusCode(500, new { message = "An error occurred while retrieving admin actions." });
            }
        }

        [HttpGet("actions/{id}")]
        public async Task<IActionResult> GetAdminAction(int id)
        {
            try
            {
                var action = await _adminActionService.GetByIdAsync(id);
                if (action == null)
                    return NotFound(new { message = "Admin action not found." });

                return Ok(action);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting admin action: {ActionId}", id);
                return StatusCode(500, new { message = "An error occurred while retrieving the admin action." });
            }
        }

        [HttpGet("actions/admin/{adminId}")]
        public async Task<IActionResult> GetActionsByAdmin(int adminId)
        {
            try
            {
                var actions = await _adminActionService.GetByAdminIdAsync(adminId);
                return Ok(actions);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting actions for admin: {AdminId}", adminId);
                return StatusCode(500, new { message = "An error occurred while retrieving admin actions." });
            }
        }

        [HttpGet("actions/target/{targetUserId}")]
        public async Task<IActionResult> GetActionsByTargetUser(int targetUserId)
        {
            try
            {
                var actions = await _adminActionService.GetByTargetUserIdAsync(targetUserId);
                return Ok(actions);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting actions for target user: {TargetUserId}", targetUserId);
                return StatusCode(500, new { message = "An error occurred while retrieving actions for target user." });
            }
        }

        [HttpPost("actions")]
        public async Task<IActionResult> CreateAdminAction([FromBody] CreateAdminActionDto request)
        {
            try
            {
                var currentUserId = GetCurrentUserId();
                if (currentUserId == null)
                    return Unauthorized();

                request.AdminId = currentUserId.Value;
                var action = await _adminActionService.CreateAdminActionAsync(request);
                
                _logger.LogInformation("Admin action created: {Action} on user: {TargetUserId} by admin: {AdminId}", 
                    request.Action, request.TargetId, currentUserId);
                return CreatedAtAction(nameof(GetAdminAction), new { id = action.Id }, action);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating admin action for user: {UserId}", GetCurrentUserId());
                return StatusCode(500, new { message = "An error occurred while creating the admin action." });
            }
        }

        [HttpDelete("actions/{id}")]
        public async Task<IActionResult> DeleteAdminAction(int id)
        {
            try
            {
                var success = await _adminActionService.DeleteAdminActionAsync(id);
                if (!success)
                    return BadRequest(new { message = "Failed to delete admin action." });

                _logger.LogInformation("Admin action deleted: {ActionId} by admin: {AdminId}", id, GetCurrentUserId());
                return Ok(new { message = "Admin action deleted successfully." });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting admin action: {ActionId}", id);
                return StatusCode(500, new { message = "An error occurred while deleting the admin action." });
            }
        }

        [HttpPost("users/{userId}/block")]
        public async Task<IActionResult> BlockUser(int userId, [FromBody] BlockUserDto request)
        {
            try
            {
                var currentUserId = GetCurrentUserId();
                if (currentUserId == null)
                    return Unauthorized();

                var success = await _userService.BlockUserAsync(userId);
                if (!success)
                    return BadRequest(new { message = "Failed to block user." });

                // Log the admin action
                var adminAction = new CreateAdminActionDto
                {
                    AdminId = currentUserId.Value,
                    TargetId = new Guid(userId.ToString()),
                    Action = "block_user",
                    Reason = request.Reason
                };
                await _adminActionService.CreateAdminActionAsync(adminAction);

                _logger.LogInformation("User blocked: {UserId} by admin: {AdminId}", userId, currentUserId);
                return Ok(new { message = "User blocked successfully." });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error blocking user: {UserId}", userId);
                return StatusCode(500, new { message = "An error occurred while blocking the user." });
            }
        }

        [HttpPost("users/{userId}/unblock")]
        public async Task<IActionResult> UnblockUser(int userId, [FromBody] UnblockUserDto request)
        {
            try
            {
                var currentUserId = GetCurrentUserId();
                if (currentUserId == null)
                    return Unauthorized();

                var success = await _userService.UnblockUserAsync(userId);
                if (!success)
                    return BadRequest(new { message = "Failed to unblock user." });

                // Log the admin action
                var adminAction = new CreateAdminActionDto
                {
                    AdminId = currentUserId.Value,
                    TargetId = new Guid(userId.ToString()),
                    Action = "unblock_user",
                    Reason = request.Reason
                };
                await _adminActionService.CreateAdminActionAsync(adminAction);

                _logger.LogInformation("User unblocked: {UserId} by admin: {AdminId}", userId, currentUserId);
                return Ok(new { message = "User unblocked successfully." });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error unblocking user: {UserId}", userId);
                return StatusCode(500, new { message = "An error occurred while unblocking the user." });
            }
        }

        [HttpGet("users")]
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

        [HttpGet("users/{id}")]
        public async Task<IActionResult> GetUser(int id)
        {
            try
            {
                var user = await _userService.GetByIdAsync(id);
                if (user == null)
                    return NotFound(new { message = "User not found." });

                return Ok(user);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting user: {UserId}", id);
                return StatusCode(500, new { message = "An error occurred while retrieving the user." });
            }
        }

        private int? GetCurrentUserId()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            return int.TryParse(userIdClaim, out var userId) ? userId : null;
        }
    }

    public class BlockUserDto
    {
        public string? Reason { get; set; }
    }

    public class UnblockUserDto
    {
        public string? Reason { get; set; }
    }
}
