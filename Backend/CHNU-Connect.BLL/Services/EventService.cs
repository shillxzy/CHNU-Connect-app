using CHNU_Connect.BLL.DTOs.Event;
using CHNU_Connect.BLL.Services.Interfaces;
using CHNU_Connect.DAL.Entities;
using CHNU_Connect.DAL.Repositories.Interfaces;
using Mapster;

namespace CHNU_Connect.BLL.Services
{
    public class EventService : IEventService
    {
        private readonly IEventRepository _eventRepository;
        private readonly IEventParticipantRepository _eventParticipantRepository;

        public EventService(IEventRepository eventRepository, IEventParticipantRepository eventParticipantRepository)
        {
            _eventRepository = eventRepository;
            _eventParticipantRepository = eventParticipantRepository;
        }

        public async Task<EventDto> CreateEventAsync(CreateEventDto dto)
        {
            var eventEntity = dto.Adapt<Event>();
            await _eventRepository.InsertAsync(eventEntity);
            await _eventRepository.SaveAsync();
            return eventEntity.Adapt<EventDto>();
        }

        public async Task<EventDto?> GetByIdAsync(int id)
        {
            var eventEntity = await _eventRepository.GetByIdAsync(id);
            return eventEntity?.Adapt<EventDto>();
        }

        public async Task<IEnumerable<EventDto>> GetAllAsync()
        {
            var events = await _eventRepository.GetAllAsync();
            return events.Adapt<IEnumerable<EventDto>>();
        }

        public async Task<IEnumerable<EventDto>> GetByCreatorIdAsync(int creatorId)
        {
            var events = await _eventRepository.GetAllAsync();
            var userEvents = events.Where(e => e.CreatorId == creatorId);
            return userEvents.Adapt<IEnumerable<EventDto>>();
        }

        public async Task<IEnumerable<EventDto>> GetPublicEventsAsync()
        {
            var events = await _eventRepository.GetAllAsync();
            var publicEvents = events.Where(e => e.IsPublic);
            return publicEvents.Adapt<IEnumerable<EventDto>>();
        }

        public async Task<EventDto> UpdateEventAsync(int id, CreateEventDto dto)
        {
            var eventEntity = await _eventRepository.GetByIdAsync(id);
            if (eventEntity == null)
                throw new ArgumentException("Event not found");

            dto.Adapt(eventEntity);
            _eventRepository.Update(eventEntity);
            await _eventRepository.SaveAsync();
            return eventEntity.Adapt<EventDto>();
        }

        public async Task<bool> DeleteEventAsync(int id)
        {
            var eventEntity = await _eventRepository.GetByIdAsync(id);
            if (eventEntity == null)
                return false;

            _eventRepository.Delete(eventEntity);
            await _eventRepository.SaveAsync();
            return true;
        }

        public async Task<bool> JoinEventAsync(int eventId, int userId)
        {
            var existingParticipant = await _eventParticipantRepository.GetAllAsync();
            var participant = existingParticipant.FirstOrDefault(p => p.EventId == eventId && p.UserId == userId);
            
            if (participant != null)
                return false; // Already joined

            var newParticipant = new EventParticipant
            {
                EventId = eventId,
                UserId = userId,
                JoinedAt = DateTime.UtcNow
            };

            await _eventParticipantRepository.InsertAsync(newParticipant);
            await _eventParticipantRepository.SaveAsync();
            return true;
        }

        public async Task<bool> LeaveEventAsync(int eventId, int userId)
        {
            var existingParticipant = await _eventParticipantRepository.GetAllAsync();
            var participant = existingParticipant.FirstOrDefault(p => p.EventId == eventId && p.UserId == userId);
            
            if (participant == null)
                return false; // Not joined

            _eventParticipantRepository.Delete(participant);
            await _eventParticipantRepository.SaveAsync();
            return true;
        }

        public async Task<IEnumerable<EventDto>> GetUserEventsAsync(int userId)
        {
            var participants = await _eventParticipantRepository.GetAllAsync();
            var userEventIds = participants.Where(p => p.UserId == userId).Select(p => p.EventId);
            
            var events = await _eventRepository.GetAllAsync();
            var userEvents = events.Where(e => userEventIds.Contains(e.Id));
            return userEvents.Adapt<IEnumerable<EventDto>>();
        }
    }
}
