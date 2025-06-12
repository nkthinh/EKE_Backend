using Repository.Entities;
using Repository.Repositories.BaseRepository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository.Repositories.Tutors
{
    public interface ITutorRepository : IBaseRepository<Tutor>
    {
        Task<Tutor?> GetTutorWithDetailsAsync(long tutorId);
        Task<IEnumerable<Tutor>> GetTutorsWithDetailsAsync();
        Task<IEnumerable<Tutor>> GetVerifiedTutorsAsync();
        Task<IEnumerable<Tutor>> GetFeaturedTutorsAsync();
        Task<Tutor?> GetTutorByUserIdAsync(long userId);
        Task<IEnumerable<Tutor>> GetTutorsBySubjectAsync(long subjectId);
        Task<IEnumerable<Tutor>> GetTutorsByRatingAsync(decimal minRating);
    }
}
