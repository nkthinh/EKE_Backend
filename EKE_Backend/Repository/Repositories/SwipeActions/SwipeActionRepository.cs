using Microsoft.EntityFrameworkCore;
using Repository.Entities;
using Repository.Enums;
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
        public async Task<SwipeAction?> GetSwipeActionAsync(long studentId, long tutorId)
        {
            // Lấy swipe action giữa student và tutor, nhưng chỉ lấy action = Like
            return await _dbSet
                .FirstOrDefaultAsync(s => s.StudentId == studentId && s.TutorId == tutorId && s.Action == SwipeActionType.Like);
        }

        public async Task CreateAsync(SwipeAction swipeAction)
        {
            await _dbSet.AddAsync(swipeAction);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(SwipeAction swipeAction)
        {
            _dbSet.Update(swipeAction);
            await _context.SaveChangesAsync();
        }

        public async Task<IEnumerable<long>> GetSwipedStudentIdsByTutorAsync(long tutorId)
        {
            // Lấy tất cả các studentId đã "like" tutor
            return await _dbSet
                .Where(sa => sa.TutorId == tutorId && sa.Action == SwipeActionType.Like)
                .Select(sa => sa.StudentId)
                .ToListAsync();
        }

        // Lấy SwipeAction giữa Student và Tutor theo ID
        public async Task<SwipeAction?> GetSwipeActionByStudentAndTutorAsync(long studentId, long tutorId)
        {
            return await _dbSet
                .FirstOrDefaultAsync(sa => sa.StudentId == studentId && sa.TutorId == tutorId);
        }
    }
}
