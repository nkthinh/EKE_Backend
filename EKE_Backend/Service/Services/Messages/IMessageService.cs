using Service.DTO.Request;
using Service.DTO.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.Services.Messages
{
    public interface IMessageService
    {
        Task<(IEnumerable<MessageResponseDto> Messages, int TotalCount)> GetConversationMessagesAsync(long conversationId, int page, int pageSize);
        Task<ConversationResponseDto> GetOrCreateConversationAsync(long matchId);
        Task<MessageResponseDto> SendMessageAsync(MessageCreateDto messageDto);
        Task<MessageResponseDto> SendMessageWithFileAsync(MessageWithFileDto messageDto);
        Task MarkMessagesAsReadAsync(long conversationId, long userId);
        Task<int> GetUnreadMessageCountAsync(long userId);
        Task<IEnumerable<ConversationResponseDto>> GetUserConversationsAsync(long userId);
        Task<bool> DeleteMessageAsync(long messageId, long userId);
        Task<(IEnumerable<MessageResponseDto> Messages, int TotalCount)> SearchMessagesAsync(long conversationId, string query, int page, int pageSize);
        Task<MessageStatisticsDto> GetMessageStatisticsAsync();
        Task<bool> IsUserInConversationAsync(long conversationId, long userId);
        Task<bool> IsUserInMatchAsync(long matchId, long userId);
        Task<long?> GetRecipientIdAsync(long conversationId, long senderId);
    }
}
