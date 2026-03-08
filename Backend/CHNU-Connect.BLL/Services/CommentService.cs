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
        private readonly IUserRepository _userRepository;

        public CommentService(ICommentRepository commentRepository, IUserRepository userRepository)
        {
            _commentRepository = commentRepository;
            _userRepository = userRepository;
        }

        public async Task<CommentDto> CreateCommentAsync(CreateCommentDto dto)
        {
            var comment = dto.Adapt<Comment>();
            comment.CreatedAt = DateTime.UtcNow;

            await _commentRepository.InsertAsync(comment);
            await _commentRepository.SaveAsync();

            var user = await _userRepository.GetByIdAsync(comment.UserId);

            var commentDto = comment.Adapt<CommentDto>();
            commentDto.AuthorName = user?.FullName ?? "Unknown";
            commentDto.AuthorAvatar = user?.PhotoUrl;

            return commentDto;
        }

        public async Task<CommentDto?> GetByIdAsync(int id)
        {
            var comment = await _commentRepository.GetByIdAsync(id);
            if (comment == null) return null;

            var user = await _userRepository.GetByIdAsync(comment.UserId);
            var commentDto = comment.Adapt<CommentDto>();
            commentDto.AuthorName = user?.FullName ?? "Unknown";
            commentDto.AuthorAvatar = user?.PhotoUrl;

            return commentDto;
        }

        public async Task<IEnumerable<CommentDto>> GetByPostIdAsync(int postId)
        {
            var comments = await _commentRepository.GetAllAsync();
            var postComments = comments.Where(c => c.PostId == postId);

            var result = new List<CommentDto>();
            foreach (var comment in postComments)
            {
                var user = await _userRepository.GetByIdAsync(comment.UserId);
                var dto = comment.Adapt<CommentDto>();
                dto.AuthorName = user?.FullName ?? "Unknown";
                dto.AuthorAvatar = user?.PhotoUrl;
                result.Add(dto);
            }

            return result;
        }

        public async Task<IEnumerable<CommentDto>> GetByUserIdAsync(int userId)
        {
            var comments = await _commentRepository.GetAllAsync();
            var userComments = comments.Where(c => c.UserId == userId);

            var user = await _userRepository.GetByIdAsync(userId);
            var result = userComments.Select(c =>
            {
                var dto = c.Adapt<CommentDto>();
                dto.AuthorName = user?.FullName ?? "Unknown";
                dto.AuthorAvatar = user?.PhotoUrl;
                return dto;
            });

            return result;
        }

        public async Task<CommentDto> UpdateCommentAsync(int id, CreateCommentDto dto)
        {
            var comment = await _commentRepository.GetByIdAsync(id);
            if (comment == null)
                throw new ArgumentException("Comment not found");

            dto.Adapt(comment);
            _commentRepository.Update(comment);
            await _commentRepository.SaveAsync();

            var user = await _userRepository.GetByIdAsync(comment.UserId);
            var commentDto = comment.Adapt<CommentDto>();
            commentDto.AuthorName = user?.FullName ?? "Unknown";
            commentDto.AuthorAvatar = user?.PhotoUrl;

            return commentDto;
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
