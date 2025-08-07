using Microsoft.AspNetCore.Mvc;
using Service.Services.Conversations;
using System;
using System.Threading.Tasks;
using Service.DTO.Response;

namespace EKE_Backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ConversationsController : ControllerBase
    {
        private readonly IConversationService _conversationService;

        public ConversationsController(IConversationService conversationService)
        {
            _conversationService = conversationService;
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
                var conversation = await _conversationService.GetOrCreateConversationAsync(matchId);
                return Ok(new { success = true, data = conversation });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }
    }
}
