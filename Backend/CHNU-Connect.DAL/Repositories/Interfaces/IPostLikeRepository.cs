using CHNU_Connect.DAL.Entities;

namespace CHNU_Connect.DAL.Repositories.Interfaces
{
    public interface IPostLikeRepository : IGenericRepository<PostLike>
    {
        Task<IEnumerable<PostLike>> GetLikesByPostIdAsync(int postId);

        Task<int> GetLikesCountByPostIdAsync(int postId);

        Task<PostLike?> GetPostLikeAsync(int postId, int userId);

        Task<IEnumerable<PostLike>> GetLikesByUserIdAsync(int userId);

        Task<bool> IsPostLikedByUserAsync(int userId, int postId);
    }
}
