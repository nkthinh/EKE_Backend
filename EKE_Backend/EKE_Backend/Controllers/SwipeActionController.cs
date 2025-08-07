using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Service.DTO.Request;
using Service.DTO.Response;
using Service.Services.SwipeActions;

namespace EKE_Backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SwipeActionController : ControllerBase
    {
        private readonly ISwipeActionService _swipeActionService;

        public SwipeActionController(ISwipeActionService swipeActionService)
        {
            _swipeActionService = swipeActionService;
        }
        // API cho phép gia sư lấy danh sách học viên đã "like" mình
        [HttpGet("liked-students/{tutorId}")]
        public async Task<ActionResult<IEnumerable<StudentResponseDto>>> GetLikedStudentsByTutor(long tutorId)
        {
            try
            {
                var likedStudents = await _swipeActionService.GetLikedStudentsByTutorAsync(tutorId);
                return Ok(new { success = true, data = likedStudents });
            }
            catch (Exception ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
        }
        [HttpPost("swipe")]
        public async Task<ActionResult<SwipeActionResponseDto>> Swipe([FromBody] SwipeActionRequestDto requestDto)
        {
            try
            {
                var studentId = GetCurrentUserId();  // Lấy studentId thay vì userId
                var response = await _swipeActionService.Swipe(studentId, requestDto.TutorId, requestDto.Action);
                return Ok(response);
            }
            catch (Exception ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
        }
        [HttpPost("accept-match")]
        public async Task<ActionResult<SwipeActionResponseDto>> AcceptMatch([FromBody] AcceptMatchRequestDto requestDto)
        {
            try
            {
                var response = await _swipeActionService.AcceptMatch(requestDto.TutorId, requestDto.StudentId);
                return Ok(response);
            }
            catch (Exception ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
        }

        private long GetCurrentUserId()
        {
            var studentIdClaim = User.FindFirst("StudentId")?.Value;
            if (string.IsNullOrEmpty(studentIdClaim) || !long.TryParse(studentIdClaim, out long studentId))
            {
                throw new UnauthorizedAccessException("Không thể xác định học viên");
            }
            return studentId;
        }

    }

}
