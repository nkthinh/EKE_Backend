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

        public MessagesController(
            IMessageService messageService,
            IHubContext<ChatHub> hubContext)
        {
            _messageService = messageService;
            _hubContext = hubContext;
        }

        /// <summary>
        /// Get all messages in a conversation between student and tutor
        /// </summary>
        /// <param name="conversationId">Conversation ID</param>
        /// <param name="page">Page number (default: 1)</param>
        /// <param name="pageSize">Page size (default: 50)</param>
        /// <returns>List of messages in the conversation</returns>
        [HttpGet("conversation/{conversationId}")]
        public async Task<IActionResult> GetConversationMessages(
            long conversationId,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 50)
        {
            try
            {
                var currentUserId = GetCurrentUserId();

                // Verify user has access to this conversation
                if (!await CanAccessConversation(conversationId, currentUserId))
                {
                    return Forbid("Bạn không có quyền truy cập cuộc trò chuyện này");
                }

                var (messages, totalCount) = await _messageService.GetConversationMessagesAsync(
                    conversationId, page, pageSize);

                // Mark messages as read for current user
                await _messageService.MarkMessagesAsReadAsync(conversationId, currentUserId);

                var response = new
                {
                    success = true,
                    data = messages,
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
        /// Get conversation by match ID
        /// </summary>
        /// <param name="matchId">Match ID</param>
        /// <returns>Conversation details</returns>
        [HttpGet("conversation/by-match/{matchId}")]
        public async Task<IActionResult> GetConversationByMatch(long matchId)
        {
            try
            {
                var currentUserId = GetCurrentUserId();

                // Verify user is part of this match
                if (!await CanAccessMatch(matchId, currentUserId))
                {
                    return Forbid("Bạn không có quyền truy cập cuộc trò chuyện này");
                }

                var conversation = await _messageService.GetOrCreateConversationAsync(matchId);
                return Ok(new { success = true, data = conversation });
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
        /// Send a new message
        /// </summary>
        /// <param name="messageDto">Message data</param>
        /// <returns>Created message</returns>
        [HttpPost]
        public async Task<IActionResult> SendMessage([FromBody] MessageCreateDto messageDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new { success = false, message = "Dữ liệu không hợp lệ", errors = ModelState });
            }

            try
            {
                var currentUserId = GetCurrentUserId();

                // Verify user has access to this conversation
                if (!await CanAccessConversation(messageDto.ConversationId, currentUserId))
                {
                    return Forbid("Bạn không có quyền gửi tin nhắn trong cuộc trò chuyện này");
                }

                messageDto.SenderId = currentUserId;
                var message = await _messageService.SendMessageAsync(messageDto);

                // Send real-time notification via SignalR
                await NotifyNewMessage(message);

                return Ok(new { success = true, data = message, message = "Tin nhắn đã được gửi" });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }

        /// <summary>
        /// Send a message with file attachment
        /// </summary>
        /// <param name="messageDto">Message data with file</param>
        /// <returns>Created message</returns>
        [HttpPost("with-file")]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> SendMessageWithFile([FromForm] MessageWithFileDto messageDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new { success = false, message = "Dữ liệu không hợp lệ", errors = ModelState });
            }

            try
            {
                var currentUserId = GetCurrentUserId();

                // Verify user has access to this conversation
                if (!await CanAccessConversation(messageDto.ConversationId, currentUserId))
                {
                    return Forbid("Bạn không có quyền gửi tin nhắn trong cuộc trò chuyện này");
                }

                // Validate file
                if (messageDto.File != null)
                {
                    var allowedTypes = new[] { "image/jpeg", "image/jpg", "image/png", "application/pdf", "text/plain" };
                    if (!allowedTypes.Contains(messageDto.File.ContentType.ToLower()))
                    {
                        return BadRequest(new { success = false, message = "Loại file không được hỗ trợ" });
                    }

                    if (messageDto.File.Length > 10 * 1024 * 1024) // 10MB
                    {
                        return BadRequest(new { success = false, message = "File không được vượt quá 10MB" });
                    }
                }

                messageDto.SenderId = currentUserId;
                var message = await _messageService.SendMessageWithFileAsync(messageDto);

                // Send real-time notification via SignalR
                await NotifyNewMessage(message);

                return Ok(new { success = true, data = message, message = "Tin nhắn đã được gửi" });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }

        /// <summary>
        /// Mark messages as read
        /// </summary>
        /// <param name="conversationId">Conversation ID</param>
        /// <returns>Success response</returns>
        [HttpPut("conversation/{conversationId}/read")]
        public async Task<IActionResult> MarkMessagesAsRead(long conversationId)
        {
            try
            {
                var currentUserId = GetCurrentUserId();

                if (!await CanAccessConversation(conversationId, currentUserId))
                {
                    return Forbid("Bạn không có quyền truy cập cuộc trò chuyện này");
                }

                await _messageService.MarkMessagesAsReadAsync(conversationId, currentUserId);
                return Ok(new { success = true, message = "Đã đánh dấu tin nhắn đã đọc" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }

        /// <summary>
        /// Get unread message count for current user
        /// </summary>
        /// <returns>Unread message count</returns>
        [HttpGet("unread-count")]
        public async Task<IActionResult> GetUnreadMessageCount()
        {
            try
            {
                var currentUserId = GetCurrentUserId();
                var count = await _messageService.GetUnreadMessageCountAsync(currentUserId);

                return Ok(new { success = true, data = new { unreadCount = count } });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }

        /// <summary>
        /// Get all conversations for current user
        /// </summary>
        /// <returns>List of conversations</returns>
        [HttpGet("conversations")]
        public async Task<IActionResult> GetUserConversations()
        {
            try
            {
                var currentUserId = GetCurrentUserId();
                var conversations = await _messageService.GetUserConversationsAsync(currentUserId);

                return Ok(new { success = true, data = conversations });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }

        /// <summary>
        /// Delete a message (soft delete)
        /// </summary>
        /// <param name="messageId">Message ID</param>
        /// <returns>Success response</returns>
        [HttpDelete("{messageId}")]
        public async Task<IActionResult> DeleteMessage(long messageId)
        {
            try
            {
                var currentUserId = GetCurrentUserId();
                var deleted = await _messageService.DeleteMessageAsync(messageId, currentUserId);

                if (!deleted)
                {
                    return NotFound(new { success = false, message = "Không tìm thấy tin nhắn hoặc bạn không có quyền xóa" });
                }

                return Ok(new { success = true, message = "Tin nhắn đã được xóa" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }

        /// <summary>
        /// Search messages in a conversation
        /// </summary>
        /// <param name="conversationId">Conversation ID</param>
        /// <param name="query">Search query</param>
        /// <param name="page">Page number</param>
        /// <param name="pageSize">Page size</param>
        /// <returns>Search results</returns>
        [HttpGet("conversation/{conversationId}/search")]
        public async Task<IActionResult> SearchMessages(
            long conversationId,
            [FromQuery] string query,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 20)
        {
            try
            {
                var currentUserId = GetCurrentUserId();

                if (!await CanAccessConversation(conversationId, currentUserId))
                {
                    return Forbid("Bạn không có quyền truy cập cuộc trò chuyện này");
                }

                if (string.IsNullOrWhiteSpace(query))
                {
                    return BadRequest(new { success = false, message = "Từ khóa tìm kiếm không được để trống" });
                }

                var (messages, totalCount) = await _messageService.SearchMessagesAsync(
                    conversationId, query, page, pageSize);

                var response = new
                {
                    success = true,
                    data = messages,
                    query,
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
        /// Get message statistics for admin
        /// </summary>
        /// <returns>Message statistics</returns>
        [HttpGet("statistics")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetMessageStatistics()
        {
            try
            {
                var stats = await _messageService.GetMessageStatisticsAsync();
                return Ok(new { success = true, data = stats });
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

        private async Task<bool> CanAccessConversation(long conversationId, long userId)
        {
            var currentUserRole = User.FindFirst(ClaimTypes.Role)?.Value;

            // Admin can access any conversation
            if (currentUserRole == "Admin") return true;

            // Check if user is part of this conversation
            return await _messageService.IsUserInConversationAsync(conversationId, userId);
        }

        private async Task<bool> CanAccessMatch(long matchId, long userId)
        {
            var currentUserRole = User.FindFirst(ClaimTypes.Role)?.Value;

            // Admin can access any match
            if (currentUserRole == "Admin") return true;

            // Check if user is part of this match
            return await _messageService.IsUserInMatchAsync(matchId, userId);
        }

        private async Task NotifyNewMessage(MessageResponseDto message)
        {
            try
            {
                // Get the recipient ID from the conversation
                var recipientId = await _messageService.GetRecipientIdAsync(message.ConversationId, message.SenderId);

                if (recipientId.HasValue)
                {
                    // Send real-time notification to recipient
                    await _hubContext.Clients.User(recipientId.Value.ToString())
                        .SendAsync("ReceiveMessage", message);

                    // Also send to conversation-specific group
                    await _hubContext.Clients.Group($"Conversation_{message.ConversationId}")
                        .SendAsync("ReceiveMessage", message);
                }
            }
            catch (Exception ex)
            {
                // Log error but don't fail the API call
                // _logger.LogError(ex, "Error sending real-time notification for message {MessageId}", message.Id);
            }
        }

        #endregion
    }

    // SignalR Hub for real-time messaging
    public class ChatHub : Hub
    {
        public async Task JoinConversationGroup(string conversationId)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, $"Conversation_{conversationId}");
        }

        public async Task LeaveConversationGroup(string conversationId)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, $"Conversation_{conversationId}");
        }

        public override async Task OnConnectedAsync()
        {
            var userId = Context.UserIdentifier;
            if (!string.IsNullOrEmpty(userId))
            {
                await Groups.AddToGroupAsync(Context.ConnectionId, $"User_{userId}");
            }
            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            var userId = Context.UserIdentifier;
            if (!string.IsNullOrEmpty(userId))
            {
                await Groups.RemoveFromGroupAsync(Context.ConnectionId, $"User_{userId}");
            }
            await base.OnDisconnectedAsync(exception);
        }
    }
}