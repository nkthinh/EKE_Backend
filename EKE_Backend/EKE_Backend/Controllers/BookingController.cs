using Application.DTOs;
using Application.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Repository.Enums;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace WebAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class BookingController : ControllerBase
    {
        private readonly IBookingService _bookingService;

        public BookingController(IBookingService bookingService)
        {
            _bookingService = bookingService;
        }

        /// <summary>
        /// Get all bookings
        /// </summary>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<BookingResponseDto>>> GetAllBookings()
        {
            try
            {
                var bookings = await _bookingService.GetAllAsync();
                return Ok(bookings);
            }
            catch (Exception ex)
            {
                return BadRequest($"Error retrieving bookings: {ex.Message}");
            }
        }

        /// <summary>
        /// Get booking by ID
        /// </summary>
        [HttpGet("{id}")]
        public async Task<ActionResult<BookingResponseDto>> GetBooking(long id)
        {
            try
            {
                var booking = await _bookingService.GetByIdAsync(id);
                return Ok(booking);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                return BadRequest($"Error retrieving booking: {ex.Message}");
            }
        }

        /// <summary>
        /// Create a new booking
        /// </summary>
        [HttpPost]
        public async Task<ActionResult<BookingResponseDto>> CreateBooking([FromBody] BookingRequestDto requestDto)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                var booking = await _bookingService.CreateAsync(requestDto);
                return CreatedAtAction(nameof(GetBooking), new { id = booking.Id }, booking);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(ex.Message);
            }
            catch (Exception ex)
            {
                return BadRequest($"Error creating booking: {ex.Message}");
            }
        }

        /// <summary>
        /// Update an existing booking
        /// </summary>
        [HttpPut("{id}")]
        public async Task<ActionResult<BookingResponseDto>> UpdateBooking(long id, [FromBody] BookingRequestDto requestDto)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                var booking = await _bookingService.UpdateAsync(id, requestDto);
                return Ok(booking);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(ex.Message);
            }
            catch (Exception ex)
            {
                return BadRequest($"Error updating booking: {ex.Message}");
            }
        }

        /// <summary>
        /// Delete a booking
        /// </summary>
        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteBooking(long id)
        {
            try
            {
                var result = await _bookingService.DeleteAsync(id);
                if (!result)
                    return NotFound($"Booking with ID {id} not found");

                return NoContent();
            }
            catch (Exception ex)
            {
                return BadRequest($"Error deleting booking: {ex.Message}");
            }
        }

        /// <summary>
        /// Get bookings by student ID
        /// </summary>
        [HttpGet("student/{studentId}")]
        public async Task<ActionResult<IEnumerable<BookingResponseDto>>> GetBookingsByStudent(long studentId)
        {
            try
            {
                var bookings = await _bookingService.GetBookingsByStudentAsync(studentId);
                return Ok(bookings);
            }
            catch (Exception ex)
            {
                return BadRequest($"Error retrieving student bookings: {ex.Message}");
            }
        }

        /// <summary>
        /// Get bookings by tutor ID
        /// </summary>
        [HttpGet("tutor/{tutorId}")]
        public async Task<ActionResult<IEnumerable<BookingResponseDto>>> GetBookingsByTutor(long tutorId)
        {
            try
            {
                var bookings = await _bookingService.GetBookingsByTutorAsync(tutorId);
                return Ok(bookings);
            }
            catch (Exception ex)
            {
                return BadRequest($"Error retrieving tutor bookings: {ex.Message}");
            }
        }

        /// <summary>
        /// Get upcoming bookings for a user
        /// </summary>
        [HttpGet("upcoming/{userId}")]
        public async Task<ActionResult<IEnumerable<BookingResponseDto>>> GetUpcomingBookings(long userId, [FromQuery] bool isTutor = false)
        {
            try
            {
                var bookings = await _bookingService.GetUpcomingBookingsAsync(userId, isTutor);
                return Ok(bookings);
            }
            catch (Exception ex)
            {
                return BadRequest($"Error retrieving upcoming bookings: {ex.Message}");
            }
        }

        /// <summary>
        /// Get past bookings for a user
        /// </summary>
        [HttpGet("past/{userId}")]
        public async Task<ActionResult<IEnumerable<BookingResponseDto>>> GetPastBookings(long userId, [FromQuery] bool isTutor = false)
        {
            try
            {
                var bookings = await _bookingService.GetPastBookingsAsync(userId, isTutor);
                return Ok(bookings);
            }
            catch (Exception ex)
            {
                return BadRequest($"Error retrieving past bookings: {ex.Message}");
            }
        }

        /// <summary>
        /// Update booking status
        /// </summary>
        [HttpPatch("{id}/status")]
        public async Task<ActionResult> UpdateBookingStatus(long id, [FromBody] UpdateBookingStatusRequest request)
        {
            try
            {
                var result = await _bookingService.UpdateBookingStatusAsync(id, request.Status);
                if (!result)
                    return NotFound($"Booking with ID {id} not found");

                return Ok(new { message = "Booking status updated successfully" });
            }
            catch (Exception ex)
            {
                return BadRequest($"Error updating booking status: {ex.Message}");
            }
        }

        /// <summary>
        /// Check for conflicting bookings
        /// </summary>
        [HttpPost("check-conflict")]
        public async Task<ActionResult> CheckConflictingBooking([FromBody] ConflictCheckRequest request)
        {
            try
            {
                var hasConflict = await _bookingService.HasConflictingBookingAsync(
                    request.TutorId,
                    request.StartTime,
                    request.EndTime,
                    request.ExcludeBookingId);

                return Ok(new { hasConflict });
            }
            catch (Exception ex)
            {
                return BadRequest($"Error checking conflicts: {ex.Message}");
            }
        }

        /// <summary>
        /// Get tutor's schedule for a specific date
        /// </summary>
        [HttpGet("tutor/{tutorId}/schedule")]
        public async Task<ActionResult<IEnumerable<BookingResponseDto>>> GetTutorSchedule(long tutorId, [FromQuery] DateTime date)
        {
            try
            {
                var bookings = await _bookingService.GetTutorScheduleAsync(tutorId, date);
                return Ok(bookings);
            }
            catch (Exception ex)
            {
                return BadRequest($"Error retrieving tutor schedule: {ex.Message}");
            }
        }

        /// <summary>
        /// Get all pending bookings
        /// </summary>
        [HttpGet("pending")]
        public async Task<ActionResult<IEnumerable<BookingResponseDto>>> GetPendingBookings()
        {
            try
            {
                var bookings = await _bookingService.GetPendingBookingsAsync();
                return Ok(bookings);
            }
            catch (Exception ex)
            {
                return BadRequest($"Error retrieving pending bookings: {ex.Message}");
            }
        }

        /// <summary>
        /// Get booking statistics
        /// </summary>
        [HttpGet("statistics")]
        public async Task<ActionResult> GetBookingStatistics()
        {
            try
            {
                var allBookings = await _bookingService.GetAllAsync();
                var now = DateTime.UtcNow;

                var statistics = new
                {
                    TotalBookings = allBookings.Count(),
                    PendingBookings = allBookings.Count(b => b.Status == BookingStatus.Pending),
                    ConfirmedBookings = allBookings.Count(b => b.Status == BookingStatus.Confirmed),
                    CompletedBookings = allBookings.Count(b => b.Status == BookingStatus.Completed),
                    CancelledBookings = allBookings.Count(b => b.Status == BookingStatus.Cancelled),
                    UpcomingBookings = allBookings.Count(b => b.StartTime > now && b.Status == BookingStatus.Confirmed),
                    BookingsToday = allBookings.Count(b => b.StartTime.Date == now.Date),
                    BookingsThisWeek = allBookings.Count(b => b.StartTime >= now.AddDays(-7)),
                    TotalRevenue = allBookings.Where(b => b.Status == BookingStatus.Completed).Sum(b => b.TotalAmount ?? 0)
                };

                return Ok(statistics);
            }
            catch (Exception ex)
            {
                return BadRequest($"Error retrieving statistics: {ex.Message}");
            }
        }
    }

    // Helper classes for request bodies
    public class UpdateBookingStatusRequest
    {
        public BookingStatus Status { get; set; }
    }

    public class ConflictCheckRequest
    {
        public long TutorId { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public long? ExcludeBookingId { get; set; }
    }
}