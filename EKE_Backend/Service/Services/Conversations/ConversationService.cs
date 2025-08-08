using AutoMapper;
using Repository.Entities;
using Repository.Repositories.Conversations;
using Repository.Repositories.Matches;
using Repository.Repositories.Students;
using Repository.Repositories.Tutors;
using Service.DTO.Response;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Service.Services.Conversations
{
    public class ConversationService : IConversationService
    {
        private readonly IConversationRepository _conversationRepository;
        private readonly IMatchRepository _matchRepository;
        private readonly IStudentRepository _studentRepository;
        private readonly ITutorRepository _tutorRepository;
        private readonly IMapper _mapper;

        public ConversationService(IConversationRepository conversationRepository, IMatchRepository matchRepository, IMapper mapper,
            IStudentRepository studentRepository, ITutorRepository tutorRepository)
        {
            _conversationRepository = conversationRepository;
            _matchRepository = matchRepository;
            _studentRepository = studentRepository;
            _tutorRepository = tutorRepository;
            _mapper = mapper;
        }

        public async Task<ConversationResponseDto> GetOrCreateConversationAsync(long matchId)
        {
            // Lấy thông tin match từ matchId
            var match = await _matchRepository.GetByIdAsync(matchId);
            if (match == null)
            {
                // Nếu không tìm thấy match, ném ra lỗi chi tiết
                throw new ArgumentException($"Match không hợp lệ. Không tìm thấy Match với MatchId: {matchId}");
            }

            var conversation = await _conversationRepository.GetByMatchIdAsync(matchId);

            // Nếu không có cuộc trò chuyện, tạo mới
            if (conversation == null)
            {
                conversation = new Conversation
                {
                    MatchId = matchId,
                    LastMessageAt = DateTime.UtcNow
                };

                await _conversationRepository.CreateAsync(conversation);
            }

            var conversationDto = _mapper.Map<ConversationResponseDto>(conversation);

            // Kiểm tra null trước khi truy cập vào Match, Student, Tutor
            if (conversation.Match != null)
            {
                // Lấy thông tin StudentId và TutorId từ Match
                conversationDto.StudentId = conversation.Match.StudentId;
                conversationDto.TutorId = conversation.Match.TutorId;

                // Lấy tên học sinh và gia sư từ User trong bảng Student và Tutor
                var student = await _studentRepository.GetStudentWithUserInfoAsync(conversation.Match.StudentId);  // Lấy thông tin học sinh
                var tutor = await _tutorRepository.GetTutorWithDetailsAsync(conversation.Match.TutorId);            // Lấy thông tin gia sư

                conversationDto.StudentName = student?.User?.FullName ?? "Unknown";  // Tên học sinh
                conversationDto.TutorName = tutor?.User?.FullName ?? "Unknown";      // Tên gia sư
            }

            conversationDto.LastMessage = "Thông tin tin nhắn cuối cùng";  // Lấy tin nhắn cuối cùng nếu cần

            return conversationDto;
        }





        public async Task<IEnumerable<ConversationResponseDto>> GetUserConversationsAsync(long userId)
        {
            var conversations = await _conversationRepository.GetUserConversationsAsync(userId);
            return _mapper.Map<IEnumerable<ConversationResponseDto>>(conversations);
        }
        public async Task<ConversationResponseDto> GetConversationByIdAsync(long conversationId)
        {
            var conversation = await _conversationRepository.GetByIdAsync(conversationId); // Lấy cuộc trò chuyện theo conversationId

            if (conversation == null)
            {
                return null;  // Nếu không tìm thấy cuộc trò chuyện
            }

            var conversationDto = _mapper.Map<ConversationResponseDto>(conversation);

            // Kiểm tra nếu Match tồn tại
            if (conversation.Match != null)
            {
                // Lấy thông tin StudentId và TutorId từ Match
                conversationDto.StudentId = conversation.Match.StudentId;
                conversationDto.TutorId = conversation.Match.TutorId;

                // Lấy tên học sinh và gia sư từ bảng User
                var student = await _studentRepository.GetStudentWithUserInfoAsync(conversation.Match.StudentId);  // Lấy thông tin học sinh
                var tutor = await _tutorRepository.GetTutorWithDetailsAsync(conversation.Match.TutorId);            // Lấy thông tin gia sư

                // Kiểm tra null và lấy tên
                conversationDto.StudentName = student?.User?.FullName ?? "Unknown";
                conversationDto.TutorName = tutor?.User?.FullName ?? "Unknown";
            }
            else
            {
                // Thêm thông báo chi tiết khi không có Match
                var errorMessage = $"Không tìm thấy Match cho cuộc trò chuyện này. " +
                                   $"Conversation ID: {conversation.Id}, " +
                                   $"Match ID: {conversation.MatchId}, " +
                                   $"Student ID: {conversation.Match?.StudentId ?? 0}, " +
                                   $"Tutor ID: {conversation.Match?.TutorId ?? 0}, " +
                                   $"LastMessageAt: {conversation.LastMessageAt}";

                throw new InvalidOperationException(errorMessage);
            }

            return conversationDto;
        }


    }
}
