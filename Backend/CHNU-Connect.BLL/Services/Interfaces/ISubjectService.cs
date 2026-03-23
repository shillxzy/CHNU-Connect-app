using CHNU_Connect.BLL.DTOs.Subject;

namespace CHNU_Connect.BLL.Services.Interfaces
{
    public interface ISubjectService
    {
        Task<IEnumerable<SubjectDto>> GetByGroupIdAsync(int groupId);
        Task<SubjectDto> CreateAsync(CreateSubjectDto dto);
        Task<SubjectDto?> UpdateAsync(int id, CreateSubjectDto dto);
        Task<bool> DeleteAsync(int id);
    }
}
