using Repository.Entities;
using Repository.Repositories.BaseRepository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository.Repositories.Tutors
{
    public interface ITutorSubjectRepository : IBaseRepository<TutorSubject>
    {
        Task<IEnumerable<TutorSubject>> GetByTutorIdAsync(long tutorId);
        Task<IEnumerable<TutorSubject>> GetBySubjectIdAsync(long subjectId);
        Task<TutorSubject?> GetByTutorAndSubjectAsync(long tutorId, long subjectId);
        Task<IEnumerable<TutorSubject>> GetTutorSubjectsWithDetailsAsync(long tutorId);
        Task<bool> TutorHasSubjectAsync(long tutorId, long subjectId);
        Task<IEnumerable<long>> GetTutorIdsBySubjectAsync(long subjectId);
        Task RemoveAllByTutorIdAsync(long tutorId);
        Task<TutorSubject?> GetByTutorAndSubjectIdAsync(long tutorId, long subjectId);
      
    }
}
