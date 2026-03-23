using CHNU_Connect.BLL.DTOs.Schedule;

namespace CHNU_Connect.BLL.Services.Interfaces
{
    public interface IScheduleService
    {
        Task<IEnumerable<ScheduleDto>> GetByGroupAsync(int groupId);
        Task<IEnumerable<ScheduleDto>> GetByUserAsync(int userId);
        Task<ScheduleDto> CreateAsync(CreateScheduleDto dto);
        Task<bool> DeleteAsync(int id);
    }
}
