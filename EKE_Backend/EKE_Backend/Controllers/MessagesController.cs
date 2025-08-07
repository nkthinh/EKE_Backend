using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
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

        public MessagesController(IMessageService messageService, IHubContext<ChatHub> hubContext)
        {
            _messageService = messageService;
            _hubContext = hubContext;
        }

        [HttpPost]
        public async Task<IActionResult> SendMessage([FromBody] MessageCreateDto messageDto)
        {
            try
            {
                var currentUserId = GetCurrentUserId();

                var conversation = await _messageService.GetOrCreateConversationAsync(messageDto.ConversationId);

                if (!await _messageService.IsUserInConversationAsync(conversation.Id, currentUserId))
                {
                    return Forbid("Bạn không có quyền gửi tin nhắn trong cuộc trò chuyện này");
                }

                messageDto.SenderId = currentUserId;
                var message = await _messageService.SendMessageAsync(messageDto);

                await _hubContext.Clients.Group($"Conversation_{conversation.Id}")
                    .SendAsync("ReceiveMessage", message);

                return Ok(new { success = true, data = message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }

        private long GetCurrentUserId()
        {
            var userIdClaim = User.FindFirst("UserId")?.Value;
            if (string.IsNullOrEmpty(userIdClaim) || !long.TryParse(userIdClaim, out long userId))
            {
                throw new UnauthorizedAccessException("Không thể xác định người dùng");
            }
            return userId;
        }
    }
}
