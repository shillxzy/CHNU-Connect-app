using CHNU_Connect.DAL.Data;
using CHNU_Connect.DAL.Entities;
using CHNU_Connect.DAL.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace CHNU_Connect.DAL.Repositories
{
    public class EventParticipantRepository : GenericRepository<EventParticipant>, IEventParticipantRepository
    {
        public EventParticipantRepository(AppDbContext context) : base(context)
        {
        }

        public async Task<IEnumerable<EventParticipant>> GetParticipantsByEventIdAsync(int eventId)
        {
            return await _dbSet.Where(ep => ep.EventId == eventId)
                              .ToListAsync();
        }

        public async Task<IEnumerable<EventParticipant>> GetEventsByUserIdAsync(int userId)
        {
            return await _dbSet.Where(ep => ep.UserId == userId)
                              .ToListAsync();
        }

        public async Task<bool> IsUserParticipatingInEventAsync(int userId, int eventId)
        {
            return await _dbSet.AnyAsync(ep => ep.UserId == userId && ep.EventId == eventId);
        }

        public async Task<int> GetParticipantsCountByEventIdAsync(int eventId)
        {
            return await _dbSet.CountAsync(ep => ep.EventId == eventId);
        }

        public async Task<EventParticipant?> GetParticipantAsync(int userId, int eventId)
        {
            return await _dbSet.FirstOrDefaultAsync(ep => ep.UserId == userId && ep.EventId == eventId);
        }
    }
}
