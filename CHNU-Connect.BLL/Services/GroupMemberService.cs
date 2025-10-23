using CHNU_Connect.BLL.DTOs.GroupMember;
using CHNU_Connect.BLL.Services.Interfaces;
using CHNU_Connect.DAL.Entities;
using CHNU_Connect.DAL.Repositories.Interfaces;
using Mapster;

namespace CHNU_Connect.BLL.Services
{
    public class GroupMemberService : IGroupMemberService
    {
        private readonly IGroupMemberRepository _groupMemberRepository;

        public GroupMemberService(IGroupMemberRepository groupMemberRepository)
        {
            _groupMemberRepository = groupMemberRepository;
        }

        public async Task<GroupMemberDto> CreateGroupMemberAsync(CreateGroupMemberDto dto)
        {
            var member = dto.Adapt<GroupMember>();
            var createdMember = await _groupMemberRepository.AddAsync(member);
            await _groupMemberRepository.SaveChangesAsync();
            return createdMember.Adapt<GroupMemberDto>();
        }

        public async Task<GroupMemberDto?> GetByIdAsync(int id)
        {
            var member = await _groupMemberRepository.GetByIdAsync(id);
            return member?.Adapt<GroupMemberDto>();
        }

        public async Task<IEnumerable<GroupMemberDto>> GetByGroupIdAsync(int groupId)
        {
            var members = await _groupMemberRepository.GetAllAsync();
            var groupMembers = members.Where(m => m.GroupId == groupId);
            return groupMembers.Adapt<IEnumerable<GroupMemberDto>>();
        }

        public async Task<IEnumerable<GroupMemberDto>> GetByUserIdAsync(int userId)
        {
            var members = await _groupMemberRepository.GetAllAsync();
            var userMembers = members.Where(m => m.UserId == userId);
            return userMembers.Adapt<IEnumerable<GroupMemberDto>>();
        }

        public async Task<GroupMemberDto> UpdateGroupMemberAsync(int id, CreateGroupMemberDto dto)
        {
            var member = await _groupMemberRepository.GetByIdAsync(id);
            if (member == null)
                throw new ArgumentException("Group member not found");

            dto.Adapt(member);
            await _groupMemberRepository.UpdateAsync(member);
            await _groupMemberRepository.SaveChangesAsync();
            return member.Adapt<GroupMemberDto>();
        }

        public async Task<bool> DeleteGroupMemberAsync(int id)
        {
            var member = await _groupMemberRepository.GetByIdAsync(id);
            if (member == null)
                return false;

            await _groupMemberRepository.DeleteAsync(member);
            await _groupMemberRepository.SaveChangesAsync();
            return true;
        }

        public async Task<bool> IsUserMemberAsync(int groupId, int userId)
        {
            var members = await _groupMemberRepository.GetAllAsync();
            return members.Any(m => m.GroupId == groupId && m.UserId == userId);
        }

        public async Task<string?> GetUserRoleAsync(int groupId, int userId)
        {
            var members = await _groupMemberRepository.GetAllAsync();
            var member = members.FirstOrDefault(m => m.GroupId == groupId && m.UserId == userId);
            return member?.Role;
        }
    }
}
