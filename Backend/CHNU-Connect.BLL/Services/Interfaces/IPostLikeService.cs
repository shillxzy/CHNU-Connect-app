using CHNU_Connect.BLL.DTOs.PostLike;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CHNU_Connect.BLL.Services.Interfaces
{
    public interface IPostLikeService
    {
        Task<PostLikeDto> CreatePostLikeAsync(CreatePostLikeDto dto);
        Task<PostLikeDto?> GetByIdAsync(int id);
        Task<IEnumerable<PostLikeDto>> GetByPostIdAsync(int postId);
        Task<IEnumerable<PostLikeDto>> GetByUserIdAsync(int userId);
        Task<bool> DeletePostLikeAsync(int id);
        Task<bool> HasUserLikedPostAsync(int postId, int userId);
    }
}
