using CHNU_Connect.BLL.DTOs.Comment;
using CHNU_Connect.BLL.Services.Interfaces;
using CHNU_Connect.DAL.Entities;
using CHNU_Connect.DAL.Repositories.Interfaces;
using Mapster;

namespace CHNU_Connect.BLL.Services
{
    public class CommentService : ICommentService
    {
        private readonly ICommentRepository _commentRepository;

        public CommentService(ICommentRepository commentRepository)
        {
            _commentRepository = commentRepository;
        }

        public async Task<CommentDto> CreateCommentAsync(CreateCommentDto dto)
        {
            var comment = dto.Adapt<Comment>();
            await _commentRepository.InsertAsync(comment);
            await _commentRepository.SaveAsync();
            return comment.Adapt<CommentDto>();
        }

        public async Task<CommentDto?> GetByIdAsync(int id)
        {
            var comment = await _commentRepository.GetByIdAsync(id);
            return comment?.Adapt<CommentDto>();
        }

        public async Task<IEnumerable<CommentDto>> GetByPostIdAsync(int postId)
        {
            var comments = await _commentRepository.GetAllAsync();
            var postComments = comments.Where(c => c.PostId == postId);
            return postComments.Adapt<IEnumerable<CommentDto>>();
        }

        public async Task<IEnumerable<CommentDto>> GetByUserIdAsync(int userId)
        {
            var comments = await _commentRepository.GetAllAsync();
            var userComments = comments.Where(c => c.UserId == userId);
            return userComments.Adapt<IEnumerable<CommentDto>>();
        }

        public async Task<CommentDto> UpdateCommentAsync(int id, CreateCommentDto dto)
        {
            var comment = await _commentRepository.GetByIdAsync(id);
            if (comment == null)
                throw new ArgumentException("Comment not found");

            dto.Adapt(comment);
            _commentRepository.Update(comment);
            await _commentRepository.SaveAsync();
            return comment.Adapt<CommentDto>();
        }

        public async Task<bool> DeleteCommentAsync(int id)
        {
            var comment = await _commentRepository.GetByIdAsync(id);
            if (comment == null)
                return false;

            _commentRepository.Delete(comment);
            await _commentRepository.SaveAsync();
            return true;
        }
    }
}
