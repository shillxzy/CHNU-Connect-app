using CHNU_Connect.DAL.Entities;

namespace CHNU_Connect.DAL.Repositories.Interfaces
{
    public interface IEventParticipantRepository : IGenericRepository<EventParticipant>
    {
        Task<IEnumerable<EventParticipant>> GetParticipantsByEventIdAsync(int eventId);
        Task<IEnumerable<EventParticipant>> GetEventsByUserIdAsync(int userId);
        Task<bool> IsUserParticipatingInEventAsync(int userId, int eventId);
        Task<int> GetParticipantsCountByEventIdAsync(int eventId);
        Task<EventParticipant?> GetParticipantAsync(int userId, int eventId);
    }
}
