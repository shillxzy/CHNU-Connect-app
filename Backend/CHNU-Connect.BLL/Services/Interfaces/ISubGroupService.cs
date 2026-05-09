using CHNU_Connect.BLL.DTOs.SubGroup;

namespace CHNU_Connect.BLL.Services.Interfaces
{
    public interface ISubGroupService
    {
        Task<IEnumerable<SubGroupDto>> GetByGroupAsync(int groupId);
        Task<SubGroupDto> CreateAsync(int groupId, string name);
        Task<SubGroupDto?> UpdateAsync(int id, string name);
        Task<bool> DeleteAsync(int id);
    }
}