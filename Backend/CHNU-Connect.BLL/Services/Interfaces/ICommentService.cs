using CHNU_Connect.BLL.DTOs.Comment;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CHNU_Connect.BLL.Services.Interfaces
{
    public interface ICommentService
    {
        Task<CommentDto> CreateCommentAsync(CreateCommentDto dto);
        Task<CommentDto?> GetByIdAsync(int id);
        Task<IEnumerable<CommentDto>> GetByPostIdAsync(int postId);
        Task<IEnumerable<CommentDto>> GetByUserIdAsync(int userId);
        Task<CommentDto> UpdateCommentAsync(int id, CreateCommentDto dto);
        Task<bool> DeleteCommentAsync(int id);
    }
}
