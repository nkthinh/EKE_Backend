using Repository.Entities;
using Repository.Repositories.BaseRepository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository.Repositories.Conversations
{
    public interface IConversationRepository : IBaseRepository<Conversation>
    {
        Task<Conversation?> GetByMatchIdAsync(long matchId);
        Task<IEnumerable<Conversation>> GetUserConversationsAsync(long userId);
        Task<Conversation?> GetConversationWithDetailsAsync(long conversationId);
        Task<Conversation> CreateAsync(Conversation conversation);
    }
}
