using Service.DTO.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.Services.Conversations
{
    public interface IConversationService
    {
        Task<ConversationResponseDto> GetOrCreateConversationAsync(long matchId);
        Task<IEnumerable<ConversationResponseDto>> GetUserConversationsAsync(long userId);
    }
}
