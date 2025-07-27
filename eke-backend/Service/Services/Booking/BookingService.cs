using Application.DTOs;
using Application.Services.Interfaces;
using Repository.Entities;
using Repository.Enums;
using Repository.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Application.Services
{
    public class BookingService : IBookingService
    {
        private readonly IBookingRepository _bookingRepository;

        public BookingService(IBookingRepository bookingRepository)
        {
            _bookingRepository = bookingRepository;
        }

        public async Task<BookingResponseDto> GetByIdAsync(long id)
        {
            var booking = await _bookingRepository.GetBookingWithDetailsAsync(id);
            if (booking == null)
                throw new KeyNotFoundException($"Booking with ID {id} not found");

            return MapToResponseDto(booking);
        }

        public async Task<IEnumerable<BookingResponseDto>> GetAllAsync()
        {
            var bookings = await _bookingRepository.GetBookingsWithDetailsAsync();
            return bookings.Select(MapToResponseDto);
        }

        public async Task<BookingResponseDto> CreateAsync(BookingRequestDto requestDto)
        {
            // Validate booking times
            if (requestDto.StartTime >= requestDto.EndTime)
                throw new ArgumentException("Start time must be before end time");

            if (requestDto.StartTime <= DateTime.UtcNow)
                throw new ArgumentException("Booking must be scheduled for a future time");

            // Check for conflicting bookings
            var hasConflict = await _bookingRepository.HasConflictingBookingAsync(
                requestDto.TutorId,
                requestDto.StartTime,
                requestDto.EndTime);

            if (hasConflict)
                throw new InvalidOperationException("The tutor already has a booking that conflicts with this time slot");

            var booking = new Booking
            {
                StudentId = requestDto.StudentId,
                TutorId = requestDto.TutorId,
                SubjectId = requestDto.SubjectId,
                StartTime = requestDto.StartTime,
                EndTime = requestDto.EndTime,
                LocationType = requestDto.LocationType,
                LocationAddress = requestDto.LocationAddress,
                Status = requestDto.Status ?? BookingStatus.Pending,
                TotalAmount = requestDto.TotalAmount,
                Notes = requestDto.Notes
            };

            var createdBooking = await _bookingRepository.CreateAsync(booking);
            var detailedBooking = await _bookingRepository.GetBookingWithDetailsAsync(createdBooking.Id);
            return MapToResponseDto(detailedBooking);
        }

        public async Task<BookingResponseDto> UpdateAsync(long id, BookingRequestDto requestDto)
        {
            var booking = await _bookingRepository.GetByIdAsync(id);
            if (booking == null)
                throw new KeyNotFoundException($"Booking with ID {id} not found");

            // Validate booking times
            if (requestDto.StartTime >= requestDto.EndTime)
                throw new ArgumentException("Start time must be before end time");

            // Check for conflicting bookings (excluding current booking)
            var hasConflict = await _bookingRepository.HasConflictingBookingAsync(
                requestDto.TutorId,
                requestDto.StartTime,
                requestDto.EndTime,
                id);

            if (hasConflict)
                throw new InvalidOperationException("The tutor already has a booking that conflicts with this time slot");

            booking.StudentId = requestDto.StudentId;
            booking.TutorId = requestDto.TutorId;
            booking.SubjectId = requestDto.SubjectId;
            booking.StartTime = requestDto.StartTime;
            booking.EndTime = requestDto.EndTime;
            booking.LocationType = requestDto.LocationType;
            booking.LocationAddress = requestDto.LocationAddress;
            if (requestDto.Status.HasValue)
                booking.Status = requestDto.Status.Value;
            booking.TotalAmount = requestDto.TotalAmount;
            booking.Notes = requestDto.Notes;

            await _bookingRepository.UpdateAsync(booking);
            var updatedBooking = await _bookingRepository.GetBookingWithDetailsAsync(id);
            return MapToResponseDto(updatedBooking);
        }

        public async Task<bool> DeleteAsync(long id)
        {
            return await _bookingRepository.DeleteAsync(id);
        }

        public async Task<IEnumerable<BookingResponseDto>> GetBookingsByStudentAsync(long studentId)
        {
            var bookings = await _bookingRepository.GetBookingsByStudentIdAsync(studentId);
            return bookings.Select(MapToResponseDto);
        }

        public async Task<IEnumerable<BookingResponseDto>> GetBookingsByTutorAsync(long tutorId)
        {
            var bookings = await _bookingRepository.GetBookingsByTutorIdAsync(tutorId);
            return bookings.Select(MapToResponseDto);
        }

        public async Task<IEnumerable<BookingResponseDto>> GetBookingsBySubjectAsync(long subjectId)
        {
            var bookings = await _bookingRepository.GetBookingsBySubjectIdAsync(subjectId);
            return bookings.Select(MapToResponseDto);
        }

        public async Task<IEnumerable<BookingResponseDto>> GetUpcomingBookingsAsync(long userId, bool isTutor = false)
        {
            var bookings = await _bookingRepository.GetUpcomingBookingsAsync(userId, isTutor);
            return bookings.Select(MapToResponseDto);
        }

        public async Task<IEnumerable<BookingResponseDto>> GetPastBookingsAsync(long userId, bool isTutor = false)
        {
            var bookings = await _bookingRepository.GetPastBookingsAsync(userId, isTutor);
            return bookings.Select(MapToResponseDto);
        }

        public async Task<bool> UpdateBookingStatusAsync(long id, BookingStatus status)
        {
            return await _bookingRepository.UpdateBookingStatusAsync(id, status);
        }

        public async Task<bool> HasConflictingBookingAsync(long tutorId, DateTime startTime, DateTime endTime, long? excludeBookingId = null)
        {
            return await _bookingRepository.HasConflictingBookingAsync(tutorId, startTime, endTime, excludeBookingId);
        }

        public async Task<IEnumerable<BookingResponseDto>> GetTutorScheduleAsync(long tutorId, DateTime date)
        {
            var bookings = await _bookingRepository.GetTutorBookingsForDateAsync(tutorId, date);
            return bookings.Select(MapToResponseDto);
        }

        public async Task<IEnumerable<BookingResponseDto>> GetPendingBookingsAsync()
        {
            var bookings = await _bookingRepository.GetPendingBookingsAsync();
            return bookings.Select(MapToResponseDto);
        }

        private BookingResponseDto MapToResponseDto(Booking booking)
        {
            var duration = (booking.EndTime - booking.StartTime).TotalHours;

            return new BookingResponseDto
            {
                Id = booking.Id,
                StudentId = booking.StudentId,
                TutorId = booking.TutorId,
                SubjectId = booking.SubjectId,
                StartTime = booking.StartTime,
                EndTime = booking.EndTime,
                LocationType = booking.LocationType,
                LocationAddress = booking.LocationAddress,
                Status = booking.Status,
                TotalAmount = booking.TotalAmount,
                Notes = booking.Notes,
                CreatedAt = booking.CreatedAt,
                UpdatedAt = booking.UpdatedAt,
                StudentName = booking.Student?.User?.FullName,
                TutorName = booking.Tutor?.User?.FullName,
                SubjectName = booking.Subject?.Name,
                TutorHourlyRate = booking.Tutor?.HourlyRate,
                DurationHours = duration
            };
        }
    }
}