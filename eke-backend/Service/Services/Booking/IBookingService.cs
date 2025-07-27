using Application.DTOs;
using Repository.Enums;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Application.Services.Interfaces
{
    public interface IBookingService
    {
        Task<BookingResponseDto> GetByIdAsync(long id);
        Task<IEnumerable<BookingResponseDto>> GetAllAsync();
        Task<BookingResponseDto> CreateAsync(BookingRequestDto requestDto);
        Task<BookingResponseDto> UpdateAsync(long id, BookingRequestDto requestDto);
        Task<bool> DeleteAsync(long id);
        Task<IEnumerable<BookingResponseDto>> GetBookingsByStudentAsync(long studentId);
        Task<IEnumerable<BookingResponseDto>> GetBookingsByTutorAsync(long tutorId);
        Task<IEnumerable<BookingResponseDto>> GetBookingsBySubjectAsync(long subjectId);
        Task<IEnumerable<BookingResponseDto>> GetUpcomingBookingsAsync(long userId, bool isTutor = false);
        Task<IEnumerable<BookingResponseDto>> GetPastBookingsAsync(long userId, bool isTutor = false);
        Task<bool> UpdateBookingStatusAsync(long id, BookingStatus status);
        Task<bool> HasConflictingBookingAsync(long tutorId, DateTime startTime, DateTime endTime, long? excludeBookingId = null);
        Task<IEnumerable<BookingResponseDto>> GetTutorScheduleAsync(long tutorId, DateTime date);
        Task<IEnumerable<BookingResponseDto>> GetPendingBookingsAsync();
    }
}