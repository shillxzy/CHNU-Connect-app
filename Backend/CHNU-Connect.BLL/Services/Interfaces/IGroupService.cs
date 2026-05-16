using CHNU_Connect.BLL.DTOs.Group;

namespace CHNU_Connect.BLL.Services.Interfaces
{
    public interface IGroupService
    {
        Task<GroupDto> CreateGroupAsync(CreateGroupDto dto);
        Task<GroupDto?> GetByIdAsync(int id);
        Task<IEnumerable<GroupDto>> GetAllAsync();

        Task<IEnumerable<GroupDto>> GetByCreatorIdAsync(int creatorId);
        Task<IEnumerable<GroupDto>> GetPublicGroupsAsync();
        Task<IEnumerable<GroupDto>> GetUserGroupsAsync(int userId);

        Task<GroupDto?> UpdateGroupAsync(int id, CreateGroupDto dto, int userId);
        Task<bool> DeleteGroupAsync(int id, int userId);
        Task<bool> AssignCuratorAsync(int groupId, int curatorId, int currentUserId);
        Task<IEnumerable<GroupDto>> GetCuratedGroupsAsync(int userId);
        Task<bool> IsCuratorAsync(int groupId, int userId);
    }
}
