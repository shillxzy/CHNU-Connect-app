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
        Task<bool> JoinAsync(int groupId, int userId);
        Task<bool> LeaveAsync(int groupId, int userId);

        Task<bool> IsMemberAsync(int groupId, int userId);
        Task<string?> GetUserRoleAsync(int groupId, int userId);

        Task<IEnumerable<int>> GetGroupIdsByUserAsync(int userId);
    }

}
