using CHNU_Connect.BLL.DTOs.EventParticipant;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CHNU_Connect.BLL.Services.Interfaces
{
    public interface IEventParticipantService
    {
        Task<EventParticipantDto> CreateEventParticipantAsync(CreateEventParticipantDto dto);
        Task<EventParticipantDto?> GetByIdAsync(int id);
        Task<IEnumerable<EventParticipantDto>> GetByEventIdAsync(int eventId);
        Task<IEnumerable<EventParticipantDto>> GetByUserIdAsync(int userId);
        Task<bool> DeleteEventParticipantAsync(int id);
        Task<bool> IsUserParticipantAsync(int eventId, int userId);
    }
}
