using CHNU_Connect.BLL.DTOs.Subject;
using CHNU_Connect.BLL.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CHNU_Connect.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class SubjectController : ControllerBase
    {
        private readonly ISubjectService _subjectService;

        public SubjectController(ISubjectService subjectService)
        {
            _subjectService = subjectService;
        }

        [HttpGet("group/{groupId}")]
        public async Task<IActionResult> GetByGroup(int groupId)
        {
            return Ok(await _subjectService.GetByGroupIdAsync(groupId));
        }

        [HttpPost]
        [Authorize(Roles = "admin,superAdmin")]
        public async Task<IActionResult> Create(CreateSubjectDto dto)
        {
            var result = await _subjectService.CreateAsync(dto);
            return Ok(result);
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "admin,superAdmin")]
        public async Task<IActionResult> Update(int id, CreateSubjectDto dto)
        {
            var result = await _subjectService.UpdateAsync(id, dto);
            return result == null ? NotFound() : Ok(result);
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "admin,superAdmin")]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _subjectService.DeleteAsync(id);
            return result ? Ok() : NotFound();
        }
    }
}
