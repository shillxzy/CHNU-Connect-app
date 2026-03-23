using CHNU_Connect.BLL.DTOs.User;
using CHNU_Connect.BLL.Services.Interfaces;
using CHNU_Connect.DAL.Entities;
using CHNU_Connect.DAL.Repositories.Interfaces;
using Mapster;
using Microsoft.EntityFrameworkCore;

namespace CHNU_Connect.BLL.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;

        public UserService(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        private void EnsureUtc(User user)
        {
            user.CreatedAt = DateTime.SpecifyKind(user.CreatedAt, DateTimeKind.Utc);

            if (user.PasswordResetTokenExpiry.HasValue)
                user.PasswordResetTokenExpiry = DateTime.SpecifyKind(user.PasswordResetTokenExpiry.Value, DateTimeKind.Utc);
        }

        public async Task<UserDto> CreateUserAsync(CreateUserDto dto)
        {
            var user = dto.Adapt<User>();
            // CreatedAt автоматично = DateTime.UtcNow, але переконаємося
            EnsureUtc(user);

            await _userRepository.InsertAsync(user);
            await _userRepository.SaveAsync();
            return user.Adapt<UserDto>();
        }

        public async Task<UserDto?> GetByIdAsync(int id)
        {
            var user = await _userRepository.GetByIdAsync(id);
            return user?.Adapt<UserDto>();
        }

        public async Task<IEnumerable<UserDto>> GetAllAsync()
        {
            var users = await _userRepository.GetAllAsync();
            return users.Adapt<IEnumerable<UserDto>>();
        }

        public async Task<UserDto> UpdateUserAsync(int id, CreateUserDto dto)
        {
            var user = await _userRepository.GetByIdAsync(id);
            if (user == null)
                throw new ArgumentException("User not found");

            // Mapster оновлює поля, крім CreatedAt
            dto.Adapt(user);

            // Завжди переконуємося, що всі DateTime в UTC
            EnsureUtc(user);

            _userRepository.Update(user);
            await _userRepository.SaveAsync();
            return user.Adapt<UserDto>();
        }

        public async Task<bool> DeleteUserAsync(int id)
        {
            var user = await _userRepository.GetByIdAsync(id);
            if (user == null)
                return false;

            _userRepository.Delete(user);
            await _userRepository.SaveAsync();
            return true;
        }

        public async Task<bool> BlockUserAsync(int id)
        {
            var user = await _userRepository.GetByIdAsync(id);
            if (user == null)
                return false;

            user.IsBlocked = true;
            EnsureUtc(user);

            _userRepository.Update(user);
            await _userRepository.SaveAsync();
            return true;
        }

        public async Task<bool> UnblockUserAsync(int id)
        {
            var user = await _userRepository.GetByIdAsync(id);
            if (user == null)
                return false;

            user.IsBlocked = false;
            EnsureUtc(user);

            _userRepository.Update(user);
            await _userRepository.SaveAsync();
            return true;
        }

        public async Task<bool> SetUserRoleAsync(int userId, string newRole)
        {
            var user = await _userRepository.GetByIdAsync(userId);
            if (user == null) return false;

            if (!Enum.TryParse<UserRole>(newRole, true, out var role))
                throw new ArgumentException("Invalid role specified");

            user.Role = role;

            EnsureUtc(user);

            _userRepository.Update(user);
            await _userRepository.SaveAsync();

            return true;
        }


        public async Task UpdateProfileAsync(int id, UpdateProfileDto dto)
        {
            var user = await _userRepository.GetByIdAsync(id);
            if (user == null)
                throw new Exception("User not found");

            user.FullName = dto.FullName ?? user.FullName;
            user.Faculty = dto.Faculty ?? user.Faculty;
            user.Course = dto.Course ?? user.Course;
            user.Bio = dto.Bio ?? user.Bio;
            user.PhotoUrl = dto.PhotoUrl ?? user.PhotoUrl;

            EnsureUtc(user);

            _userRepository.Update(user);
            await _userRepository.SaveAsync();
        }

        public async Task<UserDto?> GetByEmailAsync(string email)
        {
            var user = await _userRepository.GetByEmailAsync(email);
            return user?.Adapt<UserDto>();
        }


        public async Task UpdatePhotoAsync(int userId, string? photoUrl)
        {
            var user = await _userRepository.GetByIdAsync(userId);
            if (user == null)
                throw new Exception("User not found");

            user.PhotoUrl = photoUrl;
            EnsureUtc(user);

            _userRepository.Update(user);
            await _userRepository.SaveAsync();
        }

    }
}
