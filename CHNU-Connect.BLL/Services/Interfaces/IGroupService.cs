using CHNU_Connect.BLL.DTOs.Group;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CHNU_Connect.BLL.Services.Interfaces
{
    public interface IGroupService
    {
        Task<GroupDto> CreateGroupAsync(CreateGroupDto dto);
        Task<GroupDto?> GetByIdAsync(int id);
        Task<IEnumerable<GroupDto>> GetAllAsync();
        Task<IEnumerable<GroupDto>> GetByCreatorIdAsync(int creatorId);
        Task<IEnumerable<GroupDto>> GetPublicGroupsAsync();
        Task<GroupDto> UpdateGroupAsync(int id, CreateGroupDto dto);
        Task<bool> DeleteGroupAsync(int id);
        Task<bool> JoinGroupAsync(int groupId, int userId);
        Task<bool> LeaveGroupAsync(int groupId, int userId);
        Task<IEnumerable<GroupDto>> GetUserGroupsAsync(int userId);
    }
}
