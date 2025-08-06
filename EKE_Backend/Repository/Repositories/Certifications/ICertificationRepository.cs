using Repository.Entities;
using Repository.Repositories.BaseRepository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository.Repositories.Certifications
{
    public interface ICertificationRepository : IBaseRepository<Certification>
    {
        Task<IEnumerable<Certification>> GetByTutorIdAsync(long tutorId);
        Task<IEnumerable<Certification>> GetVerifiedCertificationsByTutorIdAsync(long tutorId);
        Task<IEnumerable<Certification>> GetPendingVerificationAsync();
        Task<bool> TutorHasCertificationAsync(long tutorId, string certificationName);
    }
}
