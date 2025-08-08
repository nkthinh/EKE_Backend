using Microsoft.AspNetCore.SignalR;

namespace EKE_Backend
{
    public class ChatHub : Hub
    {
        // Tham gia vào nhóm (Group) theo ConversationId
        public async Task JoinConversation(long conversationId)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, $"Conversation_{conversationId}");
        }

        // Rời nhóm (Group) khi kết thúc cuộc trò chuyện
        public async Task LeaveConversation(long conversationId)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, $"Conversation_{conversationId}");
        }
    }

}

