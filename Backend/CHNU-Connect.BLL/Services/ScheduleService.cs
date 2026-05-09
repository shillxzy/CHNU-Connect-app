using CHNU_Connect.BLL.DTOs.Schedule;
using CHNU_Connect.BLL.Services.Interfaces;
using CHNU_Connect.DAL.Entities;
using CHNU_Connect.DAL.Repositories.Interfaces;

namespace CHNU_Connect.BLL.Services
{
    public class ScheduleService : IScheduleService
    {
        private readonly IScheduleRepository _scheduleRepo;
        private readonly IUserRepository _userRepo;
        private readonly ILessonSlotRepository _slotRepo;

        private static readonly (int Pair, string Start, string End)[] DefaultSlots =
        {
            (1, "08:20", "09:40"),
            (2, "09:50", "11:10"),
            (3, "11:30", "12:50"),
            (4, "13:00", "14:20"),
            (5, "14:40", "16:00"),
            (6, "16:10", "17:30"),
            (7, "17:40", "19:00"),
        };

        public ScheduleService(
            IScheduleRepository scheduleRepo,
            IUserRepository userRepo,
            ILessonSlotRepository slotRepo)
        {
            _scheduleRepo = scheduleRepo;
            _userRepo = userRepo;
            _slotRepo = slotRepo;
        }

        public async Task SeedSlotsAsync()
        {
            var existing = await _slotRepo.GetOrderedAsync();
            if (existing.Any()) return;

            foreach (var (pair, start, end) in DefaultSlots)
            {
                await _slotRepo.InsertAsync(new LessonSlot
                {
                    PairNumber = pair,
                    StartTime = TimeSpan.Parse(start),
                    EndTime = TimeSpan.Parse(end),
                });
            }
            await _slotRepo.SaveAsync();
        }

        public async Task<IEnumerable<ScheduleDto>> GetByGroupAsync(int groupId)
        {
            var list = await _scheduleRepo.GetByGroupAsync(groupId);
            return list.Select(MapToDto);
        }

        public async Task<IEnumerable<ScheduleDto>> GetByUserAsync(int userId)
        {
            var user = await _userRepo.GetByIdAsync(userId);
            if (user == null) return Enumerable.Empty<ScheduleDto>();

            var schedules = await _scheduleRepo.FindAsync(s =>
                s.Group!.Members!.Any(m => m.UserId == userId) &&
                (s.SubGroupId == null || s.SubGroupId == user.SubGroupId));

            return schedules.Select(MapToDto);
        }

        public async Task<ScheduleDto> CreateAsync(CreateScheduleDto dto)
        {
            var entity = new Schedule
            {
                GroupId = dto.GroupId,
                SubGroupId = dto.SubGroupId,
                Type = dto.Type,
                Week = dto.Week,
                Day = dto.Day,
                SlotId = dto.SlotId,
                SubjectName = dto.SubjectName ?? string.Empty,
                TeacherName = dto.TeacherName ?? string.Empty,
                Location = dto.Location ?? string.Empty,
            };

            var conflict = await _scheduleRepo.ExistsConflictAsync(
                entity.GroupId, entity.SlotId, entity.Day, entity.Week, entity.SubGroupId);

            if (conflict) throw new Exception("Schedule conflict detected");

            await _scheduleRepo.InsertAsync(entity);
            await _scheduleRepo.SaveAsync();
            return MapToDto(entity);
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var s = await _scheduleRepo.GetByIdAsync(id);
            if (s == null) return false;
            _scheduleRepo.Delete(s);
            await _scheduleRepo.SaveAsync();
            return true;
        }

        public async Task<bool> MoveAsync(MoveScheduleDto dto)
        {
            var s = await _scheduleRepo.GetByIdAsync(dto.ScheduleId);
            if (s == null) return false;

            var conflict = await _scheduleRepo.ExistsConflictAsyncExcludingId(
                s.Id, s.GroupId, dto.NewSlotId, dto.NewDay, dto.NewWeek, s.SubGroupId);

            if (conflict) return false;

            s.Day = dto.NewDay;
            s.SlotId = dto.NewSlotId;
            s.Week = dto.NewWeek;

            _scheduleRepo.Update(s);
            await _scheduleRepo.SaveAsync();
            return true;
        }

        public async Task<bool> SwapAsync(int id1, int id2)
        {
            var s1 = await _scheduleRepo.GetByIdAsync(id1);
            var s2 = await _scheduleRepo.GetByIdAsync(id2);
            if (s1 == null || s2 == null) return false;

            (s1.Day, s2.Day) = (s2.Day, s1.Day);
            (s1.SlotId, s2.SlotId) = (s2.SlotId, s1.SlotId);
            (s1.Week, s2.Week) = (s2.Week, s1.Week);

            _scheduleRepo.Update(s1);
            _scheduleRepo.Update(s2);
            await _scheduleRepo.SaveAsync();
            return true;
        }

        private static ScheduleDto MapToDto(Schedule s) => new()
        {
            Id = s.Id,
            GroupId = s.GroupId,
            SubGroupId = s.SubGroupId,
            SubGroupName = s.SubGroup?.Name,
            Type = s.Type,
            Week = s.Week,
            Day = s.Day,
            SlotId = s.SlotId,
            PairNumber = s.Slot?.PairNumber ?? 0,
            SubjectName = s.SubjectName,
            TeacherName = s.TeacherName,
            Location = s.Location,
        };
    }
}