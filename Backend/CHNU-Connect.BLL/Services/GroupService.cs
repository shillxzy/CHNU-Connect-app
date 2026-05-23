using CHNU_Connect.BLL.DTOs.Group;
using CHNU_Connect.BLL.DTOs.SubGroup;
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
        private readonly ISubGroupRepository _subGroupRepo;



        public GroupService(
    IGroupRepository groupRepo,
    IGroupMemberRepository memberRepo,
    IUserRepository userRepo,
    ISubGroupRepository subGroupRepo)
        {
            _groupRepo = groupRepo;
            _memberRepo = memberRepo;
            _userRepo = userRepo;
            _subGroupRepo = subGroupRepo;
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

            // 👇 SUBGROUPS (ОЦЕ ТИ ДОДАЄШ)
            var subGroups = await _subGroupRepo.GetByGroupIdAsync(id);
            dto.SubGroups = subGroups.Adapt<List<SubGroupDto>>();

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
            var ids = list.Select(g => g.Id).ToList();
            var groups = (await _groupRepo.GetWithCuratorByIdsAsync(ids)).ToList();

            return groups.Select(g =>
            {
                var dto = g.Adapt<GroupDto>();
                if (g.Curator != null)
                    dto.Curator = g.Curator.Adapt<UserDto>();
                return dto;
            });
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
            var groupIds = memberships.Select(m => m.GroupId).ToList();
            var groups = (await _groupRepo.GetWithCuratorByIdsAsync(groupIds)).ToList();

            return groups.Select(g =>
            {
                var dto = g.Adapt<GroupDto>();
                if (g.Curator != null)
                    dto.Curator = g.Curator.Adapt<UserDto>();
                return dto;
            });
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

        public async Task<IEnumerable<GroupDto>> GetCuratedGroupsAsync(int userId)
        {
            var groups = await _groupRepo.FindAsync(g => g.CuratorId == userId);
            return groups.Adapt<IEnumerable<GroupDto>>();
        }

        public async Task<bool> IsCuratorAsync(int groupId, int userId)
        {
            var group = await _groupRepo.GetByIdAsync(groupId);
            return group?.CuratorId == userId;
        }

    }
}
