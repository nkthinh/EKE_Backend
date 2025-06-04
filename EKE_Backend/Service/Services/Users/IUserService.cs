using Service.DTO.Request;
using Service.DTO.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.Services.Users
{
    public interface IUserService
    {
        Task<UserResponseDto> GetUserByIdAsync(int id);
        Task<UserResponseDto> GetUserByEmailAsync(string email);
        Task<IEnumerable<UserResponseDto>> GetAllUsersAsync();
        Task<UserResponseDto> CreateUserAsync(UserCreateDto userCreateDto);
        Task<UserResponseDto> UpdateUserAsync(int id, UserUpdateDto userUpdateDto);
        Task<bool> DeleteUserAsync(int id);
        Task<bool> UserExistsAsync(int id);
        Task<bool> EmailExistsAsync(string email);
        Task<LoginResponseDto> AuthenticateAsync(UserLoginDto loginDto);
        Task<(IEnumerable<UserResponseDto> Users, int TotalCount)> GetPagedUsersAsync(int page, int pageSize);
        Task<bool> ChangePasswordAsync(int userId, string currentPassword, string newPassword);
    }
}
