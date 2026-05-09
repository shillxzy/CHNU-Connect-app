using CHNU_Connect.DAL.Data;
using CHNU_Connect.DAL.Entities;
using CHNU_Connect.DAL.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace CHNU_Connect.DAL.Repositories
{
    public class ScheduleRepository : GenericRepository<Schedule>, IScheduleRepository
    {
        public ScheduleRepository(AppDbContext context) : base(context)
        {
        }

        public async Task<IEnumerable<Schedule>> GetByGroupAsync(int groupId)
        {
            return await _dbSet
                .Include(s => s.Subject)
                .Include(s => s.SubGroup)
                .Include(s => s.Slot)
                .Where(s => s.GroupId == groupId)
                .ToListAsync();
        }

        public async Task<IEnumerable<Schedule>> GetByGroupAndWeekAsync(int groupId, WeekType week)
        {
            return await _dbSet
                .Include(s => s.Subject)
                .Include(s => s.SubGroup)
                .Include(s => s.Slot)
                .Where(s => s.GroupId == groupId && s.Week == week)
                .ToListAsync();
        }

        public async Task<IEnumerable<Schedule>> GetByGroupDaySlotAsync(
            int groupId,
            DayOfWeek day,
            int slotId,
            WeekType week)
        {
            return await _dbSet
                .Where(s =>
                    s.GroupId == groupId &&
                    s.Day == day &&
                    s.SlotId == slotId &&
                    s.Week == week)
                .ToListAsync();
        }

        public async Task<bool> ExistsConflictAsync(
    int groupId,
    int slotId,
    DayOfWeek day,
    WeekType week,
    int? subGroupId)
        {
            return await _dbSet.AnyAsync(s =>
                s.GroupId == groupId &&
                s.SlotId == slotId &&
                s.Day == day &&
                s.Week == week &&
                (
                    // 🔥 ЛЕКЦІЯ
                    (s.SubGroupId == null && subGroupId == null)

                    // 🔥 ПРАКТИКА
                    || (s.SubGroupId != null && subGroupId != null && s.SubGroupId == subGroupId)
                )
            );
        }


        public async Task<bool> ExistsConflictAsyncExcludingId(
    int id,
    int groupId,
    int slotId,
    DayOfWeek day,
    WeekType week,
    int? subGroupId)
        {
            return await _dbSet.AnyAsync(s =>
                s.Id != id &&
                s.GroupId == groupId &&
                s.SlotId == slotId &&
                s.Day == day &&
                s.Week == week &&
                (
                    (s.SubGroupId == null && subGroupId == null)
                    || (s.SubGroupId != null && subGroupId != null && s.SubGroupId == subGroupId)
                )
            );
        }


    }
}
