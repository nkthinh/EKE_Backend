using Repository.Entities;
using Repository.Repositories.BaseRepository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository.Repositories.Reviews
{
    public interface IReviewRepository : IBaseRepository<Review>
    {
        Task<IEnumerable<Review>> GetReviewsByTutorIdAsync(long tutorId);
        Task<IEnumerable<Review>> GetReviewsByStudentIdAsync(long studentId);
        Task<IEnumerable<Review>> GetApprovedReviewsByTutorIdAsync(long tutorId);
        Task<double> GetAverageRatingByTutorIdAsync(long tutorId);
        Task<int> GetReviewCountByTutorIdAsync(long tutorId);
        Task<Review?> GetReviewByTutorAndStudentAsync(long tutorId, long studentId);
        Task<IEnumerable<Review>> GetPendingReviewsAsync();
        Task<IEnumerable<Review>> GetReviewsByRatingAsync(int rating);
        Task<bool> HasStudentReviewedTutorAsync(long studentId, long tutorId);
    }
}
