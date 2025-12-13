using CHNU_Connect.DAL.Entities;

namespace CHNU_Connect.DAL.Repositories.Interfaces
{
    public interface IPostRepository : IGenericRepository<Post>
    {
        Task<IEnumerable<Post>> GetPostsByUserIdAsync(int userId);
        Task<IEnumerable<Post>> GetPostsByGroupIdAsync(int groupId);
        Task<IEnumerable<Post>> GetRecentPostsAsync(int count);
        Task<int> GetPostLikesCountAsync(int postId);
        Task<bool> IsPostLikedByUserAsync(int postId, int userId);
    }
}
