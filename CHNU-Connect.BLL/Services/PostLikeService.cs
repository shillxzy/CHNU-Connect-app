using CHNU_Connect.BLL.DTOs.PostLike;
using CHNU_Connect.BLL.Services.Interfaces;
using CHNU_Connect.DAL.Entities;
using CHNU_Connect.DAL.Repositories.Interfaces;
using Mapster;

namespace CHNU_Connect.BLL.Services
{
    public class PostLikeService : IPostLikeService
    {
        private readonly IPostLikeRepository _postLikeRepository;

        public PostLikeService(IPostLikeRepository postLikeRepository)
        {
            _postLikeRepository = postLikeRepository;
        }

        public async Task<PostLikeDto> CreatePostLikeAsync(CreatePostLikeDto dto)
        {
            var like = dto.Adapt<PostLike>();
            var createdLike = await _postLikeRepository.AddAsync(like);
            await _postLikeRepository.SaveChangesAsync();
            return createdLike.Adapt<PostLikeDto>();
        }

        public async Task<PostLikeDto?> GetByIdAsync(int id)
        {
            var like = await _postLikeRepository.GetByIdAsync(id);
            return like?.Adapt<PostLikeDto>();
        }

        public async Task<IEnumerable<PostLikeDto>> GetByPostIdAsync(int postId)
        {
            var likes = await _postLikeRepository.GetAllAsync();
            var postLikes = likes.Where(l => l.PostId == postId);
            return postLikes.Adapt<IEnumerable<PostLikeDto>>();
        }

        public async Task<IEnumerable<PostLikeDto>> GetByUserIdAsync(int userId)
        {
            var likes = await _postLikeRepository.GetAllAsync();
            var userLikes = likes.Where(l => l.UserId == userId);
            return userLikes.Adapt<IEnumerable<PostLikeDto>>();
        }

        public async Task<bool> DeletePostLikeAsync(int id)
        {
            var like = await _postLikeRepository.GetByIdAsync(id);
            if (like == null)
                return false;

            await _postLikeRepository.DeleteAsync(like);
            await _postLikeRepository.SaveChangesAsync();
            return true;
        }

        public async Task<bool> HasUserLikedPostAsync(int postId, int userId)
        {
            var likes = await _postLikeRepository.GetAllAsync();
            return likes.Any(l => l.PostId == postId && l.UserId == userId);
        }
    }
}
