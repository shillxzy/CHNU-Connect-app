using CHNU_Connect.BLL.DTOs.AdminAction;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CHNU_Connect.BLL.Services.Interfaces
{
    public interface IAdminActionService
    {
        Task<AdminActionDto> CreateAdminActionAsync(CreateAdminActionDto dto);
        Task<AdminActionDto?> GetByIdAsync(int id);
        Task<IEnumerable<AdminActionDto>> GetAllAsync();
        Task<IEnumerable<AdminActionDto>> GetByAdminIdAsync(int adminId);
        Task<IEnumerable<AdminActionDto>> GetByTargetUserIdAsync(int targetUserId);
        Task<bool> DeleteAdminActionAsync(int id);
    }
}
