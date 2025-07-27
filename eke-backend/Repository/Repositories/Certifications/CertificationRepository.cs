using Microsoft.EntityFrameworkCore;
using Repository.Entities;
using Repository.Repositories.BaseRepository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository.Repositories.Certifications
{
    public class CertificationRepository : BaseRepository<Certification>, ICertificationRepository
    {
        public CertificationRepository(ApplicationDbContext context) : base(context) { }

        public async Task<IEnumerable<Certification>> GetByTutorIdAsync(long tutorId)
        {
            return await _dbSet
                .Include(c => c.Tutor)
                    .ThenInclude(t => t.User)
                .Where(c => c.TutorId == tutorId)
                .OrderByDescending(c => c.CreatedAt)
                .ToListAsync();
        }

        public async Task<IEnumerable<Certification>> GetVerifiedCertificationsByTutorIdAsync(long tutorId)
        {
            return await _dbSet
                .Where(c => c.TutorId == tutorId && c.IsVerified)
                .OrderByDescending(c => c.IssueDate)
                .ToListAsync();
        }

        public async Task<IEnumerable<Certification>> GetPendingVerificationAsync()
        {
            return await _dbSet
                .Include(c => c.Tutor)
                    .ThenInclude(t => t.User)
                .Where(c => !c.IsVerified)
                .OrderBy(c => c.CreatedAt)
                .ToListAsync();
        }

        public async Task<bool> TutorHasCertificationAsync(long tutorId, string certificationName)
        {
            return await _dbSet
                .AnyAsync(c => c.TutorId == tutorId &&
                              c.Name.ToLower() == certificationName.ToLower());
        }
    }
}