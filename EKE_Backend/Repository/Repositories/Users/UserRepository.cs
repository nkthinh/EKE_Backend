using Microsoft.EntityFrameworkCore;
using Repository.Entities;
using Repository.Repositories.Users;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Repository.Repositories.Users
{
    public class UserRepository : IUserRepository
    {
        private readonly ApplicationDbContext _context;
        private readonly DbSet<User> _dbSet;

        public UserRepository(ApplicationDbContext context)
        {
            _context = context;
            _dbSet = _context.Set<User>();
        }

        public async Task<User> GetByIdAsync(int id)
        {
            return await _dbSet
                .Include(u => u.StudentProfiles)
                .Include(u => u.TutorProfile)
                .FirstOrDefaultAsync(u => u.UserId == id);
        }

        public async Task<User> GetByEmailAsync(string email)
        {
            return await _dbSet
                .Include(u => u.StudentProfiles)
                .Include(u => u.TutorProfile)
                .FirstOrDefaultAsync(u => u.Email == email);
        }

        public async Task<IEnumerable<User>> GetAllAsync()
        {
            return await _dbSet
                .Include(u => u.StudentProfiles)
                .Include(u => u.TutorProfile)
                .ToListAsync();
        }

        public async Task<IEnumerable<User>> FindAsync(Expression<Func<User, bool>> predicate)
        {
            return await _dbSet
                .Where(predicate)
                .Include(u => u.StudentProfiles)
                .Include(u => u.TutorProfile)
                .ToListAsync();
        }

        public async Task<User> AddAsync(User user)
        {
            await _dbSet.AddAsync(user);
            await _context.SaveChangesAsync();
            return user;
        }

        public async Task<User> UpdateAsync(User user)
        {
            _dbSet.Update(user);
            await _context.SaveChangesAsync();
            return user;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var user = await _dbSet.FindAsync(id);
            if (user == null) return false;

            _dbSet.Remove(user);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> ExistsAsync(int id)
        {
            return await _dbSet.AnyAsync(u => u.UserId == id);
        }

        public async Task<bool> EmailExistsAsync(string email)
        {
            return await _dbSet.AnyAsync(u => u.Email == email);
        }

        public async Task<IEnumerable<User>> GetPagedAsync(int page, int pageSize)
        {
            return await _dbSet
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Include(u => u.StudentProfiles)
                .Include(u => u.TutorProfile)
                .ToListAsync();
        }

        public async Task<int> CountAsync()
        {
            return await _dbSet.CountAsync();
        }
    }
}