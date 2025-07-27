using Service.DTO.Request;
using Service.DTO.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.Services.Chat
{
    public interface IChatService
    {
        // Conversations
        Task<IEnumerable<ConversationResponseDto>> GetUserConversationsAsync(long userId);
        Task<ConversationResponseDto?> GetConversationByIdAsync(long conversationId, long userId);
        Task<ConversationResponseDto?> GetConversationByMatchIdAsync(long matchId, long userId);

        // Messages
        Task<ChatHistoryDto> GetChatHistoryAsync(long conversationId, long userId, int page = 1, int pageSize = 50);
        Task<MessageResponseDto> SendMessageAsync(long userId, SendMessageDto messageDto);
        Task<bool> MarkMessagesAsReadAsync(long userId, MarkAsReadDto markAsReadDto);

        // Statistics
        Task<int> GetUnreadMessagesCountAsync(long userId);
        Task<Dictionary<long, int>> GetUnreadCountPerConversationAsync(long userId);

        // Real-time features
        Task<bool> SetUserOnlineStatusAsync(long userId, bool isOnline);
        Task<DateTime?> GetLastSeenAsync(long userId);
    }
}
