using AutoMapper;
using Repository.Repositories.Messages;
using Repository.Repositories.Conversations;
using Repository.Entities;
using Service.DTO.Request;
using Service.DTO.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Repository.Repositories;
using Repository.Repositories.Matches;

namespace Service.Services.Messages
{
    public class MessageService : IMessageService
    {
        private readonly IMessageRepository _messageRepository;
        private readonly IConversationRepository _conversationRepository;
        private readonly IMapper _mapper;
        private readonly IMatchRepository _matchRepository;

        public MessageService(IMessageRepository messageRepository, IConversationRepository conversationRepository, IMapper mapper, IMatchRepository matchRepository)
        {
            _messageRepository = messageRepository;
            _conversationRepository = conversationRepository;
            _mapper = mapper;
            _matchRepository = matchRepository;
        }

        // Gửi tin nhắn
        public async Task<MessageResponseDto> SendMessageAsync(MessageCreateDto messageDto)
        {
            var message = _mapper.Map<Message>(messageDto);
            await _messageRepository.CreateAsync(message);
            return _mapper.Map<MessageResponseDto>(message);
        }

        // Gửi tin nhắn có đính kèm tệp
        public async Task<MessageResponseDto> SendMessageWithFileAsync(MessageWithFileDto messageDto)
        {
            var message = _mapper.Map<Message>(messageDto);
            await _messageRepository.CreateAsync(message);
            return _mapper.Map<MessageResponseDto>(message);
        }

        // Lấy tất cả tin nhắn trong một cuộc trò chuyện
        public async Task<(IEnumerable<MessageResponseDto> Messages, int TotalCount)> GetConversationMessagesAsync(long conversationId, int page, int pageSize)
        {
            var (messages, totalCount) = await _messageRepository.GetPagedMessagesAsync(conversationId, page, pageSize);
            var messageDtos = _mapper.Map<IEnumerable<MessageResponseDto>>(messages);
            return (messageDtos, totalCount);
        }

        // Đánh dấu tin nhắn là đã đọc
        public async Task MarkMessagesAsReadAsync(long conversationId, long userId)
        {
            await _messageRepository.MarkMessagesAsReadAsync(conversationId, userId);
        }

        // Tìm kiếm tin nhắn trong một cuộc trò chuyện
        public async Task<(IEnumerable<MessageResponseDto> Messages, int TotalCount)> SearchMessagesAsync(long conversationId, string query, int page, int pageSize)
        {
            var (messages, totalCount) = await _messageRepository.SearchMessagesAsync(conversationId, query, page, pageSize);
            var messageDtos = _mapper.Map<IEnumerable<MessageResponseDto>>(messages);
            return (messageDtos, totalCount);
        }

        // Kiểm tra xem người dùng có phải là thành viên trong cuộc trò chuyện không
        public async Task<bool> IsUserInConversationAsync(long conversationId, long userId)
        {
            return await _messageRepository.IsUserInConversationAsync(conversationId, userId);
        }

        // Đếm số lượng tin nhắn chưa đọc cho người dùng
        public async Task<int> GetUnreadMessageCountAsync(long userId)
        {
            return await _messageRepository.GetTotalUnreadCountForUserAsync(userId);
        }

        // Tạo hoặc lấy cuộc trò chuyện dựa trên Match ID
        public async Task<ConversationResponseDto> GetOrCreateConversationAsync(long matchId)
        {
            var conversation = await _conversationRepository.GetByMatchIdAsync(matchId);

            // Nếu chưa có cuộc trò chuyện, tạo một cuộc trò chuyện mới
            if (conversation == null)
            {
                var match = await _matchRepository.GetByIdAsync(matchId);
                if (match == null)
                    throw new ArgumentException("Match không hợp lệ");

                conversation = new Conversation
                {
                    MatchId = matchId,
                    LastMessageAt = DateTime.UtcNow
                };

                await _conversationRepository.CreateAsync(conversation);
            }

            return _mapper.Map<ConversationResponseDto>(conversation);
        }

        // Lấy tất cả các cuộc trò chuyện của người dùng
        public async Task<IEnumerable<ConversationResponseDto>> GetUserConversationsAsync(long userId)
        {
            var conversations = await _conversationRepository.GetUserConversationsAsync(userId);
            return _mapper.Map<IEnumerable<ConversationResponseDto>>(conversations);
        }

        // Xóa tin nhắn
        public async Task<bool> DeleteMessageAsync(long messageId, long userId)
        {
            var message = await _messageRepository.GetMessageWithSenderAsync(messageId);
            if (message == null)
                return false;

            if (message.SenderId != userId)
                return false; // Only the sender can delete the message

            return await _messageRepository.DeleteAsync(messageId);
        }
    }
}
