using Repository.Entities;
using Repository.Enums;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Repository.Repositories
{
    public interface IMatchRepository
    {
        // Base repository methods
        Task<Match> GetByIdAsync(long id);
        Task<IEnumerable<Match>> GetAllAsync();
        Task<IEnumerable<Match>> FindAsync(Expression<Func<Match, bool>> predicate);
        Task<Match> CreateAsync(Match entity);
        Task<Match> UpdateAsync(Match entity);
        Task<bool> DeleteAsync(long id);
        Task<bool> ExistsAsync(long id);
        Task<int> CountAsync();
        Task<int> CountAsync(Expression<Func<Match, bool>> predicate);

        // Match-specific methods
        Task<IEnumerable<Match>> GetMatchesByStudentIdAsync(long studentId);
        Task<IEnumerable<Match>> GetMatchesByTutorIdAsync(long tutorId);
        Task<IEnumerable<Match>> GetMatchesByStatusAsync(MatchStatus status);
        Task<Match> GetActiveMatchAsync(long studentId, long tutorId);
        Task<IEnumerable<Match>> GetMatchesWithDetailsAsync();
        Task<Match> GetMatchWithDetailsAsync(long id);
        Task<IEnumerable<Match>> GetMatchesByDateRangeAsync(DateTime startDate, DateTime endDate);
        Task<IEnumerable<Match>> GetInactiveMatchesAsync(DateTime beforeDate);
        Task<bool> UpdateLastActivityAsync(long id);
        Task<bool> HasActiveMatchAsync(long studentId, long tutorId);
        Task<IEnumerable<Match>> GetStudentActiveMatchesAsync(long studentId);
        Task<IEnumerable<Match>> GetTutorActiveMatchesAsync(long tutorId);
    }
}