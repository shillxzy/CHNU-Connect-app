using CHNU_Connect.DAL.Entities;

namespace CHNU_Connect.DAL.Repositories.Interfaces
{
    public interface ILessonSlotRepository : IGenericRepository<LessonSlot>
    {
        Task<IEnumerable<LessonSlot>> GetOrderedAsync();
    }
}
