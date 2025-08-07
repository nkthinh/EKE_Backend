using Repository.Repositories.Conversations;
using Repository.Repositories.Matches;
using Repository.Entities;
using Service.DTO.Response;
using AutoMapper;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Service.Services.Conversations
{
    public class ConversationService : IConversationService
    {
        private readonly IConversationRepository _conversationRepository;
        private readonly IMatchRepository _matchRepository;
        private readonly IMapper _mapper;

        public ConversationService(IConversationRepository conversationRepository, IMatchRepository matchRepository, IMapper mapper)
        {
            _conversationRepository = conversationRepository;
            _matchRepository = matchRepository;
            _mapper = mapper;
        }

        public async Task<ConversationResponseDto> GetOrCreateConversationAsync(long matchId)
        {
            var conversation = await _conversationRepository.GetByMatchIdAsync(matchId);

            // Nếu không có cuộc trò chuyện, tạo mới
            if (conversation == null)
            {
                var match = await _matchRepository.GetByIdAsync(matchId);
                if (match == null)
                {
                    throw new ArgumentException("Match không hợp lệ");
                }

                conversation = new Conversation
                {
                    MatchId = matchId,
                    LastMessageAt = DateTime.UtcNow
                };

                await _conversationRepository.CreateAsync(conversation);
            }

            var conversationDto = _mapper.Map<ConversationResponseDto>(conversation);

            // Set Partner info
            var partner = conversation.Match.Student.UserId != conversationDto.Partner.Id
                ? new UserBasicInfoDto
                {
                    Id = conversation.Match.Student.UserId,
                    FullName = conversation.Match.Student.User.FullName,
                    ProfileImage = conversation.Match.Student.User.ProfileImage,
                    IsOnline = true // Có thể kiểm tra trạng thái online theo cách nào đó, ví dụ SignalR
                }
                : new UserBasicInfoDto
                {
                    Id = conversation.Match.Tutor.UserId,
                    FullName = conversation.Match.Tutor.User.FullName,
                    ProfileImage = conversation.Match.Tutor.User.ProfileImage,
                    IsOnline = true // Cũng tương tự như trên
                };

            conversationDto.Partner = partner;
            conversationDto.LastMessage = "Thông tin tin nhắn cuối cùng";  // Lấy tin nhắn cuối cùng nếu cần

            return conversationDto;
        }

        public async Task<IEnumerable<ConversationResponseDto>> GetUserConversationsAsync(long userId)
        {
            var conversations = await _conversationRepository.GetUserConversationsAsync(userId);
            return _mapper.Map<IEnumerable<ConversationResponseDto>>(conversations);
        }
    }
}
