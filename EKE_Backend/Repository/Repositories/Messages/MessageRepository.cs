using Microsoft.EntityFrameworkCore;
using Repository.Entities;
using Repository.Repositories.BaseRepository;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Repository.Repositories.Messages
{
    public class MessageRepository : BaseRepository<Message>, IMessageRepository
    {
        public MessageRepository(ApplicationDbContext context) : base(context) { }

        public async Task<(IEnumerable<Message> Messages, int TotalCount)> GetPagedMessagesAsync(long conversationId, int page, int pageSize)
        {
            var query = _dbSet
                .Include(m => m.Sender)
                .Where(m => m.ConversationId == conversationId);

            var totalCount = await query.CountAsync();
            var messages = await query
                .OrderByDescending(m => m.CreatedAt)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return (messages.OrderBy(m => m.CreatedAt), totalCount);
        }

        public async Task<Message?> GetMessageWithSenderAsync(long messageId)
        {
            return await _dbSet
                .Include(m => m.Sender)
                .FirstOrDefaultAsync(m => m.Id == messageId);
        }

        public async Task<Message> CreateAsync(Message message)
        {
             try
    {
        await _dbSet.AddAsync(message);
        await _context.SaveChangesAsync();
        return message;
    }
    catch (DbUpdateException ex)
    {
        // In ra thông báo lỗi chi tiết
        throw new InvalidOperationException("Error while saving message. Inner exception: " + ex.InnerException?.Message, ex);
    }
        }

        public async Task<bool> DeleteAsync(long messageId)
        {
            var message = await _dbSet.FindAsync(messageId);
            if (message == null) return false;

            _dbSet.Remove(message);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<(IEnumerable<Message> Messages, int TotalCount)> SearchMessagesAsync(long conversationId, string query, int page, int pageSize)
        {
            var queryMessages = _dbSet
                .Where(m => m.ConversationId == conversationId && m.Content.Contains(query))
                .OrderByDescending(m => m.CreatedAt);

            var totalCount = await queryMessages.CountAsync();
            var messages = await queryMessages
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return (messages, totalCount);
        }

        public async Task<int> GetUnreadCountForUserAsync(long conversationId, long userId)
        {
            return await _dbSet
                .CountAsync(m => m.ConversationId == conversationId &&
                                m.SenderId != userId &&
                                !m.IsRead);
        }

        public async Task<int> GetTotalUnreadCountForUserAsync(long userId)
        {
            return await _dbSet
                .Include(m => m.Conversation)
                    .ThenInclude(c => c.Match)
                .CountAsync(m => m.SenderId != userId &&
                                !m.IsRead &&
                                (m.Conversation.Match.Student.UserId == userId ||
                                 m.Conversation.Match.Tutor.UserId == userId));
        }

        public async Task<Dictionary<long, int>> GetUnreadCountPerConversationAsync(long userId)
        {
            return await _dbSet
                .Include(m => m.Conversation)
                    .ThenInclude(c => c.Match)
                .Where(m => m.SenderId != userId &&
                           !m.IsRead &&
                           (m.Conversation.Match.Student.UserId == userId ||
                            m.Conversation.Match.Tutor.UserId == userId))
                .GroupBy(m => m.ConversationId)
                .ToDictionaryAsync(g => g.Key, g => g.Count());
        }

        public async Task MarkMessagesAsReadAsync(long conversationId, long userId, long? lastMessageId = null)
        {
            var query = _dbSet
                .Where(m => m.ConversationId == conversationId &&
                           m.SenderId != userId &&
                           !m.IsRead);

            if (lastMessageId.HasValue)
            {
                query = query.Where(m => m.Id <= lastMessageId.Value);
            }

            var messages = await query.ToListAsync();

            foreach (var message in messages)
            {
                message.IsRead = true;
                message.ReadAt = DateTime.UtcNow;
                message.UpdatedAt = DateTime.UtcNow;
            }

            _context.UpdateRange(messages);
        }

        public async Task<bool> IsUserInConversationAsync(long conversationId, long userId)
        {
            return await _dbSet
                .Where(m => m.ConversationId == conversationId &&
                            (m.SenderId == userId || m.Conversation.Match.Student.UserId == userId || m.Conversation.Match.Tutor.UserId == userId))
                .AnyAsync();
        }
    }
}
