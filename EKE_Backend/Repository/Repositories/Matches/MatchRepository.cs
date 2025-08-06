using Microsoft.EntityFrameworkCore;
using Repository.Entities;
using Repository.Enums;
using Repository.Repositories.BaseRepository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository.Repositories.Matches
{
    public class MatchRepository : BaseRepository<Match>, IMatchRepository
    {
        public MatchRepository(ApplicationDbContext context) : base(context) { }

        public async Task<Match?> GetByStudentAndTutorAsync(long studentId, long tutorId)
        {
            return await _dbSet
                .FirstOrDefaultAsync(m => m.StudentId == studentId && m.TutorId == tutorId);
        }

        public async Task<Match?> GetMatchWithDetailsAsync(long matchId)
        {
            return await _dbSet
                .Include(m => m.Student)
                    .ThenInclude(s => s.User)
                .Include(m => m.Tutor)
                    .ThenInclude(t => t.User)
                .Include(m => m.Conversations)
                .FirstOrDefaultAsync(m => m.Id == matchId);
        }

        public async Task<IEnumerable<Match>> GetPendingMatchesForTutorAsync(long tutorId)
        {
            return await _dbSet
                .Include(m => m.Student)
                    .ThenInclude(s => s.User)
                .Include(m => m.Tutor)
                    .ThenInclude(t => t.User)
                .Where(m => m.TutorId == tutorId && m.Status == MatchStatus.Pending)
                .OrderBy(m => m.CreatedAt)
                .ToListAsync();
        }

        public async Task<IEnumerable<Match>> GetActiveMatchesForStudentAsync(long studentId)
        {
            return await _dbSet
                .Include(m => m.Student)
                    .ThenInclude(s => s.User)
                .Include(m => m.Tutor)
                    .ThenInclude(t => t.User)
                .Include(m => m.Conversations)
                .Where(m => m.StudentId == studentId && m.Status == MatchStatus.Active)
                .OrderByDescending(m => m.LastActivity)
                .ToListAsync();
        }

        public async Task<IEnumerable<Match>> GetActiveMatchesForTutorAsync(long tutorId)
        {
            return await _dbSet
                .Include(m => m.Student)
                    .ThenInclude(s => s.User)
                .Include(m => m.Tutor)
                    .ThenInclude(t => t.User)
                .Include(m => m.Conversations)
                .Where(m => m.TutorId == tutorId && m.Status == MatchStatus.Active)
                .OrderByDescending(m => m.LastActivity)
                .ToListAsync();
        }

        public async Task<int> CountActiveMatchesForStudentAsync(long studentId)
        {
            return await _dbSet
                .CountAsync(m => m.StudentId == studentId && m.Status == MatchStatus.Active);
        }

        public async Task<int> CountActiveMatchesForTutorAsync(long tutorId)
        {
            return await _dbSet
                .CountAsync(m => m.TutorId == tutorId && m.Status == MatchStatus.Active);
        }

        public async Task<int> CountPendingMatchesForTutorAsync(long tutorId)
        {
            return await _dbSet
                .CountAsync(m => m.TutorId == tutorId && m.Status == MatchStatus.Pending);
        }
    }
}
