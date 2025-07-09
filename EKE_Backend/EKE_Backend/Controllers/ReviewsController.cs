//using Microsoft.AspNetCore.Authorization;
//using Microsoft.AspNetCore.Http;
//using Microsoft.AspNetCore.Mvc;
//using Service.DTO.Request;
//using Service.DTO.Response;
//using Service.Services.Reviews;
//using System.Security.Claims;

//namespace EKE_Backend.Controllers
//{
//    [ApiController]
//    [Route("api/[controller]")]
//    [Authorize]
//    public class ReviewsController : ControllerBase
//    {
//        private readonly IReviewService _reviewService;
//        private readonly ILogger<ReviewsController> _logger;

//        public ReviewsController(IReviewService reviewService, ILogger<ReviewsController> logger)
//        {
//            _reviewService = reviewService;
//            _logger = logger;
//        }

//        /// <summary>
//        /// Create a new review
//        /// </summary>
//        [HttpPost]
//        public async Task<ActionResult<ReviewResponseDto>> CreateReview([FromBody] CreateReviewRequestDto request)
//        {
//            try
//            {
//                var studentId = GetCurrentUserId();
//                var review = await _reviewService.CreateReviewAsync(studentId, request);
//                return Ok(review);
//            }
//            catch (InvalidOperationException ex)
//            {
//                return BadRequest(new { message = ex.Message });
//            }
//            catch (ArgumentException ex)
//            {
//                return BadRequest(new { message = ex.Message });
//            }
//            catch (Exception ex)
//            {
//                _logger.LogError(ex, "Error creating review");
//                return StatusCode(500, new { message = "An error occurred while creating the review" });
//            }
//        }

//        /// <summary>
//        /// Update an existing review
//        /// </summary>
//        [HttpPut("{reviewId}")]
//        public async Task<ActionResult<ReviewResponseDto>> UpdateReview(long reviewId, [FromBody] UpdateReviewRequestDto request)
//        {
//            try
//            {
//                var studentId = GetCurrentUserId();
//                var review = await _reviewService.UpdateReviewAsync(reviewId, studentId, request);
//                return Ok(review);
//            }
//            catch (ArgumentException ex)
//            {
//                return BadRequest(new { message = ex.Message });
//            }
//            catch (UnauthorizedAccessException ex)
//            {
//                return Forbid(ex.Message);
//            }
//            catch (Exception ex)
//            {
//                _logger.LogError(ex, "Error updating review {ReviewId}", reviewId);
//                return StatusCode(500, new { message = "An error occurred while updating the review" });
//            }
//        }

//        /// <summary>
//        /// Delete a review
//        /// </summary>
//        [HttpDelete("{reviewId}")]
//        public async Task<ActionResult> DeleteReview(long reviewId)
//        {
//            try
//            {
//                var studentId = GetCurrentUserId();
//                var result = await _reviewService.DeleteReviewAsync(reviewId, studentId);

//                if (!result)
//                {
//                    return NotFound(new { message = "Review not found" });
//                }

//                return NoContent();
//            }
//            catch (UnauthorizedAccessException ex)
//            {
//                return Forbid(ex.Message);
//            }
//            catch (Exception ex)
//            {
//                _logger.LogError(ex, "Error deleting review {ReviewId}", reviewId);
//                return StatusCode(500, new { message = "An error occurred while deleting the review" });
//            }
//        }

//        /// <summary>
//        /// Get review by ID
//        /// </summary>
//        [HttpGet("{reviewId}")]
//        public async Task<ActionResult<ReviewResponseDto>> GetReviewById(long reviewId)
//        {
//            try
//            {
//                var review = await _reviewService.GetReviewByIdAsync(reviewId);

//                if (review == null)
//                {
//                    return NotFound(new { message = "Review not found" });
//                }

//                return Ok(review);
//            }
//            catch (Exception ex)
//            {
//                _logger.LogError(ex, "Error getting review {ReviewId}", reviewId);
//                return StatusCode(500, new { message = "An error occurred while retrieving the review" });
//            }
//        }

//        /// <summary>
//        /// Get all reviews for a tutor
//        /// </summary>
//        [HttpGet("tutor/{tutorId}")]
//        [AllowAnonymous]
//        public async Task<ActionResult<IEnumerable<ReviewResponseDto>>> GetReviewsByTutorId(long tutorId)
//        {
//            try
//            {
//                var reviews = await _reviewService.GetApprovedReviewsByTutorIdAsync(tutorId);
//                return Ok(reviews);
//            }
//            catch (Exception ex)
//            {
//                _logger.LogError(ex, "Error getting reviews for tutor {TutorId}", tutorId);
//                return StatusCode(500, new { message = "An error occurred while retrieving reviews" });
//            }
//        }

//        /// <summary>
//        /// Get all reviews by current student
//        /// </summary>
//        [HttpGet("my-reviews")]
//        public async Task<ActionResult<IEnumerable<ReviewResponseDto>>> GetMyReviews()
//        {
//            try
//            {
//                var studentId = GetCurrentUserId();
//                var reviews = await _reviewService.GetReviewsByStudentIdAsync(studentId);
//                return Ok(reviews);
//            }
//            catch (Exception ex)
//            {
//                _logger.LogError(ex, "Error getting reviews for current student");
//                return StatusCode(500, new { message = "An error occurred while retrieving your reviews" });
//            }
//        }

//        /// <summary>
//        /// Get review statistics for a tutor
//        /// </summary>
//        [HttpGet("tutor/{tutorId}/statistics")]
//        [AllowAnonymous]
//        public async Task<ActionResult<ReviewStatisticsDto>> GetReviewStatistics(long tutorId)
//        {
//            try
//            {
//                var statistics = await _reviewService.GetReviewStatisticsByTutorIdAsync(tutorId);
//                return Ok(statistics);
//            }
//            catch (Exception ex)
//            {
//                _logger.LogError(ex, "Error getting review statistics for tutor {TutorId}", tutorId);
//                return StatusCode(500, new { message = "An error occurred while retrieving review statistics" });
//            }
//        }

//        /// <summary>
//        /// Check if current student has reviewed a tutor
//        /// </summary>
//        [HttpGet("tutor/{tutorId}/my-review")]
//        public async Task<ActionResult<ReviewResponseDto>> GetMyReviewForTutor(long tutorId)
//        {
//            try
//            {
//                var studentId = GetCurrentUserId();
//                var review = await _reviewService.GetReviewByTutorAndStudentAsync(tutorId, studentId);

//                if (review == null)
//                {
//                    return NotFound(new { message = "You haven't reviewed this tutor yet" });
//                }

//                return Ok(review);
//            }
//            catch (Exception ex)
//            {
//                _logger.LogError(ex, "Error getting review for tutor {TutorId} by current student", tutorId);
//                return StatusCode(500, new { message = "An error occurred while retrieving your review" });
//            }
//        }

//        /// <summary>
//        /// Check if current student has reviewed a tutor (boolean result)
//        /// </summary>
//        [HttpGet("tutor/{tutorId}/has-reviewed")]
//        public async Task<ActionResult<bool>> HasReviewedTutor(long tutorId)
//        {
//            try
//            {
//                var studentId = GetCurrentUserId();
//                var hasReviewed = await _reviewService.HasStudentReviewedTutorAsync(studentId, tutorId);
//                return Ok(hasReviewed);
//            }
//            catch (Exception ex)
//            {
//                _logger.LogError(ex, "Error checking if student has reviewed tutor {TutorId}", tutorId);
//                return StatusCode(500, new { message = "An error occurred while checking review status" });
//            }
//        }

//        /// <summary>
//        /// Get reviews by rating (Admin only)
//        /// </summary>
//        [HttpGet("rating/{rating}")]
//        [Authorize(Roles = "Admin")]
//        public async Task<ActionResult<IEnumerable<ReviewResponseDto>>> GetReviewsByRating(int rating)
//        {
//            try
//            {
//                if (rating < 1 || rating > 5)
//                {
//                    return BadRequest(new { message = "Rating must be between 1 and 5" });
//                }

//                var reviews = await _reviewService.GetReviewsByRatingAsync(rating);
//                return Ok(reviews);
//            }
//            catch (Exception ex)
//            {
//                _logger.LogError(ex, "Error getting reviews with rating {Rating}", rating);
//                return StatusCode(500, new { message = "An error occurred while retrieving reviews" });
//            }
//        }

//        /// <summary>
//        /// Get pending reviews (Admin only)
//        /// </summary>
//        [HttpGet("pending")]
//        [Authorize(Roles = "Admin")]
//        public async Task<ActionResult<IEnumerable<ReviewResponseDto>>> GetPendingReviews()
//        {
//            try
//            {
//                var reviews = await _reviewService.GetPendingReviewsAsync();
//                return Ok(reviews);
//            }
//            catch (Exception ex)
//            {
//                _logger.LogError(ex, "Error getting pending reviews");
//                return StatusCode(500, new { message = "An error occurred while retrieving pending reviews" });
//            }
//        }

//        /// <summary>
//        /// Approve or reject a review (Admin only)
//        /// </summary>
//        [HttpPut("{reviewId}/approve")]
//        [Authorize(Roles = "Admin")]
//        public async Task<ActionResult> ApproveReview(long reviewId, [FromBody] ApproveReviewRequestDto request)
//        {
//            try
//            {
//                var result = await _reviewService.ApproveReviewAsync(reviewId, request);

//                if (!result)
//                {
//                    return NotFound(new { message = "Review not found" });
//                }

//                return Ok(new { message = request.IsApproved ? "Review approved successfully" : "Review rejected successfully" });
//            }
//            catch (Exception ex)
//            {
//                _logger.LogError(ex, "Error approving review {ReviewId}", reviewId);
//                return StatusCode(500, new { message = "An error occurred while processing the review" });
//            }
//        }

//        /// <summary>
//        /// Get all reviews for a tutor (Admin only - includes unapproved)
//        /// </summary>
//        [HttpGet("tutor/{tutorId}/admin")]
//        [Authorize(Roles = "Admin")]
//        public async Task<ActionResult<IEnumerable<ReviewResponseDto>>> GetAllReviewsByTutorId(long tutorId)
//        {
//            try
//            {
//                var reviews = await _reviewService.GetReviewsByTutorIdAsync(tutorId);
//                return Ok(reviews);
//            }
//            catch (Exception ex)
//            {
//                _logger.LogError(ex, "Error getting all reviews for tutor {TutorId}", tutorId);
//                return StatusCode(500, new { message = "An error occurred while retrieving reviews" });
//            }
//        }

//        private long GetCurrentUserId()
//        {
//            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
//            if (string.IsNullOrEmpty(userIdClaim) || !long.TryParse(userIdClaim, out var userId))
//            {
//                throw new UnauthorizedAccessException("User not authenticated");
//            }
//            return userId;
//        }
//    }
//}
