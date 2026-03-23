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
        private readonly IGroupMemberRepository _memberRepo;
        private readonly IUserRepository _userRepo;

        public ScheduleService(
            IScheduleRepository scheduleRepo,
            IGroupMemberRepository memberRepo,
            IUserRepository userRepo)
        {
            _scheduleRepo = scheduleRepo;
            _memberRepo = memberRepo;
            _userRepo = userRepo;
        }

        public async Task<IEnumerable<ScheduleDto>> GetByGroupAsync(int groupId)
        {
            var list = await _scheduleRepo.FindAsync(s => s.GroupId == groupId);
            return list.Adapt<IEnumerable<ScheduleDto>>();
        }

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

        public async Task<ScheduleDto> CreateAsync(CreateScheduleDto dto)
        {
            var entity = dto.Adapt<Schedule>();

            await _scheduleRepo.InsertAsync(entity);
            await _scheduleRepo.SaveAsync();

            return entity.Adapt<ScheduleDto>();
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var schedule = await _scheduleRepo.GetByIdAsync(id);
            if (schedule == null) return false;

            _scheduleRepo.Delete(schedule);
            await _scheduleRepo.SaveAsync();

            return true;
        }
    }
}
