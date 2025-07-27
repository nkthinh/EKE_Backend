using Service.DTO.Request;
using Service.DTO.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.Services.Reviews
{
    public interface IReviewService
    {
        Task<ReviewResponseDto> CreateReviewAsync(long studentId, CreateReviewRequestDto request);
        Task<ReviewResponseDto> UpdateReviewAsync(long reviewId, long studentId, UpdateReviewRequestDto request);
        Task<bool> DeleteReviewAsync(long reviewId, long studentId);
        Task<ReviewResponseDto?> GetReviewByIdAsync(long reviewId);
        Task<IEnumerable<ReviewResponseDto>> GetReviewsByTutorIdAsync(long tutorId);
        Task<IEnumerable<ReviewResponseDto>> GetReviewsByStudentIdAsync(long studentId);
        Task<IEnumerable<ReviewResponseDto>> GetApprovedReviewsByTutorIdAsync(long tutorId);
        Task<ReviewStatisticsDto> GetReviewStatisticsByTutorIdAsync(long tutorId);
        Task<ReviewResponseDto?> GetReviewByTutorAndStudentAsync(long tutorId, long studentId);
        Task<IEnumerable<ReviewResponseDto>> GetPendingReviewsAsync();
        Task<bool> ApproveReviewAsync(long reviewId, ApproveReviewRequestDto request);
        Task<bool> HasStudentReviewedTutorAsync(long studentId, long tutorId);
        Task<IEnumerable<ReviewResponseDto>> GetReviewsByRatingAsync(int rating);
    }
}
