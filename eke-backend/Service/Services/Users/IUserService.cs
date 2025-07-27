using Microsoft.AspNetCore.Http;
using Repository.Enums;
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
        // Basic CRUD Operations
        Task<UserResponseDto?> GetUserByIdAsync(long id);
        Task<UserResponseDto?> GetUserByEmailAsync(string email);
        Task<IEnumerable<UserResponseDto>> GetAllUsersAsync();
        Task<UserResponseDto> CreateUserAsync(UserCreateDto userCreateDto);
        Task<UserResponseDto> UpdateUserAsync(long id, UserUpdateDto userUpdateDto);
        Task<bool> DeleteUserAsync(long id);
        Task<bool> UserExistsAsync(long id);
        Task<bool> EmailExistsAsync(string email);

        // Authentication
        Task<LoginResponseDto?> AuthenticateAsync(UserLoginDto loginDto);
        Task<bool> ChangePasswordAsync(long userId, string currentPassword, string newPassword);
        Task<LoginResponseDto?> RefreshTokenAsync(string refreshToken);

        // Pagination & Filtering
        Task<(IEnumerable<UserResponseDto> Users, int TotalCount)> GetPagedUsersAsync(int page, int pageSize);
        Task<IEnumerable<UserResponseDto>> GetUsersByRoleAsync(UserRole role);
        Task<IEnumerable<UserResponseDto>> SearchUsersAsync(string searchTerm);

        // Role-specific Operations
        Task<IEnumerable<StudentResponseDto>> GetStudentsAsync();
        Task<IEnumerable<TutorResponseDto>> GetTutorsAsync();
        Task<IEnumerable<TutorResponseDto>> GetVerifiedTutorsAsync();
        Task<IEnumerable<TutorResponseDto>> GetFeaturedTutorsAsync();

        // Profile Management
        Task<UserResponseDto> UpdateProfileAsync(long userId, UserProfileUpdateDto profileUpdateDto);
        Task<bool> UploadProfileImageAsync(long userId, string imageUrl);
        Task<bool> VerifyUserAsync(long userId);
        Task<bool> DeactivateUserAsync(long userId);
        Task<bool> ActivateUserAsync(long userId);

        // Statistics
        Task<UserStatsDto> GetUserStatsAsync();
        Task<UserDashboardDto> GetUserDashboardAsync(long userId);
        // Thêm vào IUserService
        Task<string> UploadProfileImageAsync(long userId, IFormFile imageFile);
    }
}
