using CHNU_Connect.DAL.Data;
using CHNU_Connect.DAL.Entities;
using CHNU_Connect.DAL.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace CHNU_Connect.DAL.Repositories
{
    public class CommentRepository : GenericRepository<Comment>, ICommentRepository
    {
        public CommentRepository(AppDbContext context) : base(context)
        {
        }

        public async Task<IEnumerable<Comment>> GetCommentsByPostIdAsync(int postId)
        {
            return await _dbSet.Where(c => c.PostId == postId)
                              .OrderBy(c => c.CreatedAt)
                              .ToListAsync();
        }

        public async Task<IEnumerable<Comment>> GetCommentsByUserIdAsync(int userId)
        {
            return await _dbSet.Where(c => c.UserId == userId)
                              .OrderByDescending(c => c.CreatedAt)
                              .ToListAsync();
        }

        public async Task<IEnumerable<Comment>> GetRecentCommentsAsync(int count)
        {
            return await _dbSet.OrderByDescending(c => c.CreatedAt)
                              .Take(count)
                              .ToListAsync();
        }

        public async Task<int> GetCommentsCountByPostIdAsync(int postId)
        {
            return await _dbSet.CountAsync(c => c.PostId == postId);
        }
    }
}
