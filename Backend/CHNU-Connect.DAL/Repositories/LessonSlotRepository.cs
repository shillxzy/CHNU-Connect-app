using CHNU_Connect.DAL.Data;
using CHNU_Connect.DAL.Entities;
using CHNU_Connect.DAL.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace CHNU_Connect.DAL.Repositories
{
    public class LessonSlotRepository : GenericRepository<LessonSlot>, ILessonSlotRepository
    {
        public LessonSlotRepository(AppDbContext context) : base(context)
        {
        }

        public async Task<IEnumerable<LessonSlot>> GetOrderedAsync()
        {
            return await _dbSet
                .OrderBy(s => s.PairNumber)
                .ToListAsync();
        }
    }
}
