using Repository.Entities;
using Repository.Repositories.BaseRepository;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Repository.Repositories.Matches
{
    public interface IMatchRepository : IBaseRepository<Match>
    {
        Task<Match?> GetByStudentAndTutorAsync(long studentId, long tutorId);
        Task<Match?> GetMatchWithDetailsAsync(long matchId);
        Task<IEnumerable<Match>> GetMatchesWithDetailsAsync();
        Task<IEnumerable<Match>> GetPendingMatchesForTutorAsync(long tutorId);
        Task<IEnumerable<Match>> GetActiveMatchesForStudentAsync(long studentId);
        Task<IEnumerable<Match>> GetActiveMatchesForTutorAsync(long tutorId);
        Task<int> CountActiveMatchesForStudentAsync(long studentId);
        Task<int> CountActiveMatchesForTutorAsync(long tutorId);
        Task<int> CountPendingMatchesForTutorAsync(long tutorId);
        Task<bool> HasActiveMatchAsync(long studentId, long tutorId);
        Task<IEnumerable<Match>> GetMatchesByStudentIdAsync(long studentId);
        Task<IEnumerable<Match>> GetMatchesByTutorIdAsync(long tutorId);
        Task<bool> UpdateLastActivityAsync(long id);
        Task<Match> CreateAsync(Match match);
        Task<Match> UpdateAsync(Match match);
        Task<bool> DeleteAsync(long id);
        Task<Match?> GetMatchByStudentAndTutorAsync(long studentId, long tutorId);
    }
}
