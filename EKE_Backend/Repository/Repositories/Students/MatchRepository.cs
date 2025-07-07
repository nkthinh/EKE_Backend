using Microsoft.EntityFrameworkCore;
using Repository.Entities;
using Repository.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Repository.Repositories
{
    public class MatchRepository : IMatchRepository
    {
        protected readonly ApplicationDbContext _context;
        protected readonly DbSet<Match> _dbSet;

        public MatchRepository(ApplicationDbContext context)
        {
            _context = context;
            _dbSet = context.Set<Match>();
        }

        // Base repository implementation
        public virtual async Task<Match> GetByIdAsync(long id)
        {
            return await _dbSet
                .Include(m => m.Student)
                .Include(m => m.Tutor)
                .Include(m => m.Conversations)
                .FirstOrDefaultAsync(m => m.Id == id);
        }

        public virtual async Task<IEnumerable<Match>> GetAllAsync()
        {
            return await _dbSet
                .Include(m => m.Student)
                .Include(m => m.Tutor)
                .Include(m => m.Conversations)
                .OrderByDescending(m => m.MatchedAt)
                .ToListAsync();
        }

        public virtual async Task<IEnumerable<Match>> FindAsync(Expression<Func<Match, bool>> predicate)
        {
            return await _dbSet
                .Include(m => m.Student)
                .Include(m => m.Tutor)
                .Include(m => m.Conversations)
                .Where(predicate)
                .ToListAsync();
        }

        public virtual async Task<Match> CreateAsync(Match entity)
        {
            await _dbSet.AddAsync(entity);
            await _context.SaveChangesAsync();
            return entity;
        }

        public virtual async Task<Match> UpdateAsync(Match entity)
        {
            _dbSet.Update(entity);
            await _context.SaveChangesAsync();
            return entity;
        }

        public virtual async Task<bool> DeleteAsync(long id)
        {
            var entity = await _dbSet.FindAsync(id);
            if (entity == null)
                return false;

            _dbSet.Remove(entity);
            await _context.SaveChangesAsync();
            return true;
        }

        public virtual async Task<bool> ExistsAsync(long id)
        {
            return await _dbSet.AnyAsync(m => m.Id == id);
        }

        public virtual async Task<int> CountAsync()
        {
            return await _dbSet.CountAsync();
        }

        public virtual async Task<int> CountAsync(Expression<Func<Match, bool>> predicate)
        {
            return await _dbSet.CountAsync(predicate);
        }
        public async Task<IEnumerable<Match>> GetMatchesByStudentIdAsync(long studentId)
        {
            return await _dbSet
                .Where(m => m.StudentId == studentId)
                .Include(m => m.Student)
                .Include(m => m.Tutor)
                .Include(m => m.Conversations)
                .OrderByDescending(m => m.MatchedAt)
                .ToListAsync();
        }

        public async Task<IEnumerable<Match>> GetMatchesByTutorIdAsync(long tutorId)
        {
            return await _dbSet
                .Where(m => m.TutorId == tutorId)
                .Include(m => m.Student)
                .Include(m => m.Tutor)
                .Include(m => m.Conversations)
                .OrderByDescending(m => m.MatchedAt)
                .ToListAsync();
        }

        public async Task<IEnumerable<Match>> GetMatchesByStatusAsync(MatchStatus status)
        {
            return await _dbSet
                .Where(m => m.Status == status)
                .Include(m => m.Student)
                .Include(m => m.Tutor)
                .OrderByDescending(m => m.LastActivity)
                .ToListAsync();
        }

        public async Task<Match> GetActiveMatchAsync(long studentId, long tutorId)
        {
            return await _dbSet
                .Include(m => m.Student)
                .Include(m => m.Tutor)
                .Include(m => m.Conversations)
                .FirstOrDefaultAsync(m =>
                    m.StudentId == studentId &&
                    m.TutorId == tutorId &&
                    m.Status == MatchStatus.Active);
        }

        public async Task<IEnumerable<Match>> GetMatchesWithDetailsAsync()
        {
            return await _dbSet
                .Include(m => m.Student)
                    .ThenInclude(s => s.User)
                .Include(m => m.Tutor)
                    .ThenInclude(t => t.User)
                .Include(m => m.Conversations)
                .OrderByDescending(m => m.MatchedAt)
                .ToListAsync();
        }

        public async Task<Match> GetMatchWithDetailsAsync(long id)
        {
            return await _dbSet
                .Include(m => m.Student)
                    .ThenInclude(s => s.User)
                .Include(m => m.Tutor)
                    .ThenInclude(t => t.User)
                .Include(m => m.Conversations)
                .FirstOrDefaultAsync(m => m.Id == id);
        }

        public async Task<IEnumerable<Match>> GetMatchesByDateRangeAsync(DateTime startDate, DateTime endDate)
        {
            return await _dbSet
                .Where(m => m.MatchedAt >= startDate && m.MatchedAt <= endDate)
                .Include(m => m.Student)
                    .ThenInclude(s => s.User)
                .Include(m => m.Tutor)
                    .ThenInclude(t => t.User)
                .OrderByDescending(m => m.MatchedAt)
                .ToListAsync();
        }

        public async Task<IEnumerable<Match>> GetInactiveMatchesAsync(DateTime beforeDate)
        {
            return await _dbSet
                .Where(m => m.LastActivity < beforeDate)
                .Include(m => m.Student)
                    .ThenInclude(s => s.User)
                .Include(m => m.Tutor)
                    .ThenInclude(t => t.User)
                .OrderBy(m => m.LastActivity)
                .ToListAsync();
        }

        public async Task<bool> UpdateLastActivityAsync(long id)
        {
            var match = await _dbSet.FindAsync(id);
            if (match == null)
                return false;

            match.LastActivity = DateTime.UtcNow;
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> HasActiveMatchAsync(long studentId, long tutorId)
        {
            return await _dbSet
                .AnyAsync(m =>
                    m.StudentId == studentId &&
                    m.TutorId == tutorId &&
                    m.Status == MatchStatus.Active);
        }

        public async Task<IEnumerable<Match>> GetStudentActiveMatchesAsync(long studentId)
        {
            return await _dbSet
                .Where(m => m.StudentId == studentId && m.Status == MatchStatus.Active)
                .Include(m => m.Student)
                    .ThenInclude(s => s.User)
                .Include(m => m.Tutor)
                    .ThenInclude(t => t.User)
                .Include(m => m.Conversations)
                .OrderByDescending(m => m.LastActivity)
                .ToListAsync();
        }

        public async Task<IEnumerable<Match>> GetTutorActiveMatchesAsync(long tutorId)
        {
            return await _dbSet
                .Where(m => m.TutorId == tutorId && m.Status == MatchStatus.Active)
                .Include(m => m.Student)
                    .ThenInclude(s => s.User)
                .Include(m => m.Tutor)
                    .ThenInclude(t => t.User)
                .Include(m => m.Conversations)
                .OrderByDescending(m => m.LastActivity)
                .ToListAsync();
        }
    }
}
