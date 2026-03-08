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

        public PostService(IPostRepository postRepository, IPostLikeRepository postLikeRepository, IUserRepository userRepository)
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

            // Додати дані автора перед поверненням
            var user = await _userRepository.GetByIdAsync(authorId);
            var postDto = post.Adapt<PostDto>();
            if (user != null)
            {
                postDto.AuthorName = user.FullName;
                postDto.AuthorAvatar = user.PhotoUrl;
            }

            return postDto;
        }

        public async Task<PostDto?> GetByIdAsync(int id)
        {
            var post = await _postRepository.GetByIdAsync(id);
            if (post == null) return null;

            var postDto = post.Adapt<PostDto>();
            var user = await _userRepository.GetByIdAsync(post.UserId);
            if (user != null)
            {
                postDto.AuthorName = user.FullName;
                postDto.AuthorAvatar = user.PhotoUrl;
            }

            return postDto;
        }

        public async Task<IEnumerable<PostDto>> GetFeedAsync(int? page = 1, int pageSize = 10)
        {
            var allPosts = await _postRepository.GetAllAsync();
            var sortedPosts = allPosts.OrderByDescending(p => p.CreatedAt)
                                      .Skip(((page ?? 1) - 1) * pageSize)
                                      .Take(pageSize);

            var postDtos = new List<PostDto>();
            foreach (var post in sortedPosts)
            {
                var dto = post.Adapt<PostDto>();
                var user = await _userRepository.GetByIdAsync(post.UserId);

                dto.AuthorName = user?.FullName ?? "Unknown";
                dto.AuthorAvatar = user?.PhotoUrl ?? "/images/default-avatar-icon.png";

                postDtos.Add(dto);
            }

            return postDtos;
        }

        public async Task<IEnumerable<PostDto>> GetAllAsync()
        {
            var posts = await _postRepository.GetAllAsync();
            var postDtos = new List<PostDto>();

            foreach (var post in posts)
            {
                var dto = post.Adapt<PostDto>();
                var user = await _userRepository.GetByIdAsync(post.UserId);

                dto.AuthorName = user?.FullName ?? "Unknown";
                dto.AuthorAvatar = user?.PhotoUrl ?? "/images/default-avatar-icon.png";

                postDtos.Add(dto);
            }

            return postDtos;
        }

        public async Task<IEnumerable<PostDto>> GetByUserIdAsync(int userId)
        {
            var posts = await _postRepository.GetPostsByUserIdAsync(userId);
            var user = await _userRepository.GetByIdAsync(userId);

            return posts.Select(post =>
            {
                var dto = post.Adapt<PostDto>();
                dto.AuthorName = user?.FullName ?? "Unknown";
                dto.AuthorAvatar = user?.PhotoUrl ?? "/images/default-avatar-icon.png";
                return dto;
            });
        }

        public async Task<PostDto> UpdatePostAsync(int id, UpdatePostDto dto)
        {
            var post = await _postRepository.GetByIdAsync(id);
            if (post == null)
                throw new ArgumentException("Post not found");

            dto.Adapt(post);
            _postRepository.Update(post);
            await _postRepository.SaveAsync();

            var postDto = post.Adapt<PostDto>();
            var user = await _userRepository.GetByIdAsync(post.UserId);
            postDto.AuthorName = user?.FullName ?? "Unknown";
            postDto.AuthorAvatar = user?.PhotoUrl ?? "/images/default-avatar-icon.png";

            return postDto;
        }

        public async Task<bool> DeletePostAsync(int id)
        {
            var post = await _postRepository.GetByIdAsync(id);
            if (post == null)
                return false;

            _postRepository.Delete(post);
            await _postRepository.SaveAsync();
            return true;
        }

        public async Task<bool> LikePostAsync(int postId, int userId)
        {
            var existingLike = await _postLikeRepository.GetAllAsync();
            var like = existingLike.FirstOrDefault(l => l.PostId == postId && l.UserId == userId);

            if (like != null)
                return false;

            var newLike = new PostLike
            {
                PostId = postId,
                UserId = userId,
                CreatedAt = DateTime.UtcNow
            };

            await _postLikeRepository.InsertAsync(newLike);
            await _postLikeRepository.SaveAsync();
            return true;
        }

        public async Task<bool> UnlikePostAsync(int postId, int userId)
        {
            var existingLike = await _postLikeRepository.GetAllAsync();
            var like = existingLike.FirstOrDefault(l => l.PostId == postId && l.UserId == userId);

            if (like == null)
                return false;

            _postLikeRepository.Delete(like);
            await _postLikeRepository.SaveAsync();
            return true;
        }

        public async Task<int> GetLikeCountAsync(int postId)
        {
            return await _postRepository.GetPostLikesCountAsync(postId);
        }

        public async Task<IEnumerable<PostDto>> SearchPostsAsync(string searchTerm)
        {
            var posts = await _postRepository.GetAllAsync();
            var filteredPosts = posts.Where(p => p.Content.ToLower().Contains(searchTerm.ToLower()));

            var postDtos = new List<PostDto>();
            foreach (var post in filteredPosts)
            {
                var dto = post.Adapt<PostDto>();
                var user = await _userRepository.GetByIdAsync(post.UserId);
                dto.AuthorName = user?.FullName ?? "Unknown";
                dto.AuthorAvatar = user?.PhotoUrl ?? "/images/default-avatar-icon.png";
                postDtos.Add(dto);
            }

            return postDtos;
        }
    }
}
