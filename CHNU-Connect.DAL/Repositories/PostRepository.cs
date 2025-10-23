using CHNU_Connect.DAL.Data;
using CHNU_Connect.DAL.Entities;
using CHNU_Connect.DAL.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace CHNU_Connect.DAL.Repositories
{
    public class PostRepository : GenericRepository<Post>, IPostRepository
    {
        public PostRepository(AppDbContext context) : base(context)
        {
        }

        public async Task<IEnumerable<Post>> GetPostsByUserIdAsync(int userId)
        {
            return await _dbSet.Where(p => p.UserId == userId)
                              .OrderByDescending(p => p.CreatedAt)
                              .ToListAsync();
        }

        public async Task<IEnumerable<Post>> GetPostsByGroupIdAsync(int groupId)
        {
            return await _dbSet.Where(p => p.Id == groupId)
                              .OrderByDescending(p => p.CreatedAt)
                              .ToListAsync();
        }

        public async Task<IEnumerable<Post>> GetRecentPostsAsync(int count)
        {
            return await _dbSet.OrderByDescending(p => p.CreatedAt)
                              .Take(count)
                              .ToListAsync();
        }

        public async Task<int> GetPostLikesCountAsync(int postId)
        {
            return await _context.PostLikes.CountAsync(pl => pl.PostId == postId);
        }

        public async Task<bool> IsPostLikedByUserAsync(int postId, int userId)
        {
            return await _context.PostLikes.AnyAsync(pl => pl.PostId == postId && pl.UserId == userId);
        }
    }
}
