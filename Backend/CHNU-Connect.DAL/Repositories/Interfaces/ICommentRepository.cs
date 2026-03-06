using CHNU_Connect.DAL.Entities;

namespace CHNU_Connect.DAL.Repositories.Interfaces
{
    public interface ICommentRepository : IGenericRepository<Comment>
    {
        Task<IEnumerable<Comment>> GetCommentsByPostIdAsync(int postId);
        Task<IEnumerable<Comment>> GetCommentsByUserIdAsync(int userId);
        Task<IEnumerable<Comment>> GetRecentCommentsAsync(int count);
        Task<int> GetCommentsCountByPostIdAsync(int postId);
    }
}
