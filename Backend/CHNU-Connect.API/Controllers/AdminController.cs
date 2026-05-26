using CHNU_Connect.BLL.DTOs.AdminAction;
using CHNU_Connect.BLL.DTOs.AdminPermission;
using CHNU_Connect.BLL.DTOs.User;
using CHNU_Connect.DAL.Entities;
using CHNU_Connect.BLL.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace CHNU_Connect.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "admin,superAdmin")]
    public class AdminController : ControllerBase
    {
        private readonly IAdminActionService _adminActionService;
        private readonly IUserService _userService;
        private readonly IAdminPermissionService _permissionService;
        private readonly IActivityLogService _activityLogService;
        private readonly ILogger<AdminController> _logger;

        public AdminController(
            IAdminActionService adminActionService,
            IUserService userService,
            IAdminPermissionService permissionService,
            IActivityLogService activityLogService,
            ILogger<AdminController> logger)
        {
            _adminActionService = adminActionService;
            _userService = userService;
            _permissionService = permissionService;
            _activityLogService = activityLogService;
            _logger = logger;
        }

        // ==================== PERMISSIONS ====================

        /// <summary>Returns the permission list for the currently logged-in user (any role).</summary>
        [HttpGet("my-permissions")]
        [Authorize]
        public async Task<IActionResult> GetMyPermissions()
        {
            var userId = GetCurrentUserId();
            if (userId == null) return Unauthorized();

            var perms = await _permissionService.GetUserPermissionTypesAsync(userId.Value);
            return Ok(perms);
        }

        /// <summary>Returns permissions assigned to a given admin user.</summary>
        [HttpGet("permissions/{userId}")]
        public async Task<IActionResult> GetUserPermissions(int userId)
        {
            var perms = await _permissionService.GetByUserIdAsync(userId);
            return Ok(perms);
        }

        /// <summary>Grants a permission to an admin. Only superAdmin can call this.</summary>
        [HttpPost("permissions/grant")]
        [Authorize(Roles = "superAdmin")]
        public async Task<IActionResult> GrantPermission([FromBody] GrantPermissionDto dto)
        {
            var grantedBy = GetCurrentUserId();
            if (grantedBy == null) return Unauthorized();

            try
            {
                var result = await _permissionService.GrantAsync(dto.UserId, dto.Type, grantedBy.Value);
                _logger.LogInformation("Permission {Type} granted to user {UserId} by {GrantedBy}",
                    dto.Type, dto.UserId, grantedBy);
                return Ok(result);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        /// <summary>Revokes a permission from an admin. Only superAdmin can call this.</summary>
        [HttpDelete("permissions/revoke")]
        [Authorize(Roles = "superAdmin")]
        public async Task<IActionResult> RevokePermission([FromBody] GrantPermissionDto dto)
        {
            try
            {
                var success = await _permissionService.RevokeAsync(dto.UserId, dto.Type);
                if (!success) return NotFound(new { message = "Permission not found." });

                _logger.LogInformation("Permission {Type} revoked from user {UserId}", dto.Type, dto.UserId);
                return Ok(new { message = "Permission revoked." });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        // ==================== ADMIN ACTIONS ====================

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
                if (currentUserId == null) return Unauthorized();

                request.AdminId = currentUserId.Value;
                var action = await _adminActionService.CreateAdminActionAsync(request);

                _logger.LogInformation("Admin action created: {Action} on user: {TargetUserId} by admin: {AdminId}",
                    request.Action, request.TargetUserId, currentUserId);
                return CreatedAtAction(nameof(GetAdminAction), new { id = action.Id }, action);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating admin action");
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

                _logger.LogInformation("Admin action deleted: {ActionId}", id);
                return Ok(new { message = "Admin action deleted successfully." });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting admin action: {ActionId}", id);
                return StatusCode(500, new { message = "An error occurred while deleting the admin action." });
            }
        }

        // ==================== USER MANAGEMENT ====================

        [HttpPost("users/{userId}/block")]
        public async Task<IActionResult> BlockUser(int userId, [FromBody] BlockUserDto request)
        {
            try
            {
                var currentUserId = GetCurrentUserId();
                if (currentUserId == null) return Unauthorized();

                if (!await CanManageUsers())
                    return StatusCode(403, new { message = "You need ManageUsers permission to block users." });

                var success = await _userService.BlockUserAsync(userId);
                if (!success)
                    return BadRequest(new { message = "Failed to block user." });

                await _adminActionService.CreateAdminActionAsync(new CreateAdminActionDto
                {
                    AdminId = currentUserId.Value,
                    TargetUserId = userId,
                    Action = "block_user",
                    Reason = request.Reason
                });

                var adminUser = await _userService.GetByIdAsync(currentUserId.Value);
                await _activityLogService.LogAsync(currentUserId.Value, adminUser?.FullName ?? "Admin", "user_blocked", "User", userId);
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
                if (currentUserId == null) return Unauthorized();

                if (!await CanManageUsers())
                    return StatusCode(403, new { message = "You need ManageUsers permission to unblock users." });

                var success = await _userService.UnblockUserAsync(userId);
                if (!success)
                    return BadRequest(new { message = "Failed to unblock user." });

                await _adminActionService.CreateAdminActionAsync(new CreateAdminActionDto
                {
                    AdminId = currentUserId.Value,
                    TargetUserId = userId,
                    Action = "unblock_user",
                    Reason = request.Reason
                });

                var adminUser2 = await _userService.GetByIdAsync(currentUserId.Value);
                await _activityLogService.LogAsync(currentUserId.Value, adminUser2?.FullName ?? "Admin", "user_unblocked", "User", userId);
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

        [HttpPut("users/{id}")]
        public async Task<IActionResult> UpdateUser(int id, [FromBody] UpdateProfileDto dto)
        {
            try
            {
                await _userService.UpdateProfileAsync(id, dto);
                return Ok(new { message = "User updated successfully." });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating user: {UserId}", id);
                return StatusCode(500, new { message = "An error occurred while updating the user." });
            }
        }

        [HttpPost("set-role")]
        public async Task<IActionResult> SetRole([FromBody] ChangeRoleRequest request)
        {
            if (!await CanManageUsers())
                return StatusCode(403, new { message = "You need ManageUsers permission to change roles." });

            var allowedRoles = new[] { "student", "teacher", "admin" };

            if (!allowedRoles.Contains(request.Role.ToLower()))
                return BadRequest("There is no such role. Available roles: student, teacher, admin");

            var result = await _userService.SetUserRoleAsync(request.UserId, request.Role);

            if (!result)
                return NotFound("User not found");

            return Ok("Role successfully updated");
        }

        // ==================== ACTIVITY LOG ====================

        [HttpGet("activity-log")]
        public async Task<IActionResult> GetActivityLog(
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 20,
            [FromQuery] DateTime? from = null,
            [FromQuery] DateTime? to = null,
            [FromQuery] string? action = null,
            [FromQuery] int? userId = null)
        {
            try
            {
                var (items, totalCount) = await _activityLogService.GetPagedAsync(
                    page, pageSize, from, to, action, userId);
                return Ok(new { items, totalCount, page, pageSize });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting activity log");
                return StatusCode(500, new { message = "An error occurred while retrieving activity log." });
            }
        }

        // ==================== HELPERS ====================

        private int? GetCurrentUserId()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            return int.TryParse(userIdClaim, out var userId) ? userId : null;
        }

        private async Task<bool> CanManageUsers()
        {
            if (User.IsInRole("superAdmin")) return true;
            var userId = GetCurrentUserId();
            if (userId == null) return false;
            return await _permissionService.HasPermissionAsync(userId.Value, "ManageUsers");
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
