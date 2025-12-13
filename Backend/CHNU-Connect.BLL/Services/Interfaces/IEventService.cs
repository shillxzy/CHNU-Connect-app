using CHNU_Connect.BLL.DTOs.Event;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CHNU_Connect.BLL.Services.Interfaces
{
    public interface IEventService
    {
        Task<EventDto> CreateEventAsync(CreateEventDto dto);
        Task<EventDto?> GetByIdAsync(int id);
        Task<IEnumerable<EventDto>> GetAllAsync();
        Task<IEnumerable<EventDto>> GetByCreatorIdAsync(int creatorId);
        Task<IEnumerable<EventDto>> GetPublicEventsAsync();
        Task<EventDto> UpdateEventAsync(int id, CreateEventDto dto);
        Task<bool> DeleteEventAsync(int id);
        Task<bool> JoinEventAsync(int eventId, int userId);
        Task<bool> LeaveEventAsync(int eventId, int userId);
        Task<IEnumerable<EventDto>> GetUserEventsAsync(int userId);
    }
}
