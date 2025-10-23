using CHNU_Connect.BLL.DTOs.User;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CHNU_Connect.BLL.Services.Interfaces
{
    public interface IUserService
    {
        Task<UserDto> CreateUserAsync(CreateUserDto dto);
        Task<UserDto?> GetByIdAsync(int id);
        Task<IEnumerable<UserDto>> GetAllAsync();
        Task<UserDto> UpdateUserAsync(int id, CreateUserDto dto);
        Task<bool> DeleteUserAsync(int id);
        Task<bool> BlockUserAsync(int id);
        Task<bool> UnblockUserAsync(int id);
    }
}
