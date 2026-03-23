using CHNU_Connect.BLL.DTOs.Group;
using CHNU_Connect.BLL.DTOs.User;
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
        private readonly IUserRepository _userRepo;


        public GroupService(
     IGroupRepository groupRepo,
     IGroupMemberRepository memberRepo,
     IUserRepository userRepo)
        {
            _groupRepo = groupRepo;
            _memberRepo = memberRepo;
            _userRepo = userRepo;
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
            var group = await _groupRepo.GetByIdAsync(id);
            if (group == null) return null;

            var dto = group.Adapt<GroupDto>();

            // 👇 curator
            if (group.CuratorId != null)
            {
                var curator = await _userRepo.GetByIdAsync(group.CuratorId.Value);
                dto.Curator = curator?.Adapt<UserDto>();
            }

            // 👇 users
            var members = await _memberRepo.GetByGroupIdAsync(id);

            var users = new List<UserDto>();

            foreach (var m in members)
            {
                var user = await _userRepo.GetByIdAsync(m.UserId);
                if (user != null)
                    users.Add(user.Adapt<UserDto>());
            }

            dto.Users = users;

            return dto;
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

            group.Name = dto.Name;
            group.Description = dto.Description;

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


        public async Task<bool> AssignCuratorAsync(int groupId, int curatorId, int currentUserId)
        {
            var group = await _groupRepo.GetByIdAsync(groupId);
            if (group == null) return false;

            // тільки creator може призначати куратора
            if (group.CreatorId != currentUserId)
                return false;

            group.CuratorId = curatorId;

            _groupRepo.Update(group);
            await _groupRepo.SaveAsync();

            return true;
        }

        public async Task<bool> CanAccessGroupAsync(int groupId, int userId)
        {
            return await _memberRepo.IsMemberAsync(groupId, userId);
        }


    }
}
