using CHNU_Connect.DAL.Data;
using CHNU_Connect.DAL.Entities;
using CHNU_Connect.DAL.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace CHNU_Connect.DAL.Repositories
{
    public class SubjectRepository : GenericRepository<Subject>, ISubjectRepository
    {
        public SubjectRepository(AppDbContext context) : base(context)
        {
        }

        public async Task<IEnumerable<Subject>> GetByGroupIdAsync(int groupId)
        {
            return await _dbSet
                .Where(s => s.GroupId == groupId)
                .ToListAsync();
        }
    }
}
