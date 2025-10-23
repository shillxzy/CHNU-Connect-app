using CHNU_Connect.BLL.DTOs.Group;
using CHNU_Connect.BLL.Services.Interfaces;
using CHNU_Connect.DAL.Entities;
using CHNU_Connect.DAL.Repositories.Interfaces;
using Mapster;

namespace CHNU_Connect.BLL.Services
{
    public class GroupService : IGroupService
    {
        private readonly IGroupRepository _groupRepository;
        private readonly IGroupMemberRepository _groupMemberRepository;

        public GroupService(IGroupRepository groupRepository, IGroupMemberRepository groupMemberRepository)
        {
            _groupRepository = groupRepository;
            _groupMemberRepository = groupMemberRepository;
        }

        public async Task<GroupDto> CreateGroupAsync(CreateGroupDto dto)
        {
            var group = dto.Adapt<Group>();
            var createdGroup = await _groupRepository.AddAsync(group);
            await _groupRepository.SaveChangesAsync();
            return createdGroup.Adapt<GroupDto>();
        }

        public async Task<GroupDto?> GetByIdAsync(int id)
        {
            var group = await _groupRepository.GetByIdAsync(id);
            return group?.Adapt<GroupDto>();
        }

        public async Task<IEnumerable<GroupDto>> GetAllAsync()
        {
            var groups = await _groupRepository.GetAllAsync();
            return groups.Adapt<IEnumerable<GroupDto>>();
        }

        public async Task<IEnumerable<GroupDto>> GetByCreatorIdAsync(int creatorId)
        {
            var groups = await _groupRepository.GetAllAsync();
            var userGroups = groups.Where(g => g.CreatorId == creatorId);
            return userGroups.Adapt<IEnumerable<GroupDto>>();
        }

        public async Task<IEnumerable<GroupDto>> GetPublicGroupsAsync()
        {
            var groups = await _groupRepository.GetAllAsync();
            var publicGroups = groups.Where(g => g.IsPublic);
            return publicGroups.Adapt<IEnumerable<GroupDto>>();
        }

        public async Task<GroupDto> UpdateGroupAsync(int id, CreateGroupDto dto)
        {
            var group = await _groupRepository.GetByIdAsync(id);
            if (group == null)
                throw new ArgumentException("Group not found");

            dto.Adapt(group);
            await _groupRepository.UpdateAsync(group);
            await _groupRepository.SaveChangesAsync();
            return group.Adapt<GroupDto>();
        }

        public async Task<bool> DeleteGroupAsync(int id)
        {
            var group = await _groupRepository.GetByIdAsync(id);
            if (group == null)
                return false;

            await _groupRepository.DeleteAsync(group);
            await _groupRepository.SaveChangesAsync();
            return true;
        }

        public async Task<bool> JoinGroupAsync(int groupId, int userId)
        {
            var existingMember = await _groupMemberRepository.GetAllAsync();
            var member = existingMember.FirstOrDefault(m => m.GroupId == groupId && m.UserId == userId);
            
            if (member != null)
                return false; // Already joined

            var newMember = new GroupMember
            {
                GroupId = groupId,
                UserId = userId,
                Role = "member",
                JoinedAt = DateTime.UtcNow
            };

            await _groupMemberRepository.AddAsync(newMember);
            await _groupMemberRepository.SaveChangesAsync();
            return true;
        }

        public async Task<bool> LeaveGroupAsync(int groupId, int userId)
        {
            var existingMember = await _groupMemberRepository.GetAllAsync();
            var member = existingMember.FirstOrDefault(m => m.GroupId == groupId && m.UserId == userId);
            
            if (member == null)
                return false; // Not a member

            await _groupMemberRepository.DeleteAsync(member);
            await _groupMemberRepository.SaveChangesAsync();
            return true;
        }

        public async Task<IEnumerable<GroupDto>> GetUserGroupsAsync(int userId)
        {
            var members = await _groupMemberRepository.GetAllAsync();
            var userGroupIds = members.Where(m => m.UserId == userId).Select(m => m.GroupId);
            
            var groups = await _groupRepository.GetAllAsync();
            var userGroups = groups.Where(g => userGroupIds.Contains(g.Id));
            return userGroups.Adapt<IEnumerable<GroupDto>>();
        }
    }
}
