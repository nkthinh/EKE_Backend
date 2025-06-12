using Repository.Enums;
using Repository.Entities;
using Repository.Repositories.BaseRepository;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Repository.Repositories.Users
{
    public interface IUserRepository : IBaseRepository<User>
    {
        // Basic User Operations
        Task<User?> GetByEmailAsync(string email);
        Task<User?> GetUserWithDetailsAsync(long id);
        Task<bool> EmailExistsAsync(string email);
        Task<IEnumerable<User>> GetAllActiveUsersAsync();
        Task<IEnumerable<User>> GetUsersByRoleAsync(UserRole role);

        // Pagination & Search
        Task<(IEnumerable<User> Users, int TotalCount)> GetPagedUsersAsync(int page, int pageSize);
        Task<IEnumerable<User>> SearchUsersAsync(string searchTerm);

        // Authentication Related
        Task<User?> GetUserForAuthenticationAsync(string email);
        Task<bool> IsUserActiveAsync(long userId);

        // Profile Management
        Task<bool> UpdateProfileImageAsync(long userId, string imageUrl);
        Task<bool> VerifyUserAsync(long userId);

        // Statistics
        Task<int> GetActiveUsersCountAsync();
        Task<int> GetUsersCountByRoleAsync(UserRole role);
        Task<int> GetNewUsersCountSinceAsync(DateTime since);
    }
}