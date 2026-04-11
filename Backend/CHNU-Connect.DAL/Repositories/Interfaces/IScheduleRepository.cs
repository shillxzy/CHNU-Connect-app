using CHNU_Connect.DAL.Entities;

namespace CHNU_Connect.DAL.Repositories.Interfaces
{
    public interface IScheduleRepository : IGenericRepository<Schedule>
    {
        Task<IEnumerable<Schedule>> GetByGroupAsync(int groupId);

        Task<IEnumerable<Schedule>> GetByGroupAndWeekAsync(int groupId, WeekType week);

        Task<IEnumerable<Schedule>> GetByGroupDaySlotAsync(
            int groupId,
            DayOfWeek day,
            int slotId,
            WeekType week);

        Task<bool> ExistsConflictAsync(
            int groupId,
            int slotId,
            DayOfWeek day,
            WeekType week,
            int? subGroupId);
    }
}
