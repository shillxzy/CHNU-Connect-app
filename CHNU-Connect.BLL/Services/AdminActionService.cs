using CHNU_Connect.BLL.DTOs.AdminAction;
using CHNU_Connect.BLL.Services.Interfaces;
using CHNU_Connect.DAL.Entities;
using CHNU_Connect.DAL.Repositories.Interfaces;
using Mapster;

namespace CHNU_Connect.BLL.Services
{
    public class AdminActionService : IAdminActionService
    {
        private readonly IAdminActionRepository _adminActionRepository;

        public AdminActionService(IAdminActionRepository adminActionRepository)
        {
            _adminActionRepository = adminActionRepository;
        }

        public async Task<AdminActionDto> CreateAdminActionAsync(CreateAdminActionDto dto)
        {
            var action = dto.Adapt<AdminAction>();
            await _adminActionRepository.InsertAsync(action);
            await _adminActionRepository.SaveAsync();
            return action.Adapt<AdminActionDto>();
        }

        public async Task<AdminActionDto?> GetByIdAsync(int id)
        {
            var action = await _adminActionRepository.GetByIdAsync(id);
            return action?.Adapt<AdminActionDto>();
        }

        public async Task<IEnumerable<AdminActionDto>> GetAllAsync()
        {
            var actions = await _adminActionRepository.GetAllAsync();
            return actions.Adapt<IEnumerable<AdminActionDto>>();
        }

        public async Task<IEnumerable<AdminActionDto>> GetByAdminIdAsync(int adminId)
        {
            var actions = await _adminActionRepository.GetAllAsync();
            var adminActions = actions.Where(a => a.AdminId == adminId);
            return adminActions.Adapt<IEnumerable<AdminActionDto>>();
        }

        public async Task<IEnumerable<AdminActionDto>> GetByTargetUserIdAsync(int targetUserId)
        {
            var actions = await _adminActionRepository.GetAllAsync();
            var targetActions = actions.Where(a => a.TargetUserId == targetUserId);
            return targetActions.Adapt<IEnumerable<AdminActionDto>>();
        }

        public async Task<bool> DeleteAdminActionAsync(int id)
        {
            var action = await _adminActionRepository.GetByIdAsync(id);
            if (action == null)
                return false;

            _adminActionRepository.Delete(action);
            await _adminActionRepository.SaveAsync();
            return true;
        }
    }
}
