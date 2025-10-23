using CHNU_Connect.BLL.DTOs.Group;
using CHNU_Connect.BLL.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace CHNU_Connect.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class GroupController : ControllerBase
    {
        private readonly IGroupService _groupService;
        private readonly ILogger<GroupController> _logger;

        public GroupController(IGroupService groupService, ILogger<GroupController> logger)
        {
            _groupService = groupService;
            _logger = logger;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllGroups()
        {
            try
            {
                var groups = await _groupService.GetAllAsync();
                return Ok(groups);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting all groups");
                return StatusCode(500, new { message = "An error occurred while retrieving groups." });
            }
        }

        [HttpGet("public")]
        public async Task<IActionResult> GetPublicGroups()
        {
            try
            {
                var groups = await _groupService.GetPublicGroupsAsync();
                return Ok(groups);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting public groups");
                return StatusCode(500, new { message = "An error occurred while retrieving public groups." });
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetGroup(int id)
        {
            try
            {
                var group = await _groupService.GetByIdAsync(id);
                if (group == null)
                    return NotFound(new { message = "Group not found." });

                return Ok(group);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting group: {GroupId}", id);
                return StatusCode(500, new { message = "An error occurred while retrieving the group." });
            }
        }

        [HttpGet("user/{userId}")]
        public async Task<IActionResult> GetGroupsByUser(int userId)
        {
            try
            {
                var groups = await _groupService.GetByCreatorIdAsync(userId);
                return Ok(groups);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting groups for user: {UserId}", userId);
                return StatusCode(500, new { message = "An error occurred while retrieving user groups." });
            }
        }

        [HttpGet("my-groups")]
        public async Task<IActionResult> GetMyGroups()
        {
            try
            {
                var currentUserId = GetCurrentUserId();
                if (currentUserId == null)
                    return Unauthorized();

                var groups = await _groupService.GetUserGroupsAsync(currentUserId.Value);
                return Ok(groups);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting my groups for user: {UserId}", GetCurrentUserId());
                return StatusCode(500, new { message = "An error occurred while retrieving your groups." });
            }
        }

        [HttpPost]
        public async Task<IActionResult> CreateGroup([FromBody] CreateGroupDto request)
        {
            try
            {
                var currentUserId = GetCurrentUserId();
                if (currentUserId == null)
                    return Unauthorized();

                request.CreatedById = currentUserId.Value;
                var group = await _groupService.CreateGroupAsync(request);
                
                _logger.LogInformation("Group created by user: {UserId}", currentUserId);
                return CreatedAtAction(nameof(GetGroup), new { id = group.Id }, group);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating group for user: {UserId}", GetCurrentUserId());
                return StatusCode(500, new { message = "An error occurred while creating the group." });
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateGroup(int id, [FromBody] CreateGroupDto request)
        {
            try
            {
                var currentUserId = GetCurrentUserId();
                if (currentUserId == null)
                    return Unauthorized();

                var group = await _groupService.GetByIdAsync(id);
                if (group == null)
                    return NotFound(new { message = "Group not found." });

                // Check if user is the creator
                if (group.CreatedById != currentUserId.Value)
                    return Forbid("You can only edit groups you created.");

                var updatedGroup = await _groupService.UpdateGroupAsync(id, request);
                
                _logger.LogInformation("Group updated: {GroupId} by user: {UserId}", id, currentUserId);
                return Ok(updatedGroup);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating group: {GroupId}", id);
                return StatusCode(500, new { message = "An error occurred while updating the group." });
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteGroup(int id)
        {
            try
            {
                var currentUserId = GetCurrentUserId();
                if (currentUserId == null)
                    return Unauthorized();

                var group = await _groupService.GetByIdAsync(id);
                if (group == null)
                    return NotFound(new { message = "Group not found." });

                // Check if user is the creator
                if (group.CreatedById != currentUserId.Value)
                    return Forbid("You can only delete groups you created.");

                var success = await _groupService.DeleteGroupAsync(id);
                if (!success)
                    return BadRequest(new { message = "Failed to delete group." });

                _logger.LogInformation("Group deleted: {GroupId} by user: {UserId}", id, currentUserId);
                return Ok(new { message = "Group deleted successfully." });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting group: {GroupId}", id);
                return StatusCode(500, new { message = "An error occurred while deleting the group." });
            }
        }

        [HttpPost("{id}/join")]
        public async Task<IActionResult> JoinGroup(int id)
        {
            try
            {
                var currentUserId = GetCurrentUserId();
                if (currentUserId == null)
                    return Unauthorized();

                var success = await _groupService.JoinGroupAsync(id, currentUserId.Value);
                if (!success)
                    return BadRequest(new { message = "Already joined this group or group not found." });

                _logger.LogInformation("User joined group: {GroupId} by user: {UserId}", id, currentUserId);
                return Ok(new { message = "Successfully joined the group." });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error joining group: {GroupId}", id);
                return StatusCode(500, new { message = "An error occurred while joining the group." });
            }
        }

        [HttpDelete("{id}/leave")]
        public async Task<IActionResult> LeaveGroup(int id)
        {
            try
            {
                var currentUserId = GetCurrentUserId();
                if (currentUserId == null)
                    return Unauthorized();

                var success = await _groupService.LeaveGroupAsync(id, currentUserId.Value);
                if (!success)
                    return BadRequest(new { message = "Not a member of this group or group not found." });

                _logger.LogInformation("User left group: {GroupId} by user: {UserId}", id, currentUserId);
                return Ok(new { message = "Successfully left the group." });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error leaving group: {GroupId}", id);
                return StatusCode(500, new { message = "An error occurred while leaving the group." });
            }
        }

        private int? GetCurrentUserId()
        {
            var userIdClaim = User.FindFirst("userId")?.Value;
            return int.TryParse(userIdClaim, out var userId) ? userId : null;
        }
    }
}
