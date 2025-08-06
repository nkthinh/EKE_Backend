using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Repository.Enums;
using Service.DTO.Request;
using Service.DTO.Response;
using Service.Services.Tutors;
using System.Security.Claims;

namespace EKE_Backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TutorController : ControllerBase
    {
        private readonly ITutorService _tutorService;

        public TutorController(ITutorService tutorService)
        {
            _tutorService = tutorService;
        }

        /// <summary>
        /// Search tutors with advanced filters
        /// </summary>
        /// <param name="searchParams">Search parameters</param>
        /// <returns>List of tutors matching the criteria</returns>
        [HttpGet("search")]
        [AllowAnonymous]
        public async Task<IActionResult> SearchTutors([FromQuery] TutorSearchDto searchParams)
        {
            try
            {
                var (tutors, totalCount) = await _tutorService.SearchTutorsAsync(searchParams);

                var response = new
                {
                    success = true,
                    data = tutors,
                    filters = searchParams,
                    pagination = new
                    {
                        page = searchParams.Page,
                        pageSize = searchParams.PageSize,
                        totalCount,
                        totalPages = (int)Math.Ceiling((double)totalCount / searchParams.PageSize)
                    }
                };

                return Ok(response);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }

        /// <summary>
        /// Get tutor profile by ID
        /// </summary>
        /// <param name="id">Tutor ID</param>
        /// <returns>Tutor profile details</returns>
        [HttpGet("{id}/profile")]
        [AllowAnonymous]
        public async Task<IActionResult> GetTutorProfile(long id)
        {
            try
            {
                var tutor = await _tutorService.GetTutorProfileAsync(id);
                if (tutor == null)
                {
                    return NotFound(new { success = false, message = "Không tìm thấy gia sư" });
                }

                return Ok(new { success = true, data = tutor });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }

        /// <summary>
        /// Get tutor's complete profile (for tutor themselves or admin)
        /// </summary>
        /// <param name="id">Tutor ID</param>
        /// <returns>Complete tutor profile</returns>
        [HttpGet("{id}/complete-profile")]
        [Authorize]
        public async Task<IActionResult> GetCompleteTutorProfile(long id)
        {
            try
            {
                if (!CanAccessTutorData(id))
                {
                    return Forbid("Bạn không có quyền truy cập thông tin này");
                }

                var tutor = await _tutorService.GetCompleteTutorProfileAsync(id);
                if (tutor == null)
                {
                    return NotFound(new { success = false, message = "Không tìm thấy gia sư" });
                }

                return Ok(new { success = true, data = tutor });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }

        /// <summary>
        /// Update tutor availability (string format in current entity)
        /// </summary>
        /// <param name="id">Tutor ID</param>
        /// <param name="availabilityDto">Availability data</param>
        /// <returns>Updated availability</returns>
        [HttpPut("{id}/availability")]
        [Authorize]
        public async Task<IActionResult> UpdateAvailability(long id, [FromBody] TutorAvailabilityUpdateDto availabilityDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new { success = false, message = "Dữ liệu không hợp lệ", errors = ModelState });
            }

            try
            {
                if (!CanModifyTutorData(id))
                {
                    return Forbid("Bạn chỉ có thể cập nhật lịch của chính mình");
                }

                var result = await _tutorService.UpdateAvailabilityAsync(id, availabilityDto);
                return Ok(new { success = true, data = result, message = "Cập nhật lịch rảnh thành công" });
            }
            catch (ArgumentException ex)
            {
                return NotFound(new { success = false, message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }

        /// <summary>
        /// Get tutor's bookings for a specific date range (simplified schedule)
        /// </summary>
        /// <param name="id">Tutor ID</param>
        /// <param name="startDate">Start date</param>
        /// <param name="endDate">End date (optional, defaults to start date + 7 days)</param>
        /// <returns>Tutor's bookings</returns>
        [HttpGet("{id}/schedule")]
        [AllowAnonymous]
        public async Task<IActionResult> GetTutorSchedule(
            long id,
            [FromQuery] DateTime startDate,
            [FromQuery] DateTime? endDate = null)
        {
            try
            {
                endDate ??= startDate.AddDays(7); // Default to 7 days if not specified

                var schedule = await _tutorService.GetTutorBookingsAsync(id, startDate, endDate.Value);
                return Ok(new { success = true, data = schedule });
            }
            catch (ArgumentException ex)
            {
                return NotFound(new { success = false, message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }

        /// <summary>
        /// Update tutor profile information
        /// </summary>
        /// <param name="id">Tutor ID</param>
        /// <param name="updateDto">Update data</param>
        /// <returns>Updated tutor profile</returns>
        [HttpPut("{id}/profile")]
        [Authorize]
        public async Task<IActionResult> UpdateTutorProfile(long id, [FromBody] TutorUpdateDto updateDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new { success = false, message = "Dữ liệu không hợp lệ", errors = ModelState });
            }

            try
            {
                if (!CanModifyTutorData(id))
                {
                    return Forbid("Bạn chỉ có thể cập nhật hồ sơ của chính mình");
                }

                var updatedTutor = await _tutorService.UpdateTutorProfileAsync(id, updateDto);
                return Ok(new { success = true, data = updatedTutor, message = "Cập nhật hồ sơ thành công" });
            }
            catch (ArgumentException ex)
            {
                return NotFound(new { success = false, message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }

        /// <summary>
        /// Verify tutor - Admin only
        /// </summary>
        /// <param name="id">Tutor ID</param>
        /// <param name="verificationDto">Verification data</param>
        /// <returns>Verification result</returns>
        [HttpPost("{id}/verify")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> VerifyTutor(long id, [FromBody] TutorVerificationDto verificationDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new { success = false, message = "Dữ liệu không hợp lệ", errors = ModelState });
            }

            try
            {
                var result = await _tutorService.VerifyTutorAsync(id, verificationDto);
                if (!result)
                {
                    return NotFound(new { success = false, message = "Không tìm thấy gia sư" });
                }

                return Ok(new { success = true, message = "Xác minh gia sư thành công" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }

        /// <summary>
        /// Get tutors by subject
        /// </summary>
        /// <param name="subjectId">Subject ID</param>
        /// <param name="page">Page number</param>
        /// <param name="pageSize">Page size</param>
        /// <returns>List of tutors teaching the subject</returns>
        [HttpGet("by-subject/{subjectId}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetTutorsBySubject(
            long subjectId,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 20)
        {
            try
            {
                var (tutors, totalCount) = await _tutorService.GetTutorsBySubjectAsync(subjectId, page, pageSize);

                var response = new
                {
                    success = true,
                    data = tutors,
                    pagination = new
                    {
                        page,
                        pageSize,
                        totalCount,
                        totalPages = (int)Math.Ceiling((double)totalCount / pageSize)
                    }
                };

                return Ok(response);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }

        /// <summary>
        /// Get featured tutors (top rated, verified)
        /// </summary>
        /// <param name="limit">Number of tutors to return (default: 12)</param>
        /// <returns>List of featured tutors</returns>
        [HttpGet("featured")]
        [AllowAnonymous]
        public async Task<IActionResult> GetFeaturedTutors([FromQuery] int limit = 12)
        {
            try
            {
                var tutors = await _tutorService.GetFeaturedTutorsAsync(limit);
                return Ok(new { success = true, data = tutors });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }

        /// <summary>
        /// Get nearby tutors based on location
        /// </summary>
        /// <param name="city">City name</param>
        /// <param name="district">District name (optional)</param>
        /// <param name="page">Page number</param>
        /// <param name="pageSize">Page size</param>
        /// <returns>List of nearby tutors</returns>
        [HttpGet("nearby")]
        [AllowAnonymous]
        public async Task<IActionResult> GetNearbyTutors(
            [FromQuery] string city,
            [FromQuery] string? district = null,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 20)
        {
            try
            {
                var (tutors, totalCount) = await _tutorService.GetNearbyTutorsAsync(
                    city, district, page, pageSize);

                var response = new
                {
                    success = true,
                    data = tutors,
                    pagination = new
                    {
                        page,
                        pageSize,
                        totalCount,
                        totalPages = (int)Math.Ceiling((double)totalCount / pageSize)
                    }
                };

                return Ok(response);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }

        ///// <summary>
        ///// Get tutor statistics - Admin or Tutor themselves
        ///// </summary>
        ///// <param name="id">Tutor ID</param>
        ///// <returns>Tutor statistics</returns>
        //[HttpGet("{id}/statistics")]
        //[Authorize]
        //public async Task<IActionResult> GetTutorStatistics(long id)
        //{
        //    try
        //    {
        //        if (!CanAccessTutorData(id))
        //        {
        //            return Forbid("Bạn không có quyền truy cập thông tin này");
        //        }

        //        var stats = await _tutorService.GetTutorStatisticsAsync(id);
        //        return Ok(new { success = true, data = stats });
        //    }
        //    catch (ArgumentException ex)
        //    {
        //        return NotFound(new { success = false, message = ex.Message });
        //    }
        //    catch (Exception ex)
        //    {
        //        return StatusCode(500, new { success = false, message = ex.Message });
        //    }
        //}

        /// <summary>
        /// Get tutors requiring verification - Admin only
        /// </summary>
        /// <param name="page">Page number</param>
        /// <param name="pageSize">Page size</param>
        /// <returns>List of tutors pending verification</returns>
        [HttpGet("pending-verification")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetTutorsPendingVerification(
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 20)
        {
            try
            {
                var (tutors, totalCount) = await _tutorService.GetTutorsPendingVerificationAsync(page, pageSize);

                var response = new
                {
                    success = true,
                    data = tutors,
                    pagination = new
                    {
                        page,
                        pageSize,
                        totalCount,
                        totalPages = (int)Math.Ceiling((double)totalCount / pageSize)
                    }
                };

                return Ok(response);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }

        /// <summary>
        /// Update tutor verification status - Admin only
        /// </summary>
        /// <param name="id">Tutor ID</param>
        /// <param name="statusDto">Status update data</param>
        /// <returns>Update result</returns>
        [HttpPatch("{id}/verification-status")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateVerificationStatus(long id, [FromBody] TutorVerificationStatusDto statusDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new { success = false, message = "Dữ liệu không hợp lệ", errors = ModelState });
            }

            try
            {
                var result = await _tutorService.UpdateVerificationStatusAsync(id, statusDto);
                if (!result)
                {
                    return NotFound(new { success = false, message = "Không tìm thấy gia sư" });
                }

                return Ok(new { success = true, message = "Cập nhật trạng thái xác minh thành công" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }

        /// <summary>
        /// Get tutor reviews and ratings
        /// </summary>
        /// <param name="id">Tutor ID</param>
        /// <param name="page">Page number</param>
        /// <param name="pageSize">Page size</param>
        /// <returns>List of tutor reviews</returns>
        [HttpGet("{id}/reviews")]
        [AllowAnonymous]
        public async Task<IActionResult> GetTutorReviews(
            long id,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10)
        {
            try
            {
                var (reviews, totalCount, averageRating) = await _tutorService.GetTutorReviewsAsync(id, page, pageSize);

                var response = new
                {
                    success = true,
                    data = new
                    {
                        reviews,
                        averageRating,
                        totalReviews = totalCount
                    },
                    pagination = new
                    {
                        page,
                        pageSize,
                        totalCount,
                        totalPages = (int)Math.Ceiling((double)totalCount / pageSize)
                    }
                };

                return Ok(response);
            }
            catch (ArgumentException ex)
            {
                return NotFound(new { success = false, message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }

        /// <summary>
        /// Get recommended tutors based on student preferences
        /// </summary>
        /// <param name="studentId">Student ID</param>
        /// <param name="limit">Number of recommendations (default: 10)</param>
        /// <returns>List of recommended tutors</returns>
        [HttpGet("recommendations/{studentId}")]
        [Authorize]
        public async Task<IActionResult> GetRecommendedTutors(long studentId, [FromQuery] int limit = 10)
        {
            try
            {
                var currentUserId = GetCurrentUserId();
                var currentUserRole = User.FindFirst(ClaimTypes.Role)?.Value;

                // Check permissions
                if (currentUserRole != "Admin" && currentUserId != studentId)
                {
                    return Forbid("Bạn chỉ có thể xem gợi ý cho chính mình");
                }

                var recommendations = await _tutorService.GetRecommendedTutorsAsync(studentId, limit);
                return Ok(new { success = true, data = recommendations });
            }
            catch (ArgumentException ex)
            {
                return NotFound(new { success = false, message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }

        /// <summary>
        /// Get all tutors - Admin only
        /// </summary>
        /// <param name="page">Page number</param>
        /// <param name="pageSize">Page size</param>
        /// <param name="status">Filter by verification status (optional)</param>
        /// <returns>List of all tutors</returns>
        [HttpGet("all")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetAllTutors(
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 20,
            [FromQuery] VerificationStatus? status = null)
        {
            try
            {
                var (tutors, totalCount) = await _tutorService.GetAllTutorsAsync(page, pageSize, status);

                var response = new
                {
                    success = true,
                    data = tutors,
                    filters = new { status },
                    pagination = new
                    {
                        page,
                        pageSize,
                        totalCount,
                        totalPages = (int)Math.Ceiling((double)totalCount / pageSize)
                    }
                };

                return Ok(response);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }

        /// <summary>
        /// Upload tutor profile image
        /// </summary>
        /// <param name="id">Tutor ID</param>
        /// <param name="uploadDto">Image upload data</param>
        /// <returns>Updated profile with new image URL</returns>
        [HttpPost("{id}/upload-image")]
        [Authorize]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> UploadProfileImage(long id, [FromForm] TutorImageUploadDto uploadDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new { success = false, message = "Dữ liệu không hợp lệ", errors = ModelState });
            }

            try
            {
                if (!CanModifyTutorData(id))
                {
                    return Forbid("Bạn chỉ có thể cập nhật ảnh của chính mình");
                }

                // Validate image file
                if (uploadDto.ImageFile == null || uploadDto.ImageFile.Length == 0)
                {
                    return BadRequest(new { success = false, message = "Không có file ảnh được tải lên" });
                }

                var allowedTypes = new[] { "image/jpeg", "image/jpg", "image/png" };
                if (!allowedTypes.Contains(uploadDto.ImageFile.ContentType.ToLower()))
                {
                    return BadRequest(new { success = false, message = "Chỉ chấp nhận file JPEG và PNG" });
                }

                if (uploadDto.ImageFile.Length > 5 * 1024 * 1024) // 5MB
                {
                    return BadRequest(new { success = false, message = "Kích thước file không được vượt quá 5MB" });
                }

                var imageUrl = await _tutorService.UploadProfileImageAsync(id, uploadDto.ImageFile);
                return Ok(new { success = true, data = new { imageUrl }, message = "Tải ảnh lên thành công" });
            }
            catch (ArgumentException ex)
            {
                return NotFound(new { success = false, message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }

        #region Private Methods

        private long GetCurrentUserId()
        {
            var userIdClaim = User.FindFirst("UserId")?.Value;
            if (string.IsNullOrEmpty(userIdClaim) || !long.TryParse(userIdClaim, out long userId))
            {
                throw new UnauthorizedAccessException("Không thể xác định người dùng");
            }
            return userId;
        }

        private bool CanAccessTutorData(long tutorId)
        {
            var currentUserRole = User.FindFirst(ClaimTypes.Role)?.Value;
            var currentUserId = GetCurrentUserId();

            // Admin can access any tutor data
            if (currentUserRole == "Admin") return true;

            // Students can view public tutor data (handled at service level)
            if (currentUserRole == "Student") return true;

            // Tutors can access their own data
            if (currentUserRole == "Tutor")
            {
                // You would need to implement a method to check if currentUserId belongs to tutorId
                // For now, allowing access - implement proper check later
                return true;
            }

            return false;
        }

        private bool CanModifyTutorData(long tutorId)
        {
            var currentUserRole = User.FindFirst(ClaimTypes.Role)?.Value;
            var currentUserId = GetCurrentUserId();

            // Admin can modify any tutor data
            if (currentUserRole == "Admin") return true;

            // Tutors can only modify their own data
            if (currentUserRole == "Tutor")
            {
                // You would need to implement a method to check if currentUserId belongs to tutorId
                // For now, allowing access - implement proper check later
                return true;
            }

            return false;
        }

        #endregion
    }
}