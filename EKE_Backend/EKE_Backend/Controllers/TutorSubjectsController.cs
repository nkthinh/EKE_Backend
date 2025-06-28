using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Service.DTO.Request;
using Service.Services.TutorSubjects;
using System.Security.Claims;

namespace EKE_Backend.Controllers
{
    [ApiController]
    [Route("api/tutors/{tutorId}/subjects")]
    [Authorize]
    public class TutorSubjectsController : ControllerBase
    {
        private readonly ITutorSubjectService _tutorSubjectService;

        public TutorSubjectsController(ITutorSubjectService tutorSubjectService)
        {
            _tutorSubjectService = tutorSubjectService;
        }

        // GET: api/tutors/{tutorId}/subjects
        [HttpGet]
        public async Task<IActionResult> GetTutorSubjects(long tutorId)
        {
            try
            {
                // Check permission: Admin can view any, Tutor can view own, Student can view any
                if (!CanAccessTutorData(tutorId))
                {
                    return Forbid("Bạn không có quyền truy cập dữ liệu này");
                }

                var tutorSubjects = await _tutorSubjectService.GetTutorSubjectsAsync(tutorId);
                return Ok(new { success = true, data = tutorSubjects });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }

        // POST: api/tutors/{tutorId}/subjects
        [HttpPost]
        public async Task<IActionResult> AddTutorSubject(long tutorId, [FromBody] TutorSubjectCreateDto tutorSubjectDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new { success = false, message = "Dữ liệu không hợp lệ", errors = ModelState });
            }

            try
            {
                // Check permission: Only Admin or the tutor themselves can add subjects
                if (!CanModifyTutorData(tutorId))
                {
                    return Forbid("Bạn chỉ có thể quản lý môn học của chính mình");
                }

                var tutorSubject = await _tutorSubjectService.AddTutorSubjectAsync(tutorId, tutorSubjectDto);
                return Ok(new { success = true, data = tutorSubject, message = "Thêm môn học thành công" });
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

        // PUT: api/tutors/{tutorId}/subjects/{subjectId}
        [HttpPut("{subjectId}")]
        public async Task<IActionResult> UpdateTutorSubject(long tutorId, long subjectId, [FromBody] TutorSubjectUpdateDto tutorSubjectDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new { success = false, message = "Dữ liệu không hợp lệ", errors = ModelState });
            }

            try
            {
                if (!CanModifyTutorData(tutorId))
                {
                    return Forbid("Bạn chỉ có thể quản lý môn học của chính mình");
                }

                var updatedTutorSubject = await _tutorSubjectService.UpdateTutorSubjectAsync(tutorId, subjectId, tutorSubjectDto);
                return Ok(new { success = true, data = updatedTutorSubject, message = "Cập nhật môn học thành công" });
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

        // DELETE: api/tutors/{tutorId}/subjects/{subjectId}
        [HttpDelete("{subjectId}")]
        public async Task<IActionResult> RemoveTutorSubject(long tutorId, long subjectId)
        {
            try
            {
                if (!CanModifyTutorData(tutorId))
                {
                    return Forbid("Bạn chỉ có thể quản lý môn học của chính mình");
                }

                var removed = await _tutorSubjectService.RemoveTutorSubjectAsync(tutorId, subjectId);
                if (!removed)
                {
                    return NotFound(new { success = false, message = "Không tìm thấy môn học" });
                }

                return Ok(new { success = true, message = "Xóa môn học thành công" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }

        private bool CanAccessTutorData(long tutorId)
        {
            var currentUserRole = User.FindFirst(ClaimTypes.Role)?.Value;
            var currentUserId = long.Parse(User.FindFirst("UserId")?.Value ?? "0");

            // Admin can access any tutor data
            if (currentUserRole == "Admin") return true;

            // Students can view tutor subjects (for matching)
            if (currentUserRole == "Student") return true;

            // Tutors can only access their own data
            if (currentUserRole == "Tutor")
            {
                // Here you would need to check if currentUserId maps to tutorId
                // This requires a method to get tutor by userId
                return true; // For now, allow - implement proper check later
            }

            return false;
        }

        private bool CanModifyTutorData(long tutorId)
        {
            var currentUserRole = User.FindFirst(ClaimTypes.Role)?.Value;
            var currentUserId = long.Parse(User.FindFirst("UserId")?.Value ?? "0");

            // Admin can modify any tutor data
            if (currentUserRole == "Admin") return true;

            // Tutors can only modify their own data
            if (currentUserRole == "Tutor")
            {
                // Here you would need to check if currentUserId maps to tutorId
                return true; // For now, allow - implement proper check later
            }

            return false;
        }
    }
}