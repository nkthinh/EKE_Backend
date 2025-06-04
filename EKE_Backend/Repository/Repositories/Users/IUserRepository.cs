using Repository.Entities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq.Expressions;

namespace Repository.Repositories.Users
{
    public interface IUserRepository
    {
        Task<User> GetByIdAsync(int id);
        Task<User> GetByEmailAsync(string email);
        Task<IEnumerable<User>> GetAllAsync();
        Task<IEnumerable<User>> FindAsync(Expression<Func<User, bool>> predicate);
        Task<User> AddAsync(User user);
        Task<User> UpdateAsync(User user);
        Task<bool> DeleteAsync(int id);
        Task<bool> ExistsAsync(int id);
        Task<bool> EmailExistsAsync(string email);
        Task<IEnumerable<User>> GetPagedAsync(int page, int pageSize);
        Task<int> CountAsync();
    }
}