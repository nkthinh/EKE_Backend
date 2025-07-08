using Repository.Entities;
using Repository.UnitOfWork;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.Services.Conversations
{
    //public class ConversationService
    //{
    //    private readonly IUnitOfWork _unitOfWork;

    //    public async Task UpdateLastMessageAsync(long conversationId, long messageId)
    //    {
    //        var conversation = await _unitOfWork.Conversations.GetByIdAsync(conversationId);
    //        if (conversation != null)
    //        {
    //            conversation.LastMessageId = messageId;
    //            conversation.LastMessageAt = DateTime.UtcNow;
    //            _unitOfWork.Conversations.Update(conversation);
    //            await _unitOfWork.CompleteAsync();
    //        }
    //    }

    //    public async Task<Message?> GetLastMessageAsync(long conversationId)
    //    {
    //        return await _unitOfWork.Messages
    //            .FirstOrDefaultAsync(m => m.ConversationId == conversationId &&
    //                                m.Id == (await _unitOfWork.Conversations.GetByIdAsync(conversationId))!.LastMessageId);
    //    }
    //}
}
