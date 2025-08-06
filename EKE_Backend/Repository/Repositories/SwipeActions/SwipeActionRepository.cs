using Microsoft.EntityFrameworkCore;
using Repository.Entities;
using Repository.Repositories.BaseRepository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository.Repositories.SwipeActions
{
    public class SwipeActionRepository : BaseRepository<SwipeAction>, ISwipeActionRepository
    {
        public SwipeActionRepository(ApplicationDbContext context) : base(context) { }

        public async Task<IEnumerable<long>> GetSwipedTutorIdsByStudentAsync(long studentId)
        {
            return await _dbSet
                .Where(sa => sa.StudentId == studentId)
                .Select(sa => sa.TutorId)
                .ToListAsync();
        }

        public async Task<SwipeAction?> GetByStudentAndTutorAsync(long studentId, long tutorId)
        {
            return await _dbSet
                .FirstOrDefaultAsync(sa => sa.StudentId == studentId && sa.TutorId == tutorId);
        }

        public async Task<bool> HasStudentSwipedTutorAsync(long studentId, long tutorId)
        {
            return await _dbSet
                .AnyAsync(sa => sa.StudentId == studentId && sa.TutorId == tutorId);
        }
    }
}
