using Repository.Entities;
using Repository.Repositories.BaseRepository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository.Repositories.Matches
{
    public interface IMatchRepository : IBaseRepository<Match>
    {
        Task<Match?> GetByStudentAndTutorAsync(long studentId, long tutorId);
        Task<Match?> GetMatchWithDetailsAsync(long matchId);
        Task<IEnumerable<Match>> GetPendingMatchesForTutorAsync(long tutorId);
        Task<IEnumerable<Match>> GetActiveMatchesForStudentAsync(long studentId);
        Task<IEnumerable<Match>> GetActiveMatchesForTutorAsync(long tutorId);
        Task<int> CountActiveMatchesForStudentAsync(long studentId);
        Task<int> CountActiveMatchesForTutorAsync(long tutorId);
        Task<int> CountPendingMatchesForTutorAsync(long tutorId);
    }
}
