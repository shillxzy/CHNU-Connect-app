using CHNU_Connect.BLL.DTOs.Schedule;
using CHNU_Connect.BLL.Services.Interfaces;
using CHNU_Connect.DAL.Entities;
using CHNU_Connect.DAL.Repositories.Interfaces;
using Mapster;

namespace CHNU_Connect.BLL.Services
{
    public class ScheduleService : IScheduleService
    {
        private readonly IScheduleRepository _scheduleRepo;
        private readonly IUserRepository _userRepo;

        public ScheduleService(
            IScheduleRepository scheduleRepo,
            IUserRepository userRepo)
        {
            _scheduleRepo = scheduleRepo;
            _userRepo = userRepo;
        }

        // ================= GET BY GROUP =================
        public async Task<IEnumerable<ScheduleDto>> GetByGroupAsync(int groupId)
        {
            var list = await _scheduleRepo.GetByGroupAsync(groupId);
            return list.Adapt<IEnumerable<ScheduleDto>>();
        }

        // ================= GET BY USER =================
        public async Task<IEnumerable<ScheduleDto>> GetByUserAsync(int userId)
        {
            var user = await _userRepo.GetByIdAsync(userId);

            if (user == null)
                return Enumerable.Empty<ScheduleDto>();

            var schedules = await _scheduleRepo.FindAsync(s =>
                s.Group!.Members!.Any(m => m.UserId == userId) &&
                (s.SubGroupId == null || s.SubGroupId == user.SubGroupId)
            );

            return schedules.Adapt<IEnumerable<ScheduleDto>>();
        }

        // ================= CREATE =================
        public async Task<ScheduleDto> CreateAsync(CreateScheduleDto dto)
        {
            var entity = dto.Adapt<Schedule>();

            // 🔥 CONFLICT CHECK
            var conflict = await _scheduleRepo.ExistsConflictAsync(
                entity.GroupId,
                entity.SlotId,
                entity.Day,
                entity.Week,
                entity.SubGroupId
            );

            if (conflict)
                throw new Exception("Schedule conflict detected");

            await _scheduleRepo.InsertAsync(entity);
            await _scheduleRepo.SaveAsync();

            return entity.Adapt<ScheduleDto>();
        }

        // ================= DELETE =================
        public async Task<bool> DeleteAsync(int id)
        {
            var schedule = await _scheduleRepo.GetByIdAsync(id);
            if (schedule == null) return false;

            _scheduleRepo.Delete(schedule);
            await _scheduleRepo.SaveAsync();

            return true;
        }

        // ================= MOVE (DRAG & DROP CORE) =================
        public async Task<bool> MoveAsync(MoveScheduleDto dto)
        {
            var schedule = await _scheduleRepo.GetByIdAsync(dto.ScheduleId);
            if (schedule == null) return false;

            // 🔥 check conflict BEFORE move
            var conflict = await _scheduleRepo.ExistsConflictAsync(
                schedule.GroupId,
                dto.NewSlotId,
                dto.NewDay,
                dto.NewWeek,
                schedule.SubGroupId
            );

            if (conflict)
                return false; // або throw, залежить від UX

            schedule.Day = dto.NewDay;
            schedule.SlotId = dto.NewSlotId;
            schedule.Week = dto.NewWeek;

            _scheduleRepo.Update(schedule);
            await _scheduleRepo.SaveAsync();

            return true;
        }

        // ================= SWAP (якщо слот зайнятий) =================
        public async Task<bool> SwapAsync(int id1, int id2)
        {
            var s1 = await _scheduleRepo.GetByIdAsync(id1);
            var s2 = await _scheduleRepo.GetByIdAsync(id2);

            if (s1 == null || s2 == null)
                return false;

            // swap позицій
            (s1.Day, s2.Day) = (s2.Day, s1.Day);
            (s1.SlotId, s2.SlotId) = (s2.SlotId, s1.SlotId);
            (s1.Week, s2.Week) = (s2.Week, s1.Week);

            _scheduleRepo.Update(s1);
            _scheduleRepo.Update(s2);

            await _scheduleRepo.SaveAsync();

            return true;
        }
    }
}
