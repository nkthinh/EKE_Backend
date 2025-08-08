
using Microsoft.AspNetCore.Mvc;
using Service.DTO.Response;
using Service.Services.Conversations;
  // Đảm bảo thêm namespace cho IMatchService
using Service.Services.Matches;
using System;
using System.Threading.Tasks;

namespace EKE_Backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ConversationsController : ControllerBase
    {
        private readonly IConversationService _conversationService;
        private readonly IMatchService _matchService;  // Thêm IMatchService để kiểm tra ghép cặp

        public ConversationsController(IConversationService conversationService, IMatchService matchService)
        {
            _conversationService = conversationService;
            _matchService = matchService;
        }

        /// <summary>
        /// Get all conversations for the current user
        /// </summary>
        [HttpGet("user/{userId}")]
        public async Task<IActionResult> GetUserConversations(long userId)
        {
            try
            {
                var conversations = await _conversationService.GetUserConversationsAsync(userId);
                return Ok(new { success = true, data = conversations });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }

        /// <summary>
        /// Get conversation by match ID (create if it doesn't exist)
        /// </summary>
        [HttpGet("match/{matchId}")]
        public async Task<IActionResult> GetOrCreateConversation(long matchId)
        {
            try
            {
                // Lấy thông tin match từ matchId
                var match = await _matchService.GetByIdAsync(matchId);

                // Kiểm tra xem học sinh và gia sư đã được ghép cặp chưa
                var matchExists = await _matchService.CheckActiveMatch(match.StudentId, match.TutorId);
                if (!matchExists)
                {
                    return BadRequest(new { success = false, message = "Học sinh và gia sư chưa được ghép cặp." });
                }

                // Nếu đã ghép cặp, tạo hoặc lấy cuộc trò chuyện mới
                var conversation = await _conversationService.GetOrCreateConversationAsync(matchId);

                return Ok(new { success = true, data = conversation });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }
        /// <summary>
        /// Get conversation details by conversation ID
        /// </summary>
        [HttpGet("{conversationId}")]
        public async Task<IActionResult> GetConversationById(long conversationId)
        {
            try
            {
                var conversation = await _conversationService.GetConversationByIdAsync(conversationId);

                if (conversation == null)
                {
                    return NotFound(new { success = false, message = "Cuộc trò chuyện không tồn tại." });
                }

                return Ok(new { success = true, data = conversation });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }
    }
}
