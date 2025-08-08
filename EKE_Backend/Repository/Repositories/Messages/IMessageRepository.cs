using Repository.Entities;
using Repository.Repositories.BaseRepository;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Repository.Repositories.Messages;
namespace Repository.Repositories.Messages
{
    public interface IMessageRepository : IBaseRepository<Message>
    {
        Task<(IEnumerable<Message> Messages, int TotalCount)> GetPagedMessagesAsync(long conversationId, int page, int pageSize);
        Task<Message?> GetMessageWithSenderAsync(long messageId);
        Task<int> GetUnreadCountForUserAsync(long conversationId, long userId);
        Task<int> GetTotalUnreadCountForUserAsync(long userId);
        Task<Dictionary<long, int>> GetUnreadCountPerConversationAsync(long userId);
        Task MarkMessagesAsReadAsync(long conversationId, long userId, long? lastMessageId = null);
        Task<Message> CreateAsync(Message message);
        Task<bool> DeleteAsync(long messageId);
        Task<(IEnumerable<Message> Messages, int TotalCount)> SearchMessagesAsync(long conversationId, string query, int page, int pageSize);
        Task<bool> IsUserInConversationAsync(long conversationId, long userId);
        Task<Message?> GetLastMessageByConversationIdAsync(long conversationId);
    }
}
