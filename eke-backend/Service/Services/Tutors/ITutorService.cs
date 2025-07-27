using Application.DTOs;
using Microsoft.AspNetCore.Http;
using Repository.Enums;
using Service.DTO.Request;
using Service.DTO.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.Services.Tutors
{
    public interface ITutorService
    {
        Task<(IEnumerable<TutorSearchResultDto> Tutors, int TotalCount)> SearchTutorsAsync(TutorSearchDto searchParams);
        Task<TutorProfileDto?> GetTutorProfileAsync(long tutorId);
        Task<TutorProfileDto?> GetCompleteTutorProfileAsync(long tutorId);
        Task<string> UpdateAvailabilityAsync(long tutorId, TutorAvailabilityUpdateDto availabilityDto);
        Task<IEnumerable<BookingScheduleDto>> GetTutorBookingsAsync(long tutorId, DateTime startDate, DateTime endDate);
        Task<TutorProfileDto> UpdateTutorProfileAsync(long tutorId, TutorUpdateDto updateDto);
        Task<bool> VerifyTutorAsync(long tutorId, TutorVerificationDto verificationDto);
        Task<bool> UpdateVerificationStatusAsync(long tutorId, TutorVerificationStatusDto statusDto);
        Task<(IEnumerable<TutorSearchResultDto> Tutors, int TotalCount)> GetTutorsBySubjectAsync(long subjectId, int page, int pageSize);
        Task<IEnumerable<TutorSearchResultDto>> GetFeaturedTutorsAsync(int limit);
        Task<(IEnumerable<TutorSearchResultDto> Tutors, int TotalCount)> GetNearbyTutorsAsync(string city, string? district, int page, int pageSize);
        Task<TutorStatisticsDto> GetTutorStatisticsAsync(long tutorId);
        Task<(IEnumerable<TutorSearchResultDto> Tutors, int TotalCount)> GetTutorsPendingVerificationAsync(int page, int pageSize);
        Task<(IEnumerable<DTO.Response.ReviewResponseDto> Reviews, int TotalCount, double AverageRating)> GetTutorReviewsAsync(long tutorId, int page, int pageSize);
        Task<IEnumerable<TutorSearchResultDto>> GetRecommendedTutorsAsync(long studentId, int limit);
        Task<(IEnumerable<TutorSearchResultDto> Tutors, int TotalCount)> GetAllTutorsAsync(int page, int pageSize, VerificationStatus? status);
        Task<string> UploadProfileImageAsync(long tutorId, IFormFile imageFile);
    }
}
