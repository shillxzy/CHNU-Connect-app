using CHNU_Connect.BLL.DTOs.GroupMember;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CHNU_Connect.BLL.Services.Interfaces
{
    public interface IGroupMemberService
    {
        Task<GroupMemberDto> CreateGroupMemberAsync(CreateGroupMemberDto dto);
        Task<GroupMemberDto?> GetByIdAsync(int id);
        Task<IEnumerable<GroupMemberDto>> GetByGroupIdAsync(int groupId);
        Task<IEnumerable<GroupMemberDto>> GetByUserIdAsync(int userId);
        Task<GroupMemberDto> UpdateGroupMemberAsync(int id, CreateGroupMemberDto dto);
        Task<bool> DeleteGroupMemberAsync(int id);
        Task<bool> IsUserMemberAsync(int groupId, int userId);
        Task<string?> GetUserRoleAsync(int groupId, int userId);
    }
}
