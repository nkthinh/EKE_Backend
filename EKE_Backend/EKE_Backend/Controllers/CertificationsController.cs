using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Service.DTO.Request;
using Service.Services.Certifications;
using System.Security.Claims;

namespace EKE_Backend.Controllers
{
    [ApiController]
    [Route("api/tutors/{tutorId}/certifications")]
    [Authorize]
    public class CertificationsController : ControllerBase
    {
        private readonly ICertificationService _certificationService;

        public CertificationsController(ICertificationService certificationService)
        {
            _certificationService = certificationService;
        }

        // GET: api/tutors/{tutorId}/certifications
        [HttpGet]
        public async Task<IActionResult> GetTutorCertifications(long tutorId)
        {
            try
            {
                if (!CanAccessTutorData(tutorId))
                {
                    return Forbid("Bạn không có quyền truy cập dữ liệu này");
                }

                var certifications = await _certificationService.GetCertificationsByTutorIdAsync(tutorId);
                return Ok(new { success = true, data = certifications });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }

        // POST: api/tutors/{tutorId}/certifications
        [HttpPost]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> AddCertification(long tutorId, [FromForm] CertificationCreateDto certificationDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new { success = false, message = "Dữ liệu không hợp lệ", errors = ModelState });
            }

            try
            {
                if (!CanModifyTutorData(tutorId))
                {
                    return Forbid("Bạn chỉ có thể quản lý chứng chỉ của chính mình");
                }

                var certification = await _certificationService.CreateCertificationAsync(tutorId, certificationDto);
                return Ok(new { success = true, data = certification, message = "Thêm chứng chỉ thành công" });
            }
            catch (ArgumentException ex)
            {
                return NotFound(new { success = false, message = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(new { success = false, message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }

        // PUT: api/tutors/{tutorId}/certifications/{certificationId}
        [HttpPut("{certificationId}")]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> UpdateCertification(long tutorId, long certificationId, [FromForm] CertificationUpdateDto certificationDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new { success = false, message = "Dữ liệu không hợp lệ", errors = ModelState });
            }

            try
            {
                if (!CanModifyTutorData(tutorId))
                {
                    return Forbid("Bạn chỉ có thể quản lý chứng chỉ của chính mình");
                }

                var updatedCertification = await _certificationService.UpdateCertificationAsync(certificationId, certificationDto);
                return Ok(new { success = true, data = updatedCertification, message = "Cập nhật chứng chỉ thành công" });
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

        // DELETE: api/tutors/{tutorId}/certifications/{certificationId}
        [HttpDelete("{certificationId}")]
        public async Task<IActionResult> DeleteCertification(long tutorId, long certificationId)
        {
            try
            {
                if (!CanModifyTutorData(tutorId))
                {
                    return Forbid("Bạn chỉ có thể quản lý chứng chỉ của chính mình");
                }

                var deleted = await _certificationService.DeleteCertificationAsync(certificationId);
                if (!deleted)
                {
                    return NotFound(new { success = false, message = "Không tìm thấy chứng chỉ" });
                }

                return Ok(new { success = true, message = "Xóa chứng chỉ thành công" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }

        // POST: api/tutors/{tutorId}/certifications/{certificationId}/verify - Admin only
        [HttpPost("{certificationId}/verify")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> VerifyCertification(long tutorId, long certificationId)
        {
            try
            {
                var verified = await _certificationService.VerifyCertificationAsync(certificationId);
                if (!verified)
                {
                    return NotFound(new { success = false, message = "Không tìm thấy chứng chỉ" });
                }

                return Ok(new { success = true, message = "Xác minh chứng chỉ thành công" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }

        private bool CanAccessTutorData(long tutorId)
        {
            var currentUserRole = User.FindFirst(ClaimTypes.Role)?.Value;
            return currentUserRole == "Admin" || currentUserRole == "Student" || CanModifyTutorData(tutorId);
        }

        private bool CanModifyTutorData(long tutorId)
        {
            var currentUserRole = User.FindFirst(ClaimTypes.Role)?.Value;
            var currentUserId = long.Parse(User.FindFirst("UserId")?.Value ?? "0");

            if (currentUserRole == "Admin") return true;
            if (currentUserRole == "Tutor")
            {
                // Implement proper check to see if currentUserId belongs to tutorId
                return true; // For now, allow - implement proper check later
            }

            return false;
        }
    }
}

