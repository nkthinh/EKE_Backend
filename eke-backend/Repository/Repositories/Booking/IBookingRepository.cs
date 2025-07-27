using Repository.Entities;
using Repository.Enums;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Repository.Repositories
{
    public interface IBookingRepository
    {
        // Base repository methods
        Task<Booking> GetByIdAsync(long id);
        Task<IEnumerable<Booking>> GetAllAsync();
        Task<IEnumerable<Booking>> FindAsync(Expression<Func<Booking, bool>> predicate);
        Task<Booking> CreateAsync(Booking entity);
        Task<Booking> UpdateAsync(Booking entity);
        Task<bool> DeleteAsync(long id);
        Task<bool> ExistsAsync(long id);
        Task<int> CountAsync();
        Task<int> CountAsync(Expression<Func<Booking, bool>> predicate);

        // Booking-specific methods
        Task<IEnumerable<Booking>> GetBookingsByStudentIdAsync(long studentId);
        Task<IEnumerable<Booking>> GetBookingsByTutorIdAsync(long tutorId);
        Task<IEnumerable<Booking>> GetBookingsBySubjectIdAsync(long subjectId);
        Task<IEnumerable<Booking>> GetBookingsByStatusAsync(BookingStatus status);
        Task<IEnumerable<Booking>> GetBookingsWithDetailsAsync();
        Task<Booking> GetBookingWithDetailsAsync(long id);
        Task<IEnumerable<Booking>> GetBookingsByDateRangeAsync(DateTime startDate, DateTime endDate);
        Task<IEnumerable<Booking>> GetUpcomingBookingsAsync(long userId, bool isTutor = false);
        Task<IEnumerable<Booking>> GetPastBookingsAsync(long userId, bool isTutor = false);
        Task<bool> HasConflictingBookingAsync(long tutorId, DateTime startTime, DateTime endTime, long? excludeBookingId = null);
        Task<IEnumerable<Booking>> GetTutorBookingsForDateAsync(long tutorId, DateTime date);
        Task<IEnumerable<Booking>> GetPendingBookingsAsync();
        Task<bool> UpdateBookingStatusAsync(long id, BookingStatus status);
    }
}