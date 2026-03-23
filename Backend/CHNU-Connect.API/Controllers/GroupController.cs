using CHNU_Connect.BLL.DTOs.Group;
using CHNU_Connect.BLL.Services.Interfaces;
using CHNU_Connect.DAL.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class GroupController : ControllerBase
{
    private readonly IGroupService _groupService;
    private readonly IGroupMemberService _memberService;
    private readonly ILogger<GroupController> _logger;

    public GroupController(
        IGroupService groupService,
        IGroupMemberService memberService,
        ILogger<GroupController> logger)
    {
        _groupService = groupService;
        _memberService = memberService;
        _logger = logger;
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var userId = GetUserId();
        if (userId == null) return Unauthorized();

        var isMember = await _memberService.IsMemberAsync(id, userId.Value);

        if (!isMember)
            return Forbid(); 
        var group = await _groupService.GetByIdAsync(id);

        if (group == null)
            return NotFound();

        return Ok(group);
    }

    [HttpGet("my")]
    public async Task<IActionResult> MyGroups()
    {
        var userId = GetUserId();
        if (userId == null) return Unauthorized();

        var groups = await _groupService.GetUserGroupsAsync(userId.Value);
        return Ok(groups);
    }

    [HttpPost]
    [Authorize(Roles = "admin,superAdmin")]
    public async Task<IActionResult> Create(CreateGroupDto dto)
    {
        var userId = GetUserId();
        if (userId == null) return Unauthorized();

        try
        {
            dto.CreatorId = userId.Value;

            var group = await _groupService.CreateGroupAsync(dto);

            await _memberService.JoinAsync(group.Id, userId.Value);

            return CreatedAtAction(nameof(GetById), new { id = group.Id }, group);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating group");
            return StatusCode(500);
        }
    }

    [HttpPut("{id}")]
    [Authorize(Roles = "admin,superAdmin")]
    public async Task<IActionResult> Update(int id, CreateGroupDto dto)
    {
        var userId = GetUserId();
        if (userId == null) return Unauthorized();

        var group = await _groupService.UpdateGroupAsync(id, dto, userId.Value);

        if (group == null)
            return Forbid();

        return Ok(group);
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = "admin,superAdmin")]
    public async Task<IActionResult> Delete(int id)
    {
        var userId = GetUserId();
        if (userId == null) return Unauthorized();

        var success = await _groupService.DeleteGroupAsync(id, userId.Value);

        if (!success)
            return Forbid();

        return Ok();
    }

    private int? GetUserId()
    {
        var claim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        return int.TryParse(claim, out var id) ? id : null;
    }
}
