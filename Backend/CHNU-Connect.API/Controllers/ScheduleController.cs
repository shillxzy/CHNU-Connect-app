using CHNU_Connect.BLL.DTOs.Schedule;
using CHNU_Connect.BLL.Services.Interfaces;
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

        public ScheduleController(IScheduleService scheduleService)
        {
            _scheduleService = scheduleService;
        }

        // ================= PUBLIC =================

        /// 🔥 Get group schedule (main endpoint for frontend grid)
        [HttpGet("group/{groupId}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetByGroup(int groupId)
        {
            var result = await _scheduleService.GetByGroupAsync(groupId);
            return Ok(result);
        }

        /// 🔥 Get current user schedule
        [HttpGet("my")]
        [Authorize]
        public async Task<IActionResult> GetMySchedule()
        {
            var userId = GetUserId();
            if (userId == null) return Unauthorized();

            var result = await _scheduleService.GetByUserAsync(userId.Value);
            return Ok(result);
        }

        // ================= ADMIN ACTIONS =================

        /// 🔥 Create lesson (drag create or admin panel)
        [HttpPost]
        [Authorize(Roles = "admin,superAdmin")]
        public async Task<IActionResult> Create([FromBody] CreateScheduleDto dto)
        {
            var result = await _scheduleService.CreateAsync(dto);
            return Ok(result);
        }

        /// 🔥 Move lesson (DRAG & DROP CORE)
        [HttpPut("move")]
        [Authorize(Roles = "admin,superAdmin")]
        public async Task<IActionResult> Move([FromBody] MoveScheduleDto dto)
        {
            var result = await _scheduleService.MoveAsync(dto);

            if (!result)
                return BadRequest("Move failed (conflict or not found)");

            return Ok(result);
        }

        /// 🔥 Swap lessons
        [HttpPut("swap")]
        [Authorize(Roles = "admin,superAdmin")]
        public async Task<IActionResult> Swap([FromBody] SwapScheduleDto dto)
        {
            var result = await _scheduleService.SwapAsync(dto.ScheduleId1, dto.ScheduleId2);

            if (!result)
                return BadRequest("Swap failed");

            return Ok(result);
        }

        /// 🔥 Delete lesson
        [HttpDelete("{id}")]
        [Authorize(Roles = "admin,superAdmin")]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _scheduleService.DeleteAsync(id);

            if (!result)
                return NotFound();

            return Ok();
        }

        // ================= HELPERS =================

        private int? GetUserId()
        {
            var claim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            return int.TryParse(claim, out var id) ? id : null;
        }
    }
}
