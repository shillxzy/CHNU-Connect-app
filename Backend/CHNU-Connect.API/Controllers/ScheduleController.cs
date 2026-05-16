using CHNU_Connect.BLL.DTOs.Schedule;
using CHNU_Connect.BLL.DTOs.SubGroup;
using CHNU_Connect.BLL.Services.Interfaces;
using CHNU_Connect.DAL.Repositories.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace CHNU_Connect.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ScheduleController : ControllerBase
    {
        private readonly IScheduleService _scheduleService;
        private readonly ISubGroupService _subGroupService;
        private readonly ILessonSlotRepository _slotRepo;
        private readonly IAdminPermissionService _permissionService;
        private readonly IGroupRepository _groupRepo;
        private readonly IScheduleRepository _scheduleRepo;
        private readonly ISubGroupRepository _subGroupRepo;

        public ScheduleController(
            IScheduleService scheduleService,
            ISubGroupService subGroupService,
            ILessonSlotRepository slotRepo,
            IAdminPermissionService permissionService,
            IGroupRepository groupRepo,
            IScheduleRepository scheduleRepo,
            ISubGroupRepository subGroupRepo)
        {
            _scheduleService = scheduleService;
            _subGroupService = subGroupService;
            _slotRepo = slotRepo;
            _permissionService = permissionService;
            _groupRepo = groupRepo;
            _scheduleRepo = scheduleRepo;
            _subGroupRepo = subGroupRepo;
        }

        // ==================== SLOTS ====================

        [HttpGet("slots")]
        [AllowAnonymous]
        public async Task<IActionResult> GetSlots()
        {
            var slots = await _slotRepo.GetOrderedAsync();
            return Ok(slots);
        }

        [HttpPost("slots/seed")]
        [Authorize(Roles = "admin,superAdmin")]
        public async Task<IActionResult> SeedSlots()
        {
            if (!await CanManageScheduleAdmin())
                return StatusCode(403, new { message = "You need ManageSchedule permission." });

            await _scheduleService.SeedSlotsAsync();
            return Ok(new { message = "7 пар успішно додано до БД" });
        }

        // ==================== SCHEDULE ====================

        [HttpGet("group/{groupId}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetByGroup(int groupId)
        {
            var result = await _scheduleService.GetByGroupAsync(groupId);
            return Ok(result);
        }

        [HttpGet("my")]
        [Authorize]
        public async Task<IActionResult> GetMySchedule()
        {
            var userId = GetUserId();
            if (userId == null) return Unauthorized();
            var result = await _scheduleService.GetByUserAsync(userId.Value);
            return Ok(result);
        }

        [HttpPost]
        [Authorize(Roles = "admin,superAdmin,teacher")]
        public async Task<IActionResult> Create([FromBody] CreateScheduleDto dto)
        {
            if (!await CanManageScheduleForGroup(dto.GroupId))
                return StatusCode(403, new { message = "Access denied. You are not the curator of this group." });

            var result = await _scheduleService.CreateAsync(dto);
            return Ok(result);
        }

        [HttpPut("move")]
        [Authorize(Roles = "admin,superAdmin,teacher")]
        public async Task<IActionResult> Move([FromBody] MoveScheduleDto dto)
        {
            var schedule = await _scheduleRepo.GetByIdAsync(dto.ScheduleId);
            if (schedule == null) return NotFound();
            if (!await CanManageScheduleForGroup(schedule.GroupId))
                return StatusCode(403, new { message = "Access denied. You are not the curator of this group." });

            var result = await _scheduleService.MoveAsync(dto);
            if (!result) return BadRequest("Move failed (conflict or not found)");
            return Ok(result);
        }

        [HttpPut("swap")]
        [Authorize(Roles = "admin,superAdmin,teacher")]
        public async Task<IActionResult> Swap([FromBody] SwapScheduleDto dto)
        {
            var schedule = await _scheduleRepo.GetByIdAsync(dto.ScheduleId1);
            if (schedule == null) return NotFound();
            if (!await CanManageScheduleForGroup(schedule.GroupId))
                return StatusCode(403, new { message = "Access denied. You are not the curator of this group." });

            var result = await _scheduleService.SwapAsync(dto.ScheduleId1, dto.ScheduleId2);
            if (!result) return BadRequest("Swap failed");
            return Ok(result);
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "admin,superAdmin,teacher")]
        public async Task<IActionResult> Delete(int id)
        {
            var schedule = await _scheduleRepo.GetByIdAsync(id);
            if (schedule == null) return NotFound();
            if (!await CanManageScheduleForGroup(schedule.GroupId))
                return StatusCode(403, new { message = "Access denied. You are not the curator of this group." });

            var result = await _scheduleService.DeleteAsync(id);
            if (!result) return NotFound();
            return Ok();
        }

        // ==================== SUBGROUPS ====================

        [HttpGet("subgroups/{groupId}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetSubGroups(int groupId)
        {
            var result = await _subGroupService.GetByGroupAsync(groupId);
            return Ok(result);
        }

        [HttpPost("subgroups")]
        [Authorize(Roles = "admin,superAdmin,teacher")]
        public async Task<IActionResult> CreateSubGroup([FromBody] CreateSubGroupRequest req)
        {
            if (!await CanManageScheduleForGroup(req.GroupId))
                return StatusCode(403, new { message = "Access denied. You are not the curator of this group." });

            var result = await _subGroupService.CreateAsync(req.GroupId, req.Name);
            return Ok(result);
        }

        [HttpPut("subgroups/{id}")]
        [Authorize(Roles = "admin,superAdmin,teacher")]
        public async Task<IActionResult> UpdateSubGroup(int id, [FromBody] UpdateSubGroupRequest req)
        {
            var subGroup = await _subGroupRepo.GetByIdAsync(id);
            if (subGroup == null) return NotFound();
            if (!await CanManageScheduleForGroup(subGroup.GroupId))
                return StatusCode(403, new { message = "Access denied. You are not the curator of this group." });

            var result = await _subGroupService.UpdateAsync(id, req.Name);
            if (result == null) return NotFound();
            return Ok(result);
        }

        [HttpDelete("subgroups/{id}")]
        [Authorize(Roles = "admin,superAdmin,teacher")]
        public async Task<IActionResult> DeleteSubGroup(int id)
        {
            var subGroup = await _subGroupRepo.GetByIdAsync(id);
            if (subGroup == null) return NotFound();
            if (!await CanManageScheduleForGroup(subGroup.GroupId))
                return StatusCode(403, new { message = "Access denied. You are not the curator of this group." });

            var result = await _subGroupService.DeleteAsync(id);
            if (!result) return NotFound();
            return Ok();
        }

        // ==================== HELPERS ====================

        private int? GetUserId()
        {
            var claim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            return int.TryParse(claim, out var id) ? id : null;
        }

        private async Task<bool> CanManageScheduleAdmin()
        {
            if (User.IsInRole("superAdmin")) return true;
            var userId = GetUserId();
            if (userId == null) return false;
            return await _permissionService.HasPermissionAsync(userId.Value, "ManageSchedule");
        }

        private async Task<bool> CanManageScheduleForGroup(int groupId)
        {
            if (User.IsInRole("superAdmin")) return true;
            var userId = GetUserId();
            if (userId == null) return false;
            if (User.IsInRole("admin"))
                return await _permissionService.HasPermissionAsync(userId.Value, "ManageSchedule");
            if (User.IsInRole("teacher"))
            {
                var group = await _groupRepo.GetByIdAsync(groupId);
                return group?.CuratorId == userId.Value;
            }
            return false;
        }
    }

    public record CreateSubGroupRequest(int GroupId, string Name);
    public record UpdateSubGroupRequest(string Name);
}
