using CHNU_Connect.BLL.DTOs.Post;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CHNU_Connect.BLL.Services.Interfaces
{
    public interface IPostService
    {
        Task<PostDto> CreatePostAsync(CreatePostDto dto);
        Task<PostDto?> GetByIdAsync(int id);
        Task<IEnumerable<PostDto>> GetAllAsync();
        Task<IEnumerable<PostDto>> GetByUserIdAsync(int userId);
        Task<PostDto> UpdatePostAsync(int id, CreatePostDto dto);
        Task<bool> DeletePostAsync(int id);
        Task<bool> LikePostAsync(int postId, int userId);
        Task<bool> UnlikePostAsync(int postId, int userId);
        Task<int> GetLikeCountAsync(int postId);
    }
}
