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

        public PostService(IPostRepository postRepository, IPostLikeRepository postLikeRepository)
        {
            _postRepository = postRepository;
            _postLikeRepository = postLikeRepository;
        }

        public async Task<PostDto> CreatePostAsync(CreatePostDto dto)
        {
            var post = dto.Adapt<Post>();
            var createdPost = await _postRepository.AddAsync(post);
            await _postRepository.SaveChangesAsync();
            return createdPost.Adapt<PostDto>();
        }

        public async Task<PostDto?> GetByIdAsync(int id)
        {
            var post = await _postRepository.GetByIdAsync(id);
            return post?.Adapt<PostDto>();
        }

        public async Task<IEnumerable<PostDto>> GetAllAsync()
        {
            var posts = await _postRepository.GetAllAsync();
            return posts.Adapt<IEnumerable<PostDto>>();
        }

        public async Task<IEnumerable<PostDto>> GetByUserIdAsync(int userId)
        {
            var posts = await _postRepository.GetPostsByUserIdAsync(userId);
            return posts.Adapt<IEnumerable<PostDto>>();
        }

        public async Task<PostDto> UpdatePostAsync(int id, CreatePostDto dto)
        {
            var post = await _postRepository.GetByIdAsync(id);
            if (post == null)
                throw new ArgumentException("Post not found");

            dto.Adapt(post);
            await _postRepository.UpdateAsync(post);
            await _postRepository.SaveChangesAsync();
            return post.Adapt<PostDto>();
        }

        public async Task<bool> DeletePostAsync(int id)
        {
            var post = await _postRepository.GetByIdAsync(id);
            if (post == null)
                return false;

            await _postRepository.DeleteAsync(post);
            await _postRepository.SaveChangesAsync();
            return true;
        }

        public async Task<bool> LikePostAsync(int postId, int userId)
        {
            var existingLike = await _postLikeRepository.GetAllAsync();
            var like = existingLike.FirstOrDefault(l => l.PostId == postId && l.UserId == userId);
            
            if (like != null)
                return false; // Already liked

            var newLike = new PostLike
            {
                PostId = postId,
                UserId = userId,
                CreatedAt = DateTime.UtcNow
            };

            await _postLikeRepository.AddAsync(newLike);
            await _postLikeRepository.SaveChangesAsync();
            return true;
        }

        public async Task<bool> UnlikePostAsync(int postId, int userId)
        {
            var existingLike = await _postLikeRepository.GetAllAsync();
            var like = existingLike.FirstOrDefault(l => l.PostId == postId && l.UserId == userId);
            
            if (like == null)
                return false; // Not liked

            await _postLikeRepository.DeleteAsync(like);
            await _postLikeRepository.SaveChangesAsync();
            return true;
        }

        public async Task<int> GetLikeCountAsync(int postId)
        {
            return await _postRepository.GetPostLikesCountAsync(postId);
        }
    }
}
