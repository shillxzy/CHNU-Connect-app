using CHNU_Connect.BLL.DTOs.Group;
using CHNU_Connect.BLL.Services.Interfaces;
using CHNU_Connect.DAL.Entities;
using CHNU_Connect.DAL.Repositories.Interfaces;
using Mapster;

namespace CHNU_Connect.BLL.Services
{
    public class GroupService : IGroupService
    {
        private readonly IGroupRepository _groupRepo;
        private readonly IGroupMemberRepository _memberRepo;

        public GroupService(IGroupRepository groupRepo, IGroupMemberRepository memberRepo)
        {
            _groupRepo = groupRepo;
            _memberRepo = memberRepo;
        }

        public async Task<GroupDto> CreateGroupAsync(CreateGroupDto dto)
        {
            var entity = dto.Adapt<Group>();

            await _groupRepo.InsertAsync(entity);
            await _groupRepo.SaveAsync();

            return entity.Adapt<GroupDto>();
        }

        public async Task<GroupDto?> GetByIdAsync(int id)
        {
            var entity = await _groupRepo.GetByIdAsync(id);
            return entity?.Adapt<GroupDto>();
        }

        public async Task<IEnumerable<GroupDto>> GetAllAsync()
        {
            var list = await _groupRepo.GetAllAsync();
            return list.Adapt<IEnumerable<GroupDto>>();
        }

        public async Task<IEnumerable<GroupDto>> GetByCreatorIdAsync(int creatorId)
        {
            var groups = await _groupRepo.GetByCreatorIdAsync(creatorId);
            return groups.Adapt<IEnumerable<GroupDto>>();
        }

        public async Task<IEnumerable<GroupDto>> GetPublicGroupsAsync()
        {
            var groups = await _groupRepo.GetByTypeAsync(GroupType.Announcement);
            return groups.Adapt<IEnumerable<GroupDto>>();
        }

        public async Task<IEnumerable<GroupDto>> GetUserGroupsAsync(int userId)
        {
            var memberships = await _memberRepo.GetByUserIdAsync(userId);

            var groupIds = memberships.Select(m => m.GroupId);

            var groups = await _groupRepo.FindAsync(g => groupIds.Contains(g.Id));

            return groups.Adapt<IEnumerable<GroupDto>>();
        }

        public async Task<GroupDto?> UpdateGroupAsync(int id, CreateGroupDto dto, int userId)
        {
            var group = await _groupRepo.GetByIdAsync(id);
            if (group == null) return null;

            if (group.CreatorId != userId)
                return null;

            dto.Adapt(group);

            _groupRepo.Update(group);
            await _groupRepo.SaveAsync();

            return group.Adapt<GroupDto>();
        }

        public async Task<bool> DeleteGroupAsync(int id, int userId)
        {
            var group = await _groupRepo.GetByIdAsync(id);
            if (group == null) return false;

            if (group.CreatorId != userId)
                return false;

            _groupRepo.Delete(group);
            await _groupRepo.SaveAsync();

            return true;
        }

        public async Task<bool> JoinGroupAsync(int groupId, int userId)
        {
            if (await _memberRepo.IsMemberAsync(groupId, userId))
                return false;

            var member = new GroupMember
            {
                GroupId = groupId,
                UserId = userId,
                Role = GroupMemberRole.Student,
                JoinedAt = DateTime.UtcNow
            };

            await _memberRepo.InsertAsync(member);
            await _memberRepo.SaveAsync();

            return true;
        }

        public async Task<bool> LeaveGroupAsync(int groupId, int userId)
        {
            var member = await _memberRepo.GetAsync(groupId, userId);
            if (member == null) return false;

            _memberRepo.Delete(member);
            await _memberRepo.SaveAsync();

            return true;
        }
    }
}
