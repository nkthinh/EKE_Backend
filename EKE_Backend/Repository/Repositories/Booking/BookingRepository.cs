using Microsoft.EntityFrameworkCore;
using Repository.Entities;
using Repository.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Repository.Repositories
{
    public class BookingRepository : IBookingRepository
    {
        protected readonly ApplicationDbContext _context;
        protected readonly DbSet<Booking> _dbSet;

        public BookingRepository(ApplicationDbContext context)
        {
            _context = context;
            _dbSet = context.Set<Booking>();
        }

        // Base repository implementation
        public virtual async Task<Booking> GetByIdAsync(long id)
        {
            return await _dbSet
                .Include(b => b.Student)
                    .ThenInclude(s => s.User)
                .Include(b => b.Tutor)
                    .ThenInclude(t => t.User)
                .Include(b => b.Subject)
                .FirstOrDefaultAsync(b => b.Id == id);
        }

        public virtual async Task<IEnumerable<Booking>> GetAllAsync()
        {
            return await _dbSet
                .Include(b => b.Student)
                    .ThenInclude(s => s.User)
                .Include(b => b.Tutor)
                    .ThenInclude(t => t.User)
                .Include(b => b.Subject)
                .OrderByDescending(b => b.CreatedAt)
                .ToListAsync();
        }

        public virtual async Task<IEnumerable<Booking>> FindAsync(Expression<Func<Booking, bool>> predicate)
        {
            return await _dbSet
                .Include(b => b.Student)
                    .ThenInclude(s => s.User)
                .Include(b => b.Tutor)
                    .ThenInclude(t => t.User)
                .Include(b => b.Subject)
                .Where(predicate)
                .ToListAsync();
        }

        public virtual async Task<Booking> CreateAsync(Booking entity)
        {
            await _dbSet.AddAsync(entity);
            await _context.SaveChangesAsync();
            return entity;
        }

        public virtual async Task<Booking> UpdateAsync(Booking entity)
        {
            _dbSet.Update(entity);
            await _context.SaveChangesAsync();
            return entity;
        }

        public virtual async Task<bool> DeleteAsync(long id)
        {
            var entity = await _dbSet.FindAsync(id);
            if (entity == null)
                return false;

            _dbSet.Remove(entity);
            await _context.SaveChangesAsync();
            return true;
        }

        public virtual async Task<bool> ExistsAsync(long id)
        {
            return await _dbSet.AnyAsync(b => b.Id == id);
        }

        public virtual async Task<int> CountAsync()
        {
            return await _dbSet.CountAsync();
        }

        public virtual async Task<int> CountAsync(Expression<Func<Booking, bool>> predicate)
        {
            return await _dbSet.CountAsync(predicate);
        }

        // Booking-specific methods
        public async Task<IEnumerable<Booking>> GetBookingsByStudentIdAsync(long studentId)
        {
            return await _dbSet
                .Where(b => b.StudentId == studentId)
                .Include(b => b.Student)
                    .ThenInclude(s => s.User)
                .Include(b => b.Tutor)
                    .ThenInclude(t => t.User)
                .Include(b => b.Subject)
                .OrderByDescending(b => b.StartTime)
                .ToListAsync();
        }

        public async Task<IEnumerable<Booking>> GetBookingsByTutorIdAsync(long tutorId)
        {
            return await _dbSet
                .Where(b => b.TutorId == tutorId)
                .Include(b => b.Student)
                    .ThenInclude(s => s.User)
                .Include(b => b.Tutor)
                    .ThenInclude(t => t.User)
                .Include(b => b.Subject)
                .OrderByDescending(b => b.StartTime)
                .ToListAsync();
        }

        public async Task<IEnumerable<Booking>> GetBookingsBySubjectIdAsync(long subjectId)
        {
            return await _dbSet
                .Where(b => b.SubjectId == subjectId)
                .Include(b => b.Student)
                    .ThenInclude(s => s.User)
                .Include(b => b.Tutor)
                    .ThenInclude(t => t.User)
                .Include(b => b.Subject)
                .OrderByDescending(b => b.StartTime)
                .ToListAsync();
        }

        public async Task<IEnumerable<Booking>> GetBookingsByStatusAsync(BookingStatus status)
        {
            return await _dbSet
                .Where(b => b.Status == status)
                .Include(b => b.Student)
                    .ThenInclude(s => s.User)
                .Include(b => b.Tutor)
                    .ThenInclude(t => t.User)
                .Include(b => b.Subject)
                .OrderByDescending(b => b.StartTime)
                .ToListAsync();
        }

        public async Task<IEnumerable<Booking>> GetBookingsWithDetailsAsync()
        {
            return await _dbSet
                .Include(b => b.Student)
                    .ThenInclude(s => s.User)
                .Include(b => b.Tutor)
                    .ThenInclude(t => t.User)
                .Include(b => b.Subject)
                .OrderByDescending(b => b.CreatedAt)
                .ToListAsync();
        }

        public async Task<Booking> GetBookingWithDetailsAsync(long id)
        {
            return await _dbSet
                .Include(b => b.Student)
                    .ThenInclude(s => s.User)
                .Include(b => b.Tutor)
                    .ThenInclude(t => t.User)
                .Include(b => b.Subject)
                .FirstOrDefaultAsync(b => b.Id == id);
        }

        public async Task<IEnumerable<Booking>> GetBookingsByDateRangeAsync(DateTime startDate, DateTime endDate)
        {
            return await _dbSet
                .Where(b => b.StartTime >= startDate && b.StartTime <= endDate)
                .Include(b => b.Student)
                    .ThenInclude(s => s.User)
                .Include(b => b.Tutor)
                    .ThenInclude(t => t.User)
                .Include(b => b.Subject)
                .OrderBy(b => b.StartTime)
                .ToListAsync();
        }

        public async Task<IEnumerable<Booking>> GetUpcomingBookingsAsync(long userId, bool isTutor = false)
        {
            var now = DateTime.UtcNow;
            var query = _dbSet.Where(b => b.StartTime > now);

            if (isTutor)
                query = query.Where(b => b.TutorId == userId);
            else
                query = query.Where(b => b.StudentId == userId);

            return await query
                .Include(b => b.Student)
                    .ThenInclude(s => s.User)
                .Include(b => b.Tutor)
                    .ThenInclude(t => t.User)
                .Include(b => b.Subject)
                .OrderBy(b => b.StartTime)
                .ToListAsync();
        }

        public async Task<IEnumerable<Booking>> GetPastBookingsAsync(long userId, bool isTutor = false)
        {
            var now = DateTime.UtcNow;
            var query = _dbSet.Where(b => b.EndTime < now);

            if (isTutor)
                query = query.Where(b => b.TutorId == userId);
            else
                query = query.Where(b => b.StudentId == userId);

            return await query
                .Include(b => b.Student)
                    .ThenInclude(s => s.User)
                .Include(b => b.Tutor)
                    .ThenInclude(t => t.User)
                .Include(b => b.Subject)
                .OrderByDescending(b => b.StartTime)
                .ToListAsync();
        }

        public async Task<bool> HasConflictingBookingAsync(long tutorId, DateTime startTime, DateTime endTime, long? excludeBookingId = null)
        {
            var query = _dbSet.Where(b =>
                b.TutorId == tutorId &&
                b.Status != BookingStatus.Cancelled &&
                ((b.StartTime < endTime && b.EndTime > startTime)));

            if (excludeBookingId.HasValue)
                query = query.Where(b => b.Id != excludeBookingId.Value);

            return await query.AnyAsync();
        }

        public async Task<IEnumerable<Booking>> GetTutorBookingsForDateAsync(long tutorId, DateTime date)
        {
            var startOfDay = date.Date;
            var endOfDay = startOfDay.AddDays(1);

            return await _dbSet
                .Where(b => b.TutorId == tutorId &&
                           b.StartTime >= startOfDay &&
                           b.StartTime < endOfDay &&
                           b.Status != BookingStatus.Cancelled)
                .Include(b => b.Student)
                    .ThenInclude(s => s.User)
                .Include(b => b.Subject)
                .OrderBy(b => b.StartTime)
                .ToListAsync();
        }

        public async Task<IEnumerable<Booking>> GetPendingBookingsAsync()
        {
            return await _dbSet
                .Where(b => b.Status == BookingStatus.Pending)
                .Include(b => b.Student)
                    .ThenInclude(s => s.User)
                .Include(b => b.Tutor)
                    .ThenInclude(t => t.User)
                .Include(b => b.Subject)
                .OrderBy(b => b.CreatedAt)
                .ToListAsync();
        }

        public async Task<bool> UpdateBookingStatusAsync(long id, BookingStatus status)
        {
            var booking = await _dbSet.FindAsync(id);
            if (booking == null)
                return false;

            booking.Status = status;
            await _context.SaveChangesAsync();
            return true;
        }
    }
}