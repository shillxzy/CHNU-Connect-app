using CHNU_Connect.DAL.Data;
using CHNU_Connect.DAL.Entities;
using CHNU_Connect.DAL.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace CHNU_Connect.DAL.Repositories
{
    public class PostLikeRepository : GenericRepository<PostLike>, IPostLikeRepository
    {
        public PostLikeRepository(AppDbContext context) : base(context)
        {
        }

        public async Task<IEnumerable<PostLike>> GetLikesByPostIdAsync(int postId)
        {
            return await _dbSet.Where(pl => pl.PostId == postId)
                              .ToListAsync();
        }

        public async Task<IEnumerable<PostLike>> GetLikesByUserIdAsync(int userId)
        {
            return await _dbSet.Where(pl => pl.UserId == userId)
                              .ToListAsync();
        }

        public async Task<bool> IsPostLikedByUserAsync(int userId, int postId)
        {
            return await _dbSet.AnyAsync(pl => pl.UserId == userId && pl.PostId == postId);
        }

        public async Task<int> GetLikesCountByPostIdAsync(int postId)
        {
            return await _dbSet.CountAsync(pl => pl.PostId == postId);
        }

        public async Task<PostLike?> GetPostLikeAsync(int userId, int postId)
        {
            return await _dbSet.FirstOrDefaultAsync(pl => pl.UserId == userId && pl.PostId == postId);
        }
    }
}
