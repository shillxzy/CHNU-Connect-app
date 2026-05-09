using CHNU_Connect.BLL.DTOs.SubGroup;
using CHNU_Connect.BLL.Services.Interfaces;
using CHNU_Connect.DAL.Entities;
using CHNU_Connect.DAL.Repositories.Interfaces;

namespace CHNU_Connect.BLL.Services
{
    public class SubGroupService : ISubGroupService
    {
        private readonly ISubGroupRepository _repo;

        public SubGroupService(ISubGroupRepository repo)
        {
            _repo = repo;
        }

        public async Task<IEnumerable<SubGroupDto>> GetByGroupAsync(int groupId)
        {
            var list = await _repo.GetByGroupIdAsync(groupId);
            return list.Select(sg => new SubGroupDto { Id = sg.Id, Name = sg.Name });
        }

        public async Task<SubGroupDto> CreateAsync(int groupId, string name)
        {
            var entity = new SubGroup { GroupId = groupId, Name = name };
            await _repo.InsertAsync(entity);
            await _repo.SaveAsync();
            return new SubGroupDto { Id = entity.Id, Name = entity.Name };
        }

        public async Task<SubGroupDto?> UpdateAsync(int id, string name)
        {
            var entity = await _repo.GetByIdAsync(id);
            if (entity == null) return null;
            entity.Name = name;
            _repo.Update(entity);
            await _repo.SaveAsync();
            return new SubGroupDto { Id = entity.Id, Name = entity.Name };
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var entity = await _repo.GetByIdAsync(id);
            if (entity == null) return false;
            _repo.Delete(entity);
            await _repo.SaveAsync();
            return true;
        }
    }
}