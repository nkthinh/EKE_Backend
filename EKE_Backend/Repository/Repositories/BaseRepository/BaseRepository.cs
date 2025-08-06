using Microsoft.EntityFrameworkCore;
using Repository.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Repository.Repositories.BaseRepository
{
    public class BaseRepository<T> : IBaseRepository<T> where T : BaseEntity
    {
        protected readonly ApplicationDbContext _context;
        protected readonly DbSet<T> _dbSet;

        public BaseRepository(ApplicationDbContext context)
        {
            _context = context;
            _dbSet = context.Set<T>();
        }

        public virtual async Task<T> GetByIdAsync(long id)
        {
            return await _dbSet.FindAsync(id);
        }

        public virtual async Task<IEnumerable<T>> GetAllAsync()
        {
            return await _dbSet.ToListAsync();
        }

        public virtual async Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate)
        {
            return await _dbSet.Where(predicate).ToListAsync();
        }

        public virtual async Task<T> FirstOrDefaultAsync(Expression<Func<T, bool>> predicate)
        {
            return await _dbSet.FirstOrDefaultAsync(predicate);
        }

        public virtual async Task<T> AddAsync(T entity)
        {
            await _dbSet.AddAsync(entity);
            return entity;
        }

        public virtual async Task<IEnumerable<T>> AddRangeAsync(IEnumerable<T> entities)
        {
            await _dbSet.AddRangeAsync(entities);
            return entities;
        }

        public virtual T Update(T entity)
        {
            _dbSet.Update(entity);
            return entity;
        }

        public virtual void UpdateRange(IEnumerable<T> entities)
        {
            _dbSet.UpdateRange(entities);
        }

        public virtual void Remove(T entity)
        {
            _dbSet.Remove(entity);
        }

        public virtual void RemoveRange(IEnumerable<T> entities)
        {
            _dbSet.RemoveRange(entities);
        }

        // Additional method for Remove by ID if needed
        public virtual async Task<bool> RemoveByIdAsync(long id)
        {
            var entity = await GetByIdAsync(id);
            if (entity != null)
            {
                Remove(entity);
                return true;
            }
            return false;
        }

        public virtual async Task<int> CountAsync()
        {
            return await _dbSet.CountAsync();
        }

        public virtual async Task<int> CountAsync(Expression<Func<T, bool>> predicate)
        {
            return await _dbSet.CountAsync(predicate);
        }

        public virtual async Task<bool> AnyAsync(Expression<Func<T, bool>> predicate)
        {
            return await _dbSet.AnyAsync(predicate);
        }

        public virtual IQueryable<T> Query()
        {
            return _dbSet.AsQueryable();
        }

        public virtual async Task<IEnumerable<T>> GetPagedAsync(int page, int pageSize)
        {
            return await _dbSet
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
        }

        public virtual async Task<IEnumerable<T>> GetPagedAsync(int page, int pageSize, Expression<Func<T, bool>> predicate)
        {
            return await _dbSet
                .Where(predicate)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
        }


    }
}