using CHNU_Connect.BLL.DTOs.Post;
using CHNU_Connect.BLL.Services.Interfaces;
using CHNU_Connect.DAL.Entities;
using CHNU_Connect.DAL.Repositories.Interfaces;
using Mapster;

namespace CHNU_Connect.BLL.Services
{
    public class PostService : IPostService
    {
        private readonly IPostRepository _postRepository;
        private readonly IPostLikeRepository _postLikeRepository;
        private readonly IUserRepository _userRepository;

        public PostService(
            IPostRepository postRepository,
            IPostLikeRepository postLikeRepository,
            IUserRepository userRepository)
        {
            _postRepository = postRepository;
            _postLikeRepository = postLikeRepository;
            _userRepository = userRepository;
        }

        public async Task<PostDto> CreatePostAsync(CreatePostDto dto, int authorId)
        {
            var post = dto.Adapt<Post>();
            post.UserId = authorId;
            post.CreatedAt = DateTime.UtcNow;

            await _postRepository.InsertAsync(post);
            await _postRepository.SaveAsync();

            return await BuildPostDtoAsync(post, authorId);
        }

        public async Task<PostDto?> GetByIdAsync(int id, int? currentUserId)
        {
            var post = await _postRepository.GetByIdAsync(id);
            if (post == null) return null;

            return await BuildPostDtoAsync(post, currentUserId);
        }

        public async Task<IEnumerable<PostDto>> GetAllAsync(int? currentUserId)
        {
            var posts = await _postRepository.GetAllAsync();
            var postDtos = new List<PostDto>();

            foreach (var post in posts.OrderByDescending(p => p.CreatedAt))
            {
                var dto = await BuildPostDtoAsync(post, currentUserId);
                postDtos.Add(dto);
            }

            return postDtos;
        }

        public async Task<IEnumerable<PostDto>> GetByUserIdAsync(int userId, int? currentUserId)
        {
            var posts = await _postRepository.GetPostsByUserIdAsync(userId);
            var postDtos = new List<PostDto>();

            foreach (var post in posts.OrderByDescending(p => p.CreatedAt))
            {
                var dto = await BuildPostDtoAsync(post, currentUserId);
                postDtos.Add(dto);
            }

            return postDtos;
        }

        public async Task<PostDto> UpdatePostAsync(int id, UpdatePostDto dto, int? currentUserId)
        {
            var post = await _postRepository.GetByIdAsync(id);
            if (post == null)
                throw new ArgumentException("Post not found");

            dto.Adapt(post);

            _postRepository.Update(post);
            await _postRepository.SaveAsync();

            return await BuildPostDtoAsync(post, currentUserId);
        }

        public async Task<bool> DeletePostAsync(int id)
        {
            var post = await _postRepository.GetByIdAsync(id);
            if (post == null) return false;

            _postRepository.Delete(post);
            await _postRepository.SaveAsync();

            return true;
        }

        public async Task<bool> LikePostAsync(int postId, int userId)
        {
            bool alreadyLiked = await _postLikeRepository.IsPostLikedByUserAsync(userId, postId);
            if (alreadyLiked) return false;

            var like = new PostLike
            {
                PostId = postId,
                UserId = userId,
                CreatedAt = DateTime.UtcNow
            };

            await _postLikeRepository.InsertAsync(like);
            await _postLikeRepository.SaveAsync();

            return true;
        }

        public async Task<bool> UnlikePostAsync(int postId, int userId)
        {
            var like = await _postLikeRepository.GetPostLikeAsync(userId, postId);
            if (like == null) return false;

            _postLikeRepository.Delete(like);
            await _postLikeRepository.SaveAsync();

            return true;
        }

        public async Task<int> GetLikeCountAsync(int postId)
        {
            return await _postLikeRepository.GetLikesCountByPostIdAsync(postId);
        }

        public async Task<IEnumerable<PostDto>> GetFeedAsync(int? currentUserId, int? page = 1, int pageSize = 10)
        {
            var allPosts = await _postRepository.GetAllAsync();

            var pagedPosts = allPosts
                .OrderByDescending(p => p.CreatedAt)
                .Skip(((page ?? 1) - 1) * pageSize)
                .Take(pageSize);

            var postDtos = new List<PostDto>();

            foreach (var post in pagedPosts)
            {
                var dto = await BuildPostDtoAsync(post, currentUserId);
                postDtos.Add(dto);
            }

            return postDtos;
        }

        public async Task<IEnumerable<PostDto>> SearchPostsAsync(string searchTerm, int? currentUserId)
        {
            var posts = await _postRepository.GetAllAsync();

            var filtered = posts
                .Where(p => p.Content.Contains(searchTerm, StringComparison.OrdinalIgnoreCase));

            var postDtos = new List<PostDto>();

            foreach (var post in filtered.OrderByDescending(p => p.CreatedAt))
            {
                var dto = await BuildPostDtoAsync(post, currentUserId);
                postDtos.Add(dto);
            }

            return postDtos;
        }

        private async Task<PostDto> BuildPostDtoAsync(Post post, int? currentUserId)
        {
            var dto = post.Adapt<PostDto>();

            var user = await _userRepository.GetByIdAsync(post.UserId);

            dto.AuthorName = user?.FullName ?? "Unknown";
            dto.AuthorAvatar = user?.PhotoUrl ?? "/images/default-avatar-icon.png";

            dto.LikeCount = await _postLikeRepository.GetLikesCountByPostIdAsync(post.Id);

            dto.HasCurrentUserLiked = currentUserId != null &&
                await _postLikeRepository.IsPostLikedByUserAsync(currentUserId.Value, post.Id);

            return dto;
        }
    }
}
