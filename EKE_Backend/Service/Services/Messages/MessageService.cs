using AutoMapper;
using Microsoft.AspNetCore.Http;
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
using System.Net.Http;
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

        public async Task<MessageResponseDto> SendMessageAsync(MessageCreateDto messageDto)
        {
            // Kiểm tra nếu SenderId có hợp lệ không (bằng currentUserId đã truyền vào)
            var currentUserId = messageDto.SenderId; // Đã có SenderId từ API controller, không cần lấy lại

            // Kiểm tra nếu conversationId hợp lệ
            var conversation = await _conversationRepository.GetByIdAsync(messageDto.ConversationId);
            if (conversation == null)
            {
                throw new ArgumentException("Conversation không hợp lệ");
            }

            // Lấy match từ cuộc trò chuyện, xác định StudentId và TutorId
            var match = await _matchRepository.GetByIdAsync(conversation.MatchId);
            if (match == null)
            {
                throw new ArgumentException("Không tìm thấy Match cho cuộc trò chuyện này");
            }

            // Kiểm tra xem người dùng có phải là học sinh hoặc gia sư trong cuộc trò chuyện không
            var isStudent = await _studentRepository.GetStudentIdByUserIdAsync(currentUserId) != null;
            var isTutor = await _tutorRepository.GetTutorIdByUserIdAsync(currentUserId) != null;

            if (!isStudent && !isTutor)
            {
                throw new UnauthorizedAccessException("Người dùng không phải học sinh hoặc gia sư hợp lệ");
            }

           
            // Tiến hành tạo tin nhắn
            var message = _mapper.Map<Message>(messageDto);
            await _messageRepository.CreateAsync(message);

            // Trả về tin nhắn
            return _mapper.Map<MessageResponseDto>(message);
        }




        // Gửi tin nhắn có đính kèm tệp
        public async Task<MessageResponseDto> SendMessageWithFileAsync(MessageWithFileDto messageDto)
        {
            var message = _mapper.Map<Message>(messageDto);
            await _messageRepository.CreateAsync(message);
            return _mapper.Map<MessageResponseDto>(message);
        }



        public async Task<(IEnumerable<MessageResponseDto> Messages, int TotalCount)> GetConversationMessagesAsync(long conversationId, int page, int pageSize)
        {
            // Lấy tất cả tin nhắn trong cuộc trò chuyện
            var (messages, totalCount) = await _messageRepository.GetPagedMessagesAsync(conversationId, page, pageSize);

            // Chuyển đổi sang DTO trước khi trả về
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

        public async Task<bool> IsUserInConversationAsync(long conversationId, long userId)
        {
            // Lấy user từ bảng Student hoặc Tutor
            var student = await _studentRepository.GetStudentByUserIdAsync(userId);  // Kiểm tra nếu userId là học sinh
            var tutor = await _tutorRepository.GetTutorByUserIdAsync(userId);        // Kiểm tra nếu userId là gia sư

            // Nếu không phải học sinh và cũng không phải gia sư, ném lỗi
            if (student == null && tutor == null)
            {
                throw new UnauthorizedAccessException("User không phải học sinh hoặc gia sư hợp lệ");
            }

            // Lấy cuộc trò chuyện bằng conversationId
            var conversation = await _conversationRepository.GetByIdAsync(conversationId);
            if (conversation == null)
            {
                throw new ArgumentException($"Không tìm thấy cuộc trò chuyện với ID {conversationId}");
            }

            // Lấy match từ cuộc trò chuyện, xác định StudentId và TutorId
            var match = await _matchRepository.GetByIdAsync(conversation.MatchId);
            if (match == null)
            {
                throw new ArgumentException($"Không tìm thấy match với ID {conversation.MatchId}");
            }

            // Kiểm tra nếu người dùng là học sinh (so sánh với StudentId) hoặc gia sư (so sánh với TutorId)
            if (student != null && match.StudentId == student.Id)
            {
                return true;  // Người dùng là học sinh và là phần của match
            }

            if (tutor != null && match.TutorId == tutor.Id)
            {
                return true;  // Người dùng là gia sư và là phần của match
            }

            return false;  // Người dùng không phải học sinh hoặc gia sư trong cuộc trò chuyện
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
