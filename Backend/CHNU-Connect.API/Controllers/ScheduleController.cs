using CHNU_Connect.BLL.DTOs.Schedule;
using CHNU_Connect.BLL.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace CHNU_Connect.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class ScheduleController : ControllerBase
    {
        private readonly IScheduleService _scheduleService;

        public ScheduleController(IScheduleService scheduleService)
        {
            _scheduleService = scheduleService;
        }

        [HttpGet("group/{groupId}")]
        public async Task<IActionResult> GetByGroup(int groupId)
        {
            return Ok(await _scheduleService.GetByGroupAsync(groupId));
        }

        [HttpGet("my")]
        public async Task<IActionResult> GetMySchedule()
        {
            var userId = GetUserId();
            if (userId == null) return Unauthorized();

            return Ok(await _scheduleService.GetByUserAsync(userId.Value));
        }

        [HttpPost]
        [Authorize(Roles = "admin,superAdmin")]
        public async Task<IActionResult> Create(CreateScheduleDto dto)
        {
            var result = await _scheduleService.CreateAsync(dto);
            return Ok(result);
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "admin,superAdmin")]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _scheduleService.DeleteAsync(id);
            return result ? Ok() : NotFound();
        }

        private int? GetUserId()
        {
            var claim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            return int.TryParse(claim, out var id) ? id : null;
        }
    }
}
