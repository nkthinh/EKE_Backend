using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Repository.Repositories.Conversations;
using Repository.Repositories.Students;
using Repository.Repositories.Tutors;
using Service.DTO.Request;
using Service.DTO.Response;
using Service.Services.Messages;
using System.Security.Claims;

namespace EKE_Backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class MessagesController : ControllerBase
    {
        private readonly IMessageService _messageService;
        private readonly IHubContext<ChatHub> _hubContext;
        private readonly IStudentRepository _studentRepository;
        private readonly ITutorRepository _tutorRepository;
        private readonly IConversationRepository _conversationRepository;

        public MessagesController(IMessageService messageService, IHubContext<ChatHub> hubContext,
            IStudentRepository studentRepository, ITutorRepository tutorRepository, IConversationRepository conversationRepository)
        {
            _messageService = messageService;
            _hubContext = hubContext;
            _studentRepository = studentRepository;
            _tutorRepository = tutorRepository;
            _conversationRepository = conversationRepository;
        }
        [HttpPost]
        public async Task<IActionResult> SendMessage([FromBody] MessageCreateDto messageDto)
        {
            try
            {
                var currentUserId = await GetCurrentUserId();  // Lấy UserId của người gửi

                // Kiểm tra nếu conversationId hợp lệ, không cần kiểm tra lại matchId nữa
                var conversation = await _conversationRepository.GetByIdAsync(messageDto.ConversationId);
                if (conversation == null)
                {
                    return BadRequest(new { success = false, message = "Cuộc trò chuyện không tồn tại." });
                }

                // Kiểm tra nếu người dùng có quyền gửi tin nhắn
                if (!await _messageService.IsUserInConversationAsync(conversation.Id, currentUserId))
                {
                    return Forbid("Bạn không có quyền gửi tin nhắn trong cuộc trò chuyện này");
                }

                messageDto.SenderId = currentUserId;  // Đặt SenderId cho tin nhắn
                var message = await _messageService.SendMessageAsync(messageDto);  // Gửi tin nhắn

                // Gửi tin nhắn tới các client đã đăng ký nhóm này
                await _hubContext.Clients.Group($"Conversation_{conversation.Id}")
                    .SendAsync("ReceiveMessage", message);

                return Ok(new { success = true, data = message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }


        private async Task<long> GetCurrentUserId()
        {
            var userIdClaim = User.FindFirst("UserId")?.Value;
            var userRoleClaim = User.FindFirst("Role")?.Value; // Lấy role từ token (Student hoặc Tutor)

            if (string.IsNullOrEmpty(userIdClaim) || !long.TryParse(userIdClaim, out long userId))
            {
                throw new UnauthorizedAccessException("Không thể xác định người dùng");
            }

            if (userRoleClaim == "Student")
            {
                return await GetStudentIdByUserId(userId); // Trả về StudentId từ bảng Student
            }
            else if (userRoleClaim == "Tutor")
            {
                return await GetTutorIdByUserId(userId); // Trả về TutorId từ bảng Tutor
            }

            throw new UnauthorizedAccessException("Không thể xác định vai trò người dùng");
        }

        private async Task<long> GetStudentIdByUserId(long userId)
        {
            var student = await _studentRepository.GetStudentByUserIdAsync(userId); // Truy vấn để lấy StudentId từ bảng Student
            if (student == null)
            {
                throw new UnauthorizedAccessException("Không tìm thấy học sinh");
            }
            return student.Id;
        }

        private async Task<long> GetTutorIdByUserId(long userId)
        {
            var tutor = await _tutorRepository.GetTutorByUserIdAsync(userId); // Truy vấn để lấy TutorId từ bảng Tutor
            if (tutor == null)
            {
                throw new UnauthorizedAccessException("Không tìm thấy gia sư");
            }
            return tutor.Id;
        }


    }
}
