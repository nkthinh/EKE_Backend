using Microsoft.Extensions.Logging;
using Repository.Entities;
using Repository.UnitOfWork;
using Service.DTO.Request;
using Service.DTO.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.Services.Reviews
{
    public class ReviewService : IReviewService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<ReviewService> _logger;

        public ReviewService(IUnitOfWork unitOfWork, ILogger<ReviewService> logger)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public async Task<ReviewResponseDto> CreateReviewAsync(long studentId, CreateReviewRequestDto request)
        {
            try
            {
                // Check if student has already reviewed this tutor
                var existingReview = await _unitOfWork.Reviews.GetReviewByTutorAndStudentAsync(request.TutorId, studentId);
                if (existingReview != null)
                {
                    throw new InvalidOperationException("You have already reviewed this tutor");
                }

                // Verify student exists
                var student = await _unitOfWork.Students.GetByIdAsync(studentId);
                if (student == null)
                {
                    throw new ArgumentException("Student not found");
                }

                // Verify tutor exists
                var tutor = await _unitOfWork.Tutors.GetByIdAsync(request.TutorId);
                if (tutor == null)
                {
                    throw new ArgumentException("Tutor not found");
                }

                var review = new Review
                {
                    TutorId = request.TutorId,
                    StudentId = studentId,
                    Rating = request.Rating,
                    Comment = request.Comment,
                    IsAnonymous = request.IsAnonymous,
                    IsApproved = true, // Auto-approve by default
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                };

                await _unitOfWork.Reviews.AddAsync(review);
                await _unitOfWork.CompleteAsync();

                // Get the review with navigation properties loaded
                var createdReview = await _unitOfWork.Reviews.GetByIdAsync(review.Id);
                return MapToResponseDto(createdReview!);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating review for student {StudentId} and tutor {TutorId}", studentId, request.TutorId);
                throw;
            }
        }

        public async Task<ReviewResponseDto> UpdateReviewAsync(long reviewId, long studentId, UpdateReviewRequestDto request)
        {
            try
            {
                var review = await _unitOfWork.Reviews.GetByIdAsync(reviewId);
                if (review == null)
                {
                    throw new ArgumentException("Review not found");
                }

                if (review.StudentId != studentId)
                {
                    throw new UnauthorizedAccessException("You can only update your own reviews");
                }

                review.Rating = request.Rating;
                review.Comment = request.Comment;
                review.IsAnonymous = request.IsAnonymous;
                review.UpdatedAt = DateTime.UtcNow;

                await _unitOfWork.Reviews.UpdateAsync(review);
                await _unitOfWork.CompleteAsync();

                // Reload the review to get updated navigation properties
                var updatedReview = await _unitOfWork.Reviews.GetByIdAsync(reviewId);
                return MapToResponseDto(updatedReview!);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating review {ReviewId} by student {StudentId}", reviewId, studentId);
                throw;
            }
        }

        public async Task<bool> DeleteReviewAsync(long reviewId, long studentId)
        {
            try
            {
                var review = await _unitOfWork.Reviews.GetByIdAsync(reviewId);
                if (review == null)
                {
                    return false;
                }

                if (review.StudentId != studentId)
                {
                    throw new UnauthorizedAccessException("You can only delete your own reviews");
                }

                await _unitOfWork.Reviews.DeleteAsync(reviewId);
                await _unitOfWork.CompleteAsync();

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting review {ReviewId} by student {StudentId}", reviewId, studentId);
                throw;
            }
        }

        public async Task<ReviewResponseDto?> GetReviewByIdAsync(long reviewId)
        {
            try
            {
                var review = await _unitOfWork.Reviews.GetByIdAsync(reviewId);
                if (review == null)
                {
                    return null;
                }

                return MapToResponseDto(review);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting review {ReviewId}", reviewId);
                throw;
            }
        }

        public async Task<IEnumerable<ReviewResponseDto>> GetReviewsByTutorIdAsync(long tutorId)
        {
            try
            {
                var reviews = await _unitOfWork.Reviews.GetReviewsByTutorIdAsync(tutorId);
                return MapToResponseDtos(reviews);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting reviews for tutor {TutorId}", tutorId);
                throw;
            }
        }

        public async Task<IEnumerable<ReviewResponseDto>> GetReviewsByStudentIdAsync(long studentId)
        {
            try
            {
                var reviews = await _unitOfWork.Reviews.GetReviewsByStudentIdAsync(studentId);
                return MapToResponseDtos(reviews);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting reviews for student {StudentId}", studentId);
                throw;
            }
        }

        public async Task<IEnumerable<ReviewResponseDto>> GetApprovedReviewsByTutorIdAsync(long tutorId)
        {
            try
            {
                var reviews = await _unitOfWork.Reviews.GetApprovedReviewsByTutorIdAsync(tutorId);
                return MapToResponseDtos(reviews);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting approved reviews for tutor {TutorId}", tutorId);
                throw;
            }
        }

        public async Task<ReviewStatisticsDto> GetReviewStatisticsByTutorIdAsync(long tutorId)
        {
            try
            {
                var reviews = await _unitOfWork.Reviews.GetApprovedReviewsByTutorIdAsync(tutorId);
                var reviewsList = reviews.ToList();

                // Get tutor info with navigation properties
                var tutor = await _unitOfWork.Tutors.GetByIdAsync(tutorId);
                var tutorName = tutor?.User?.FullName ?? "Unknown";

                var statistics = new ReviewStatisticsDto
                {
                    TutorId = tutorId,
                    TutorName = tutorName,
                    TotalReviews = reviewsList.Count,
                    AverageRating = reviewsList.Any() ? Math.Round(reviewsList.Average(r => r.Rating), 2) : 0,
                    FiveStars = reviewsList.Count(r => r.Rating == 5),
                    FourStars = reviewsList.Count(r => r.Rating == 4),
                    ThreeStars = reviewsList.Count(r => r.Rating == 3),
                    TwoStars = reviewsList.Count(r => r.Rating == 2),
                    OneStar = reviewsList.Count(r => r.Rating == 1)
                };

                return statistics;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting review statistics for tutor {TutorId}", tutorId);
                throw;
            }
        }

        public async Task<ReviewResponseDto?> GetReviewByTutorAndStudentAsync(long tutorId, long studentId)
        {
            try
            {
                var review = await _unitOfWork.Reviews.GetReviewByTutorAndStudentAsync(tutorId, studentId);
                if (review == null)
                {
                    return null;
                }

                return MapToResponseDto(review);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting review for tutor {TutorId} and student {StudentId}", tutorId, studentId);
                throw;
            }
        }

        public async Task<IEnumerable<ReviewResponseDto>> GetPendingReviewsAsync()
        {
            try
            {
                var reviews = await _unitOfWork.Reviews.GetPendingReviewsAsync();
                return MapToResponseDtos(reviews);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting pending reviews");
                throw;
            }
        }

        public async Task<bool> ApproveReviewAsync(long reviewId, ApproveReviewRequestDto request)
        {
            try
            {
                var review = await _unitOfWork.Reviews.GetByIdAsync(reviewId);
                if (review == null)
                {
                    return false;
                }

                review.IsApproved = request.IsApproved;
                review.UpdatedAt = DateTime.UtcNow;

                await _unitOfWork.Reviews.UpdateAsync(review);
                await _unitOfWork.CompleteAsync();

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error approving review {ReviewId}", reviewId);
                throw;
            }
        }

        public async Task<bool> HasStudentReviewedTutorAsync(long studentId, long tutorId)
        {
            try
            {
                return await _unitOfWork.Reviews.HasStudentReviewedTutorAsync(studentId, tutorId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error checking if student {StudentId} has reviewed tutor {TutorId}", studentId, tutorId);
                throw;
            }
        }

        public async Task<IEnumerable<ReviewResponseDto>> GetReviewsByRatingAsync(int rating)
        {
            try
            {
                var reviews = await _unitOfWork.Reviews.GetReviewsByRatingAsync(rating);
                return MapToResponseDtos(reviews);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting reviews with rating {Rating}", rating);
                throw;
            }
        }

        private ReviewResponseDto MapToResponseDto(Review review)
        {
            return new ReviewResponseDto
            {
                Id = review.Id,
                TutorId = review.TutorId,
                TutorName = review.Tutor?.User?.FullName ?? "Unknown",
                StudentId = review.StudentId,
                StudentName = review.IsAnonymous ? "Anonymous" : (review.Student?.User?.FullName ?? "Unknown"),
                Rating = review.Rating,
                Comment = review.Comment,
                IsAnonymous = review.IsAnonymous,
                IsApproved = review.IsApproved,
                CreatedAt = review.CreatedAt,
                UpdatedAt = review.UpdatedAt
            };
        }

        private IEnumerable<ReviewResponseDto> MapToResponseDtos(IEnumerable<Review> reviews)
        {
            return reviews.Select(MapToResponseDto);
        }
    }
}