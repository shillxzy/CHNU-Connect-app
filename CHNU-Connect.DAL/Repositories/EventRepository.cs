using CHNU_Connect.DAL.Data;
using CHNU_Connect.DAL.Entities;
using CHNU_Connect.DAL.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace CHNU_Connect.DAL.Repositories
{
    public class EventRepository : GenericRepository<Event>, IEventRepository
    {
        public EventRepository(AppDbContext context) : base(context)
        {
        }

        public async Task<IEnumerable<Event>> GetEventsByCreatorIdAsync(int creatorId)
        {
            return await _dbSet.Where(e => e.CreatorId == creatorId)
                              .OrderByDescending(e => e.CreatedAt)
                              .ToListAsync();
        }

        public async Task<IEnumerable<Event>> GetUpcomingEventsAsync()
        {
            var now = DateTime.UtcNow;
            return await _dbSet.Where(e => e.Date > now)
                              .OrderBy(e => e.Date)
                              .ToListAsync();
        }

        public async Task<IEnumerable<Event>> GetEventsByDateRangeAsync(DateTime startDate, DateTime endDate)
        {
            return await _dbSet.Where(e => e.Date >= startDate && e.Date <= endDate)
                              .OrderBy(e => e.Date)
                              .ToListAsync();
        }

        public async Task<int> GetEventParticipantsCountAsync(int eventId)
        {
            return await _context.EventParticipants.CountAsync(ep => ep.EventId == eventId);
        }

        public async Task<bool> IsUserParticipatingAsync(int eventId, int userId)
        {
            return await _context.EventParticipants.AnyAsync(ep => ep.EventId == eventId && ep.UserId == userId);
        }
    }
}
