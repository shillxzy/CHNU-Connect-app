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
        public async Task<IActionResult> GetAll()
        {
            var groups = await _groupService.GetAllAsync();
            return Ok(groups);
        }

        [HttpGet("public")]
        public async Task<IActionResult> GetPublic()
        {
            var groups = await _groupService.GetPublicGroupsAsync();
            return Ok(groups);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var group = await _groupService.GetByIdAsync(id);

            if (group == null)
                return NotFound(new { message = "Group not found" });

            return Ok(group);
        }

        [HttpGet("my")]
        public async Task<IActionResult> MyGroups()
        {
            var userId = GetUserId();
            if (userId == null)
                return Unauthorized();

            var groups = await _groupService.GetUserGroupsAsync(userId.Value);
            return Ok(groups);
        }

        [HttpGet("creator/{creatorId}")]
        public async Task<IActionResult> GetByCreator(int creatorId)
        {
            var groups = await _groupService.GetByCreatorIdAsync(creatorId);
            return Ok(groups);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateGroupDto dto)
        {
            var userId = GetUserId();
            if (userId == null)
                return Unauthorized();

            try
            {
                dto.CreatorId = userId.Value;

                var group = await _groupService.CreateGroupAsync(dto);

                return CreatedAtAction(nameof(GetById), new { id = group.Id }, group);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating group");
                return StatusCode(500, new { message = "Error creating group" });
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] CreateGroupDto dto)
        {
            var userId = GetUserId();
            if (userId == null)
                return Unauthorized();

            try
            {
                var result = await _groupService.UpdateGroupAsync(id, dto, userId.Value);

                if (result == null)
                    return Forbid();

                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating group {GroupId}", id);
                return StatusCode(500, new { message = "Error updating group" });
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var userId = GetUserId();
            if (userId == null)
                return Unauthorized();

            try
            {
                var success = await _groupService.DeleteGroupAsync(id, userId.Value);

                if (!success)
                    return Forbid();

                return Ok(new { message = "Group deleted" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting group {GroupId}", id);
                return StatusCode(500, new { message = "Error deleting group" });
            }
        }

        [HttpPost("{id}/join")]
        public async Task<IActionResult> Join(int id)
        {
            var userId = GetUserId();
            if (userId == null)
                return Unauthorized();

            try
            {
                var result = await _groupService.JoinGroupAsync(id, userId.Value);

                if (!result)
                    return BadRequest(new { message = "Already in group or invalid group" });

                return Ok(new { message = "Joined successfully" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error joining group {GroupId}", id);
                return StatusCode(500, new { message = "Error joining group" });
            }
        }

        [HttpDelete("{id}/leave")]
        public async Task<IActionResult> Leave(int id)
        {
            var userId = GetUserId();
            if (userId == null)
                return Unauthorized();

            try
            {
                var result = await _groupService.LeaveGroupAsync(id, userId.Value);

                if (!result)
                    return BadRequest(new { message = "Not a member of this group" });

                return Ok(new { message = "Left group" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error leaving group {GroupId}", id);
                return StatusCode(500, new { message = "Error leaving group" });
            }
        }

        private int? GetUserId()
        {
            var claim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            return int.TryParse(claim, out var id) ? id : null;
        }
    }
}
