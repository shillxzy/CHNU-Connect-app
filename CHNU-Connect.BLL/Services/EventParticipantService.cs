using CHNU_Connect.BLL.DTOs.EventParticipant;
using CHNU_Connect.BLL.Services.Interfaces;
using CHNU_Connect.DAL.Entities;
using CHNU_Connect.DAL.Repositories.Interfaces;
using Mapster;

namespace CHNU_Connect.BLL.Services
{
    public class EventParticipantService : IEventParticipantService
    {
        private readonly IEventParticipantRepository _eventParticipantRepository;

        public EventParticipantService(IEventParticipantRepository eventParticipantRepository)
        {
            _eventParticipantRepository = eventParticipantRepository;
        }

        public async Task<EventParticipantDto> CreateEventParticipantAsync(CreateEventParticipantDto dto)
        {
            var participant = dto.Adapt<EventParticipant>();
            var createdParticipant = await _eventParticipantRepository.AddAsync(participant);
            await _eventParticipantRepository.SaveChangesAsync();
            return createdParticipant.Adapt<EventParticipantDto>();
        }

        public async Task<EventParticipantDto?> GetByIdAsync(int id)
        {
            var participant = await _eventParticipantRepository.GetByIdAsync(id);
            return participant?.Adapt<EventParticipantDto>();
        }

        public async Task<IEnumerable<EventParticipantDto>> GetByEventIdAsync(int eventId)
        {
            var participants = await _eventParticipantRepository.GetAllAsync();
            var eventParticipants = participants.Where(p => p.EventId == eventId);
            return eventParticipants.Adapt<IEnumerable<EventParticipantDto>>();
        }

        public async Task<IEnumerable<EventParticipantDto>> GetByUserIdAsync(int userId)
        {
            var participants = await _eventParticipantRepository.GetAllAsync();
            var userParticipants = participants.Where(p => p.UserId == userId);
            return userParticipants.Adapt<IEnumerable<EventParticipantDto>>();
        }

        public async Task<bool> DeleteEventParticipantAsync(int id)
        {
            var participant = await _eventParticipantRepository.GetByIdAsync(id);
            if (participant == null)
                return false;

            await _eventParticipantRepository.DeleteAsync(participant);
            await _eventParticipantRepository.SaveChangesAsync();
            return true;
        }

        public async Task<bool> IsUserParticipantAsync(int eventId, int userId)
        {
            var participants = await _eventParticipantRepository.GetAllAsync();
            return participants.Any(p => p.EventId == eventId && p.UserId == userId);
        }
    }
}
