using CHNU_Connect.BLL.Services.Interfaces;
using CHNU_Connect.DAL.Entities;
using CHNU_Connect.DAL.Repositories.Interfaces;

namespace CHNU_Connect.BLL.Services
{
    public class GroupMemberService : IGroupMemberService
    {
        private readonly IGroupMemberRepository _groupMemberRepository;

        public GroupMemberService(IGroupMemberRepository groupMemberRepository)
        {
            _groupMemberRepository = groupMemberRepository;
        }

        public async Task<bool> JoinAsync(int groupId, int userId)
        {
            if (await _groupMemberRepository.IsMemberAsync(groupId, userId))
                return false;

            var member = new GroupMember
            {
                GroupId = groupId,
                UserId = userId,
                Role = GroupMemberRole.Student
            };

            await _groupMemberRepository.InsertAsync(member);
            await _groupMemberRepository.SaveAsync();

            return true;
        }

        public async Task<bool> LeaveAsync(int groupId, int userId)
        {
            var member = await _groupMemberRepository.GetAsync(groupId, userId);

            if (member == null)
                return false;

            _groupMemberRepository.Delete(member);
            await _groupMemberRepository.SaveAsync();

            return true;
        }

        public async Task<bool> IsMemberAsync(int groupId, int userId)
        {
            return await _groupMemberRepository.IsMemberAsync(groupId, userId);
        }

        public async Task<string?> GetUserRoleAsync(int groupId, int userId)
        {
            var member = await _groupMemberRepository.GetAsync(groupId, userId);

            return member?.Role.ToString();
        }

        public async Task<IEnumerable<int>> GetGroupIdsByUserAsync(int userId)
        {
            var members = await _groupMemberRepository.GetByUserIdAsync(userId);
            return members.Select(m => m.GroupId);
        }

        public async Task<bool> AddStudentAsync(int groupId, int userId)
        {
            if (await _groupMemberRepository.IsMemberAsync(groupId, userId))
                return false;

            await _groupMemberRepository.InsertAsync(new GroupMember
            {
                GroupId = groupId,
                UserId = userId,
                Role = GroupMemberRole.Student
            });

            await _groupMemberRepository.SaveAsync();
            return true;
        }

    }
}
