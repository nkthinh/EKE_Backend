using Service.DTO.Request;
using Service.DTO.Response;
using System.Collections.Generic;
using System.Threading.Tasks;
using Repository;
using Microsoft.EntityFrameworkCore;
using Application.DTOs;

namespace Service.Services.Tutors
{
    public class TutorService : ITutorService
    {
        private readonly ApplicationDbContext _dbContext;

        public TutorService(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<(IEnumerable<TutorSearchResultDto> Tutors, int TotalCount)> SearchTutorsAsync(TutorSearchDto searchParams)
        {
            var query = _dbContext.Tutors
                .Include(t => t.User)
                .AsQueryable();

            var totalCount = await query.CountAsync();
            var tutors = await query
                .Skip((searchParams.Page - 1) * searchParams.PageSize)
                .Take(searchParams.PageSize)
                .Select(t => new TutorSearchResultDto
                {
                    Id = t.Id,
                    FullName = t.User.FullName,
                    ProfileImage = t.User.ProfileImage,
                    AverageRating = t.AverageRating,
                    University = t.University,
                    Major = t.Major,
                })
                .ToListAsync();

            return (tutors, totalCount);
        }

        public Task<TutorProfileDto?> GetTutorProfileAsync(long tutorId)
            => throw new System.NotImplementedException();

        public Task<TutorProfileDto?> GetCompleteTutorProfileAsync(long tutorId)
            => throw new System.NotImplementedException();

        public Task<string> UpdateAvailabilityAsync(long tutorId, TutorAvailabilityUpdateDto availabilityDto)
            => throw new System.NotImplementedException();

        public Task<IEnumerable<BookingScheduleDto>> GetTutorBookingsAsync(long tutorId, System.DateTime startDate, System.DateTime endDate)
            => throw new System.NotImplementedException();

        public Task<TutorProfileDto> UpdateTutorProfileAsync(long tutorId, TutorUpdateDto updateDto)
            => throw new System.NotImplementedException();

        public Task<bool> VerifyTutorAsync(long tutorId, TutorVerificationDto verificationDto)
            => throw new System.NotImplementedException();

        public Task<bool> UpdateVerificationStatusAsync(long tutorId, TutorVerificationStatusDto statusDto)
            => throw new System.NotImplementedException();

        public Task<(IEnumerable<TutorSearchResultDto> Tutors, int TotalCount)> GetTutorsBySubjectAsync(long subjectId, int page, int pageSize)
            => throw new System.NotImplementedException();

        public Task<IEnumerable<TutorSearchResultDto>> GetFeaturedTutorsAsync(int limit)
            => throw new System.NotImplementedException();

        public Task<(IEnumerable<TutorSearchResultDto> Tutors, int TotalCount)> GetNearbyTutorsAsync(string city, string? district, int page, int pageSize)
            => throw new System.NotImplementedException();

        public Task<TutorStatisticsDto> GetTutorStatisticsAsync(long tutorId)
            => throw new System.NotImplementedException();

        public Task<(IEnumerable<TutorSearchResultDto> Tutors, int TotalCount)> GetTutorsPendingVerificationAsync(int page, int pageSize)
            => throw new System.NotImplementedException();

        public Task<(IEnumerable<DTO.Response.ReviewResponseDto> Reviews, int TotalCount, double AverageRating)> GetTutorReviewsAsync(long tutorId, int page, int pageSize)
            => throw new System.NotImplementedException();

        public Task<IEnumerable<TutorSearchResultDto>> GetRecommendedTutorsAsync(long studentId, int limit)
            => throw new System.NotImplementedException();

        public Task<(IEnumerable<TutorSearchResultDto> Tutors, int TotalCount)> GetAllTutorsAsync(int page, int pageSize, Repository.Enums.VerificationStatus? status)
            => throw new System.NotImplementedException();

        public Task<string> UploadProfileImageAsync(long tutorId, Microsoft.AspNetCore.Http.IFormFile imageFile)
            => throw new System.NotImplementedException();
    }
}
