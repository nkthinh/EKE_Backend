using Repository.Enums;
using Microsoft.EntityFrameworkCore;
using Repository.Entities;
using Repository.Repositories.BaseRepository;
using Repository.Repositories.Users;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Repository.Repositories.Users
{
    public class UserRepository : BaseRepository<User>, IUserRepository
    {
        public UserRepository(ApplicationDbContext context) : base(context) { }

        public async Task<User?> GetByEmailAsync(string email)
        {
            return await _dbSet
                .Include(u => u.Student)
                .Include(u => u.Tutor)
                .FirstOrDefaultAsync(u => u.Email.ToLower() == email.ToLower() && u.IsActive);
        }

        public async Task<User?> GetUserWithDetailsAsync(long id)
        {
            return await _dbSet
                .Include(u => u.Student)
                .Include(u => u.Tutor)
                    .ThenInclude(t => t.TutorSubjects)
                        .ThenInclude(ts => ts.Subject)
                .Include(u => u.Tutor)
                    .ThenInclude(t => t.Certifications)
                .FirstOrDefaultAsync(u => u.Id == id && u.IsActive);
        }

        public async Task<bool> EmailExistsAsync(string email)
        {
            return await _dbSet.AnyAsync(u => u.Email.ToLower() == email.ToLower());
        }

        public async Task<IEnumerable<User>> GetAllActiveUsersAsync()
        {
            return await _dbSet
                .Include(u => u.Student)
                .Include(u => u.Tutor)
                .Where(u => u.IsActive)
                .OrderByDescending(u => u.CreatedAt)
                .ToListAsync();
        }

        public async Task<IEnumerable<User>> GetUsersByRoleAsync(UserRole role)
        {
            return await _dbSet
                .Include(u => u.Student)
                .Include(u => u.Tutor)
                .Where(u => u.Role == role && u.IsActive)
                .OrderByDescending(u => u.CreatedAt)
                .ToListAsync();
        }

        public async Task<(IEnumerable<User> Users, int TotalCount)> GetPagedUsersAsync(int page, int pageSize)
        {
            var query = _dbSet
                .Include(u => u.Student)
                .Include(u => u.Tutor)
                .Where(u => u.IsActive);

            var totalCount = await query.CountAsync();

            var users = await query
                .OrderByDescending(u => u.CreatedAt)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return (users, totalCount);
        }

        public async Task<IEnumerable<User>> SearchUsersAsync(string searchTerm)
        {
            return await _dbSet
                .Include(u => u.Student)
                .Include(u => u.Tutor)
                .Where(u => u.IsActive &&
                       (u.FullName.Contains(searchTerm) ||
                        u.Email.Contains(searchTerm) ||
                        (u.Phone != null && u.Phone.Contains(searchTerm))))
                .OrderByDescending(u => u.CreatedAt)
                .ToListAsync();
        }

        public async Task<User?> GetUserForAuthenticationAsync(string email)
        {
            return await _dbSet
                .FirstOrDefaultAsync(u => u.Email.ToLower() == email.ToLower() && u.IsActive);
        }

        public async Task<bool> IsUserActiveAsync(long userId)
        {
            return await _dbSet.AnyAsync(u => u.Id == userId && u.IsActive);
        }

        public async Task<bool> UpdateProfileImageAsync(long userId, string imageUrl)
        {
            var user = await _dbSet.FindAsync(userId);
            if (user == null || !user.IsActive) return false;

            user.ProfileImage = imageUrl;
            user.UpdatedAt = DateTime.UtcNow;

            return true; // Will be saved when UnitOfWork.CompleteAsync() is called
        }

        public async Task<bool> VerifyUserAsync(long userId)
        {
            var user = await _dbSet.FindAsync(userId);
            if (user == null || !user.IsActive) return false;

            user.IsVerified = true;
            user.UpdatedAt = DateTime.UtcNow;

            return true; // Will be saved when UnitOfWork.CompleteAsync() is called
        }

        public async Task<int> GetActiveUsersCountAsync()
        {
            return await _dbSet.CountAsync(u => u.IsActive);
        }

        public async Task<int> GetUsersCountByRoleAsync(UserRole role)
        {
            return await _dbSet.CountAsync(u => u.Role == role && u.IsActive);
        }

        public async Task<int> GetNewUsersCountSinceAsync(DateTime since)
        {
            return await _dbSet.CountAsync(u => u.CreatedAt >= since && u.IsActive);
        }
    }
}