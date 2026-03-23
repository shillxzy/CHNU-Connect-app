using CHNU_Connect.BLL.DTOs.Subject;
using CHNU_Connect.BLL.Services.Interfaces;
using CHNU_Connect.DAL.Entities;
using CHNU_Connect.DAL.Repositories.Interfaces;
using Mapster;

namespace CHNU_Connect.BLL.Services
{
    public class SubjectService : ISubjectService
    {
        private readonly ISubjectRepository _subjectRepo;
        private readonly IGroupRepository _groupRepo;

        public SubjectService(
            ISubjectRepository subjectRepo,
            IGroupRepository groupRepo)
        {
            _subjectRepo = subjectRepo;
            _groupRepo = groupRepo;
        }

        public async Task<IEnumerable<SubjectDto>> GetByGroupIdAsync(int groupId)
        {
            var subjects = await _subjectRepo.FindAsync(s => s.GroupId == groupId);
            return subjects.Adapt<IEnumerable<SubjectDto>>();
        }

        public async Task<SubjectDto> CreateAsync(CreateSubjectDto dto)
        {
            var group = await _groupRepo.GetByIdAsync(dto.GroupId);
            if (group == null)
                throw new Exception("Group not found");

            var entity = dto.Adapt<Subject>();

            await _subjectRepo.InsertAsync(entity);
            await _subjectRepo.SaveAsync();

            return entity.Adapt<SubjectDto>();
        }

        public async Task<SubjectDto?> UpdateAsync(int id, CreateSubjectDto dto)
        {
            var subject = await _subjectRepo.GetByIdAsync(id);
            if (subject == null) return null;

            dto.Adapt(subject);

            _subjectRepo.Update(subject);
            await _subjectRepo.SaveAsync();

            return subject.Adapt<SubjectDto>();
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var subject = await _subjectRepo.GetByIdAsync(id);
            if (subject == null) return false;

            _subjectRepo.Delete(subject);
            await _subjectRepo.SaveAsync();

            return true;
        }
    }
}
