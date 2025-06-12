using Microsoft.EntityFrameworkCore;
using Repository.Entities;
using Repository.Enums;
using Repository.Repositories.BaseRepository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository.Repositories.Tutors
{
    public class TutorRepository : BaseRepository<Tutor>, ITutorRepository
    {
        public TutorRepository(ApplicationDbContext context) : base(context) { }

        public async Task<Tutor?> GetTutorWithDetailsAsync(long tutorId)
        {
            return await _dbSet
                .Include(t => t.User)
                .Include(t => t.TutorSubjects)
                    .ThenInclude(ts => ts.Subject)
                .Include(t => t.Certifications)
                .Include(t => t.ReviewsReceived.Take(5)) // Latest 5 reviews
                .FirstOrDefaultAsync(t => t.Id == tutorId);
        }

        public async Task<IEnumerable<Tutor>> GetTutorsWithDetailsAsync()
        {
            return await _dbSet
                .Include(t => t.User)
                .Include(t => t.TutorSubjects)
                    .ThenInclude(ts => ts.Subject)
                .Where(t => t.User.IsActive)
                .OrderByDescending(t => t.AverageRating)
                .ToListAsync();
        }

        public async Task<IEnumerable<Tutor>> GetVerifiedTutorsAsync()
        {
            return await _dbSet
                .Include(t => t.User)
                .Include(t => t.TutorSubjects)
                    .ThenInclude(ts => ts.Subject)
                .Where(t => t.VerificationStatus == VerificationStatus.Verified && t.User.IsActive)
                .OrderByDescending(t => t.AverageRating)
                .ToListAsync();
        }

        public async Task<IEnumerable<Tutor>> GetFeaturedTutorsAsync()
        {
            return await _dbSet
                .Include(t => t.User)
                .Include(t => t.TutorSubjects)
                    .ThenInclude(ts => ts.Subject)
                .Where(t => t.IsFeatured && t.User.IsActive)
                .OrderByDescending(t => t.AverageRating)
                .ToListAsync();
        }

        public async Task<Tutor?> GetTutorByUserIdAsync(long userId)
        {
            return await _dbSet
                .Include(t => t.User)
                .Include(t => t.TutorSubjects)
                    .ThenInclude(ts => ts.Subject)
                .Include(t => t.Certifications)
                .FirstOrDefaultAsync(t => t.UserId == userId);
        }

        public async Task<IEnumerable<Tutor>> GetTutorsBySubjectAsync(long subjectId)
        {
            return await _dbSet
                .Include(t => t.User)
                .Include(t => t.TutorSubjects)
                    .ThenInclude(ts => ts.Subject)
                .Where(t => t.TutorSubjects.Any(ts => ts.SubjectId == subjectId) && t.User.IsActive)
                .OrderByDescending(t => t.AverageRating)
                .ToListAsync();
        }

        public async Task<IEnumerable<Tutor>> GetTutorsByRatingAsync(decimal minRating)
        {
            return await _dbSet
                .Include(t => t.User)
                .Where(t => t.AverageRating >= minRating && t.User.IsActive)
                .OrderByDescending(t => t.AverageRating)
                .ToListAsync();
        }
    }
}
