using AutoMapper;
using Repository.Entities;
using Repository.Repositories;
using Repository.Repositories.Conversations;
using Repository.Repositories.Matches;
using Repository.Repositories.Messages;
using Repository.Repositories.Students;
using Repository.Repositories.Tutors;
using Repository.Repositories.Users;
using Service.DTO.Request;
using Service.DTO.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Service.Services.Messages
{
    public class MessageService : IMessageService
    {
        private readonly IMessageRepository _messageRepository;
        private readonly IConversationRepository _conversationRepository;
        private readonly IMapper _mapper;
        private readonly IMatchRepository _matchRepository;
        private readonly IStudentRepository _studentRepository;
        private readonly ITutorRepository _tutorRepository;
        private readonly IUserRepository _userRepository;

        public MessageService(IMessageRepository messageRepository, IConversationRepository conversationRepository, IMapper mapper,
            IMatchRepository matchRepository, IStudentRepository studentRepository, ITutorRepository tutorRepository, IUserRepository userRepository)
        {
            _messageRepository = messageRepository;
            _conversationRepository = conversationRepository;
            _mapper = mapper;
            _matchRepository = matchRepository;
            _studentRepository = studentRepository;
            _tutorRepository = tutorRepository;
            _userRepository = userRepository;
        }

        // Gửi tin nhắn
        public async Task<MessageResponseDto> SendMessageAsync(MessageCreateDto messageDto)
        {
            // Kiểm tra SenderId có hợp lệ không
            if (!await IsValidSenderId(messageDto.SenderId))
            {
                throw new ArgumentException("SenderId không hợp lệ.");
            }

            var conversation = await _conversationRepository.GetByIdAsync(messageDto.ConversationId);
            if (conversation == null)
            {
                throw new ArgumentException("Conversation không hợp lệ");
            }

            var match = await _matchRepository.GetByIdAsync(conversation.MatchId);
            if (match == null)
            {
                throw new ArgumentException("Không tìm thấy Match cho cuộc trò chuyện này");
            }

            if (match.StudentId != messageDto.SenderId && match.TutorId != messageDto.SenderId)
            {
                throw new UnauthorizedAccessException("Bạn không có quyền gửi tin nhắn trong cuộc trò chuyện này");
            }

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
            var conversation = await _conversationRepository.GetByIdAsync(conversationId);
            if (conversation == null)
            {
                throw new ArgumentException($"Conversation with ID {conversationId} not found");
            }

            var match = await _matchRepository.GetByIdAsync(conversation.MatchId);
            if (match == null)
            {
                throw new ArgumentException($"Match with ID {conversation.MatchId} not found");
            }

            // Kiểm tra quyền truy cập của học sinh hoặc gia sư
            return match.StudentId == userId || match.TutorId == userId;
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
        public async Task<bool> IsValidSenderId(long senderId)
        {
            var user = await _userRepository.GetByIdAsync(senderId);
            return user != null;
        }

    }
}
