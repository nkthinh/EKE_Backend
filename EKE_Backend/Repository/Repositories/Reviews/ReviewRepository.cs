using Microsoft.EntityFrameworkCore;
using Repository.Entities;
using Repository.Repositories.BaseRepository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository.Repositories.Reviews
{
    public class ReviewRepository : BaseRepository<Review>, IReviewRepository
    {
        public ReviewRepository(ApplicationDbContext context) : base(context) { }

        public async Task<IEnumerable<Review>> GetReviewsByTutorIdAsync(long tutorId)
        {
            return await _dbSet
                .Include(r => r.Student)
                .Include(r => r.Student.User)
                .Include(r => r.Tutor)
                .Include(r => r.Tutor.User)
                .Where(r => r.TutorId == tutorId)
                .OrderByDescending(r => r.CreatedAt)
                .ToListAsync();
        }

        public async Task<IEnumerable<Review>> GetReviewsByStudentIdAsync(long studentId)
        {
            return await _dbSet
                .Include(r => r.Student)
                .Include(r => r.Student.User)
                .Include(r => r.Tutor)
                .Include(r => r.Tutor.User)
                .Where(r => r.StudentId == studentId)
                .OrderByDescending(r => r.CreatedAt)
                .ToListAsync();
        }

        public async Task<IEnumerable<Review>> GetApprovedReviewsByTutorIdAsync(long tutorId)
        {
            return await _dbSet
                .Include(r => r.Student)
                .Include(r => r.Student.User)
                .Include(r => r.Tutor)
                .Include(r => r.Tutor.User)
                .Where(r => r.TutorId == tutorId && r.IsApproved)
                .OrderByDescending(r => r.CreatedAt)
                .ToListAsync();
        }

        public async Task<double> GetAverageRatingByTutorIdAsync(long tutorId)
        {
            var reviews = await _dbSet
                .Where(r => r.TutorId == tutorId && r.IsApproved)
                .ToListAsync();

            if (!reviews.Any())
                return 0;

            return reviews.Average(r => r.Rating);
        }

        public async Task<int> GetReviewCountByTutorIdAsync(long tutorId)
        {
            return await _dbSet
                .Where(r => r.TutorId == tutorId && r.IsApproved)
                .CountAsync();
        }

        public async Task<Review?> GetReviewByTutorAndStudentAsync(long tutorId, long studentId)
        {
            return await _dbSet
                .Include(r => r.Student)
                .Include(r => r.Student.User)
                .Include(r => r.Tutor)
                .Include(r => r.Tutor.User)
                .FirstOrDefaultAsync(r => r.TutorId == tutorId && r.StudentId == studentId);
        }

        public async Task<IEnumerable<Review>> GetPendingReviewsAsync()
        {
            return await _dbSet
                .Include(r => r.Student)
                .Include(r => r.Student.User)
                .Include(r => r.Tutor)
                .Include(r => r.Tutor.User)
                .Where(r => !r.IsApproved)
                .OrderByDescending(r => r.CreatedAt)
                .ToListAsync();
        }

        public async Task<IEnumerable<Review>> GetReviewsByRatingAsync(int rating)
        {
            return await _dbSet
                .Include(r => r.Student)
                .Include(r => r.Student.User)
                .Include(r => r.Tutor)
                .Include(r => r.Tutor.User)
                .Where(r => r.Rating == rating && r.IsApproved)
                .OrderByDescending(r => r.CreatedAt)
                .ToListAsync();
        }

        public async Task<bool> HasStudentReviewedTutorAsync(long studentId, long tutorId)
        {
            return await _dbSet
                .AnyAsync(r => r.StudentId == studentId && r.TutorId == tutorId);
        }

        // Override GetByIdAsync to include navigation properties
        public override async Task<Review?> GetByIdAsync(long id)
        {
            return await _dbSet
                .Include(r => r.Student)
                .Include(r => r.Student.User)
                .Include(r => r.Tutor)
                .Include(r => r.Tutor.User)
                .FirstOrDefaultAsync(r => r.Id == id);
        }
    }
}