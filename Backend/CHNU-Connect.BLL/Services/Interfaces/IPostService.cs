using CHNU_Connect.BLL.DTOs.Post;

namespace CHNU_Connect.BLL.Services.Interfaces
{
    public interface IPostService
    {
        Task<PostDto> CreatePostAsync(CreatePostDto dto, int authorId);

        Task<PostDto?> GetByIdAsync(int id, int? currentUserId);

        Task<IEnumerable<PostDto>> GetAllAsync(int? currentUserId);

        Task<IEnumerable<PostDto>> GetByUserIdAsync(int userId, int? currentUserId);

        Task<PostDto> UpdatePostAsync(int id, UpdatePostDto dto, int? currentUserId);

        Task<bool> DeletePostAsync(int id);

        Task<bool> LikePostAsync(int postId, int userId);

        Task<bool> UnlikePostAsync(int postId, int userId);

        Task<int> GetLikeCountAsync(int postId);

        Task<IEnumerable<PostDto>> GetFeedAsync(int? currentUserId, int? page = 1, int pageSize = 10);

        Task<IEnumerable<PostDto>> SearchPostsAsync(string searchTerm, int? currentUserId);
    }
}
