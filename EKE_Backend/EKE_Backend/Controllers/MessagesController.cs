using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Repository.Repositories.Conversations;
using Repository.Repositories.Students;
using Repository.Repositories.Tutors;
using Repository.Repositories.Users;
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
        private readonly IUserRepository _userRepository;
        private readonly IHubContext<ChatHub> _hubContext;
        private readonly IStudentRepository _studentRepository;
        private readonly ITutorRepository _tutorRepository;
        private readonly IConversationRepository _conversationRepository;
        private readonly ILogger<MessagesController> _logger;
        public MessagesController(IMessageService messageService, IHubContext<ChatHub> hubContext,
            IStudentRepository studentRepository, ITutorRepository tutorRepository, IConversationRepository conversationRepository,
             ILogger<MessagesController> logger, IUserRepository userRepository)
        {
            _messageService = messageService;
            _hubContext = hubContext;
            _studentRepository = studentRepository;
            _tutorRepository = tutorRepository;
            _conversationRepository = conversationRepository;
            _logger = logger;
            _userRepository = userRepository;
        }

        [HttpPost]
        public async Task<IActionResult> SendMessage([FromBody] MessageCreateDto messageDto)
        {
            try
            {
                var currentUserId = await GetCurrentUserId();  // Lấy UserId của người gửi
                var userRole = await GetCurrentUserRole();      // Lấy Role của người gửi (Student hoặc Tutor)

                // Kiểm tra nếu conversationId hợp lệ
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

                // Kiểm tra nếu User là Student hay Tutor và gán SenderId đúng
                if (userRole == 1) // Student
                {
                    var studentId = await _studentRepository.GetStudentIdByUserIdAsync(currentUserId);
                    if (studentId == null)
                    {
                        throw new UnauthorizedAccessException("Người dùng không phải học sinh");
                    }
                    messageDto.SenderId = currentUserId;   // Gán SenderId là StudentId
                }
                else if (userRole == 2) // Tutor
                {
                    var tutor = await _tutorRepository.GetTutorByUserIdAsync(currentUserId);
                    if (tutor == null)
                    {
                        throw new UnauthorizedAccessException("Người dùng không phải gia sư");
                    }
                    messageDto.SenderId = currentUserId;  // Gán SenderId là TutorId
                }
                else
                {
                    return Unauthorized("Không xác định được vai trò người dùng");
                }

                // Gửi tin nhắn
                var message = await _messageService.SendMessageAsync(messageDto);

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
        [HttpGet("conversation/{conversationId}/messages")]
        public async Task<IActionResult> GetConversationMessages(long conversationId, [FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            try
            {
                // Lấy userId từ token hiện tại
                var currentUserId = await GetCurrentUserId();

                // Kiểm tra nếu người dùng có quyền truy cập cuộc trò chuyện
                var userRole = await GetCurrentUserRole();
                if (userRole != 1 && userRole != 2) // Kiểm tra nếu người dùng là Student hoặc Tutor
                {
                    return Unauthorized("Không xác định được vai trò người dùng");
                }

                // Lấy các tin nhắn trong cuộc trò chuyện
                var (messages, totalCount) = await _messageService.GetConversationMessagesAsync(conversationId, page, pageSize);

                // Kiểm tra nếu cuộc trò chuyện tồn tại
                var conversation = await _conversationRepository.GetByIdAsync(conversationId);
                if (conversation == null)
                {
                    return BadRequest(new { success = false, message = "Cuộc trò chuyện không tồn tại" });
                }

                // Trả về các tin nhắn trong cuộc trò chuyện
                return Ok(new { success = true, messages, totalCount });
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

            // Trả về UserId trực tiếp, sẽ sử dụng trong quá trình xác định StudentId hoặc TutorId
            return userId;
        }
        private async Task<int> GetCurrentUserRole()
        {
            var userIdClaim = User.FindFirst("UserId")?.Value; // Lấy UserId từ claim

            if (string.IsNullOrEmpty(userIdClaim) || !long.TryParse(userIdClaim, out long userId))
            {
                throw new UnauthorizedAccessException("Không thể xác định người dùng");
            }

            // Fetch user from the database using the UserId
            var user = await _userRepository.GetByIdAsync(userId);

            if (user == null)
            {
                throw new UnauthorizedAccessException("Không tìm thấy người dùng");
            }

            // Return the Role as an integer (1 for student, 2 for tutor)
            return user.Role.HasValue ? (int)user.Role.Value : 0;  // Default to 0 if role is null
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
