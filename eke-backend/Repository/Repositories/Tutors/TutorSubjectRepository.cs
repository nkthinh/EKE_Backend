using Microsoft.EntityFrameworkCore;
using Repository.Entities;
using Repository.Repositories.BaseRepository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository.Repositories.Tutors
{
    public class TutorSubjectRepository : BaseRepository<TutorSubject>, ITutorSubjectRepository
    {
        public TutorSubjectRepository(ApplicationDbContext context) : base(context) { }

        public async Task<IEnumerable<TutorSubject>> GetByTutorIdAsync(long tutorId)
        {
            return await _dbSet
                .Include(ts => ts.Subject)
                .Where(ts => ts.TutorId == tutorId)
                .OrderBy(ts => ts.Subject.Name)
                .ToListAsync();
        }

        public async Task<IEnumerable<TutorSubject>> GetBySubjectIdAsync(long subjectId)
        {
            return await _dbSet
                .Include(ts => ts.Tutor)
                    .ThenInclude(t => t.User)
                .Where(ts => ts.SubjectId == subjectId)
                .OrderByDescending(ts => ts.Tutor.AverageRating)
                .ToListAsync();
        }

        public async Task<TutorSubject?> GetByTutorAndSubjectAsync(long tutorId, long subjectId)
        {
            return await _dbSet
                .Include(ts => ts.Subject)
                .Include(ts => ts.Tutor)
                .FirstOrDefaultAsync(ts => ts.TutorId == tutorId && ts.SubjectId == subjectId);
        }

        public async Task<IEnumerable<TutorSubject>> GetTutorSubjectsWithDetailsAsync(long tutorId)
        {
            return await _dbSet
                .Include(ts => ts.Subject)
                .Include(ts => ts.Tutor)
                    .ThenInclude(t => t.User)
                .Where(ts => ts.TutorId == tutorId)
                .OrderBy(ts => ts.Subject.Name)
                .ToListAsync();
        }

        public async Task<bool> TutorHasSubjectAsync(long tutorId, long subjectId)
        {
            return await _dbSet
                .AnyAsync(ts => ts.TutorId == tutorId && ts.SubjectId == subjectId);
        }

        public async Task<IEnumerable<long>> GetTutorIdsBySubjectAsync(long subjectId)
        {
            return await _dbSet
                .Where(ts => ts.SubjectId == subjectId)
                .Select(ts => ts.TutorId)
                .Distinct()
                .ToListAsync();
        }

        public async Task RemoveAllByTutorIdAsync(long tutorId)
        {
            var tutorSubjects = await _dbSet
                .Where(ts => ts.TutorId == tutorId)
                .ToListAsync();

            _dbSet.RemoveRange(tutorSubjects);
        }
        public async Task<TutorSubject?> GetByTutorAndSubjectIdAsync(long tutorId, long subjectId)
        {
            return await _dbSet
                .Include(ts => ts.Subject)
                .Include(ts => ts.Tutor)
                    .ThenInclude(t => t.User)
                .FirstOrDefaultAsync(ts => ts.TutorId == tutorId && ts.SubjectId == subjectId);
        }
    }
}
