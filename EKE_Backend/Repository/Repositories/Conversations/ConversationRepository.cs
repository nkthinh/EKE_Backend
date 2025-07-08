using Microsoft.EntityFrameworkCore;
using Repository.Entities;
using Repository.Repositories.BaseRepository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository.Repositories.Conversations
{
    public class ConversationRepository : BaseRepository<Conversation>, IConversationRepository
    {
        public ConversationRepository(ApplicationDbContext context) : base(context) { }

        public async Task<IEnumerable<Conversation>> GetUserConversationsAsync(long userId)
        {
            return await _dbSet
                .Include(c => c.Match)
                    .ThenInclude(m => m.Student)
                        .ThenInclude(s => s.User)
                .Include(c => c.Match)
                    .ThenInclude(m => m.Tutor)
                        .ThenInclude(t => t.User)
                .Include(c => c.Messages.Take(1).OrderByDescending(msg => msg.CreatedAt))
                .Where(c => c.Match.Student.UserId == userId || c.Match.Tutor.UserId == userId)
                .OrderByDescending(c => c.LastMessageAt ?? c.CreatedAt)
                .ToListAsync();
        }

        public async Task<Conversation?> GetConversationWithDetailsAsync(long conversationId)
        {
            return await _dbSet
                .Include(c => c.Match)
                    .ThenInclude(m => m.Student)
                        .ThenInclude(s => s.User)
                .Include(c => c.Match)
                    .ThenInclude(m => m.Tutor)
                        .ThenInclude(t => t.User)
                .FirstOrDefaultAsync(c => c.Id == conversationId);
        }

        public async Task<Conversation?> GetByMatchIdAsync(long matchId)
        {
            return await _dbSet
                .Include(c => c.Match)
                    .ThenInclude(m => m.Student)
                        .ThenInclude(s => s.User)
                .Include(c => c.Match)
                    .ThenInclude(m => m.Tutor)
                        .ThenInclude(t => t.User)
                .FirstOrDefaultAsync(c => c.MatchId == matchId);
        }
    }
}
