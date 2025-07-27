using AutoMapper;
using Microsoft.Extensions.Logging;
using Repository.Entities;
using Repository.UnitOfWork;
using Service.DTO.Request;
using Service.DTO.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.Services.Chat
{
    public class ChatService : IChatService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ILogger<ChatService> _logger;

        public ChatService(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            ILogger<ChatService> logger)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<IEnumerable<ConversationResponseDto>> GetUserConversationsAsync(long userId)
        {
            try
            {
                var conversations = await _unitOfWork.Conversations.GetUserConversationsAsync(userId);
                var result = new List<ConversationResponseDto>();

                foreach (var conversation in conversations)
                {
                    var dto = _mapper.Map<ConversationResponseDto>(conversation);

                    // Get partner info (the other user in the match)
                    var match = conversation.Match;
                    var partnerId = match.Student.UserId == userId ? match.Tutor.UserId : match.Student.UserId;
                    var partner = partnerId == match.Student.UserId ? match.Student.User : match.Tutor.User;

                    dto.Partner = new UserBasicInfoDto
                    {
                        Id = partner.Id,
                        FullName = partner.FullName,
                        ProfileImage = partner.ProfileImage,
                        IsOnline = false, // TODO: Implement online status
                        LastSeen = null // TODO: Implement last seen
                    };

                    // Get unread count for this conversation
                    dto.UnreadCount = await _unitOfWork.Messages.GetUnreadCountForUserAsync(conversation.Id, userId);

                    // Get last message
                    var lastMessage = conversation.GetLastMessage();
                    if (lastMessage != null)
                    {
                        dto.LastMessage = lastMessage.Content;
                        dto.LastMessageAt = lastMessage.CreatedAt;
                    }

                    result.Add(dto);
                }

                return result.OrderByDescending(c => c.LastMessageAt ?? c.CreatedAt);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting conversations for user: {UserId}", userId);
                throw;
            }
        }

        public async Task<ConversationResponseDto?> GetConversationByIdAsync(long conversationId, long userId)
        {
            try
            {
                var conversation = await _unitOfWork.Conversations.GetConversationWithDetailsAsync(conversationId);
                if (conversation == null) return null;

                // Check if user is part of this conversation
                var match = conversation.Match;
                if (match.Student.UserId != userId && match.Tutor.UserId != userId)
                {
                    throw new UnauthorizedAccessException("User is not part of this conversation");
                }

                var dto = _mapper.Map<ConversationResponseDto>(conversation);

                // Set partner info
                var partnerId = match.Student.UserId == userId ? match.Tutor.UserId : match.Student.UserId;
                var partner = partnerId == match.Student.UserId ? match.Student.User : match.Tutor.User;

                dto.Partner = new UserBasicInfoDto
                {
                    Id = partner.Id,
                    FullName = partner.FullName,
                    ProfileImage = partner.ProfileImage,
                    IsOnline = false, // TODO: Implement
                    LastSeen = null // TODO: Implement
                };

                dto.UnreadCount = await _unitOfWork.Messages.GetUnreadCountForUserAsync(conversationId, userId);

                return dto;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting conversation: ConversationId={ConversationId}, UserId={UserId}",
                    conversationId, userId);
                throw;
            }
        }

        public async Task<ConversationResponseDto?> GetConversationByMatchIdAsync(long matchId, long userId)
        {
            try
            {
                var conversation = await _unitOfWork.Conversations.GetByMatchIdAsync(matchId);
                if (conversation == null) return null;

                return await GetConversationByIdAsync(conversation.Id, userId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting conversation by match: MatchId={MatchId}, UserId={UserId}",
                    matchId, userId);
                throw;
            }
        }

        public async Task<ChatHistoryDto> GetChatHistoryAsync(long conversationId, long userId, int page = 1, int pageSize = 50)
        {
            try
            {
                // Get conversation info
                var conversation = await GetConversationByIdAsync(conversationId, userId);
                if (conversation == null)
                {
                    throw new ArgumentException("Conversation not found or unauthorized");
                }

                // Get paginated messages
                var (messages, totalCount) = await _unitOfWork.Messages.GetPagedMessagesAsync(
                    conversationId, page, pageSize);

                var messageDtos = messages.Select(m => new MessageResponseDto
                {
                    Id = m.Id,
                    ConversationId = m.ConversationId,
                    SenderId = m.SenderId,
                    MessageType = m.MessageType,
                    Content = m.Content,
                    FileUrl = m.FileUrl,
                    IsRead = m.IsRead,
                    ReadAt = m.ReadAt,
                    CreatedAt = m.CreatedAt,
                    SenderName = m.Sender.FullName,
                    SenderAvatar = m.Sender.ProfileImage,
                    IsMine = m.SenderId == userId
                }).ToList();

                return new ChatHistoryDto
                {
                    Conversation = conversation,
                    Messages = messageDtos,
                    HasMoreMessages = (page * pageSize) < totalCount,
                    TotalCount = totalCount
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting chat history: ConversationId={ConversationId}, UserId={UserId}",
                    conversationId, userId);
                throw;
            }
        }

        public async Task<MessageResponseDto> SendMessageAsync(long userId, SendMessageDto messageDto)
        {
            try
            {
                // Verify user can send to this conversation
                var conversation = await GetConversationByIdAsync(messageDto.ConversationId, userId);
                if (conversation == null)
                {
                    throw new ArgumentException("Conversation not found or unauthorized");
                }

                var message = new Message
                {
                    ConversationId = messageDto.ConversationId,
                    SenderId = userId,
                    MessageType = messageDto.MessageType,
                    Content = messageDto.Content,
                    FileUrl = messageDto.FileUrl,
                    IsRead = false,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                };

                await _unitOfWork.Messages.AddAsync(message);

                // Update conversation last message info
                var conversationEntity = await _unitOfWork.Conversations.GetByIdAsync(messageDto.ConversationId);
                if (conversationEntity != null)
                {
                    conversationEntity.LastMessageId = message.Id;
                    conversationEntity.LastMessageAt = message.CreatedAt;
                    conversationEntity.UpdatedAt = DateTime.UtcNow;
                    _unitOfWork.Conversations.Update(conversationEntity);

                    // Update match last activity
                    var match = await _unitOfWork.Matches.GetByIdAsync(conversationEntity.MatchId);
                    if (match != null)
                    {
                        match.LastActivity = DateTime.UtcNow;
                        match.UpdatedAt = DateTime.UtcNow;
                        _unitOfWork.Matches.Update(match);
                    }
                }

                await _unitOfWork.CompleteAsync();

                // Return message with sender info
                var createdMessage = await _unitOfWork.Messages.GetMessageWithSenderAsync(message.Id);
                return new MessageResponseDto
                {
                    Id = createdMessage.Id,
                    ConversationId = createdMessage.ConversationId,
                    SenderId = createdMessage.SenderId,
                    MessageType = createdMessage.MessageType,
                    Content = createdMessage.Content,
                    FileUrl = createdMessage.FileUrl,
                    IsRead = createdMessage.IsRead,
                    ReadAt = createdMessage.ReadAt,
                    CreatedAt = createdMessage.CreatedAt,
                    SenderName = createdMessage.Sender.FullName,
                    SenderAvatar = createdMessage.Sender.ProfileImage,
                    IsMine = true
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error sending message: UserId={UserId}, ConversationId={ConversationId}",
                    userId, messageDto.ConversationId);
                throw;
            }
        }

        public async Task<bool> MarkMessagesAsReadAsync(long userId, MarkAsReadDto markAsReadDto)
        {
            try
            {
                // Verify user can access this conversation
                var conversation = await GetConversationByIdAsync(markAsReadDto.ConversationId, userId);
                if (conversation == null) return false;

                await _unitOfWork.Messages.MarkMessagesAsReadAsync(
                    markAsReadDto.ConversationId, userId, markAsReadDto.LastMessageId);

                await _unitOfWork.CompleteAsync();
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error marking messages as read: UserId={UserId}, ConversationId={ConversationId}",
                    userId, markAsReadDto.ConversationId);
                throw;
            }
        }

        public async Task<int> GetUnreadMessagesCountAsync(long userId)
        {
            try
            {
                return await _unitOfWork.Messages.GetTotalUnreadCountForUserAsync(userId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting unread messages count: UserId={UserId}", userId);
                throw;
            }
        }

        public async Task<Dictionary<long, int>> GetUnreadCountPerConversationAsync(long userId)
        {
            try
            {
                return await _unitOfWork.Messages.GetUnreadCountPerConversationAsync(userId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting unread count per conversation: UserId={UserId}", userId);
                throw;
            }
        }

        public async Task<bool> SetUserOnlineStatusAsync(long userId, bool isOnline)
        {
            try
            {
                // TODO: Implement online status (could use Redis or in-memory cache)
                // For now, just update user's last seen if going offline
                if (!isOnline)
                {
                    var user = await _unitOfWork.Users.GetByIdAsync(userId);
                    if (user != null)
                    {
                        // Could add LastSeen field to User entity
                        user.UpdatedAt = DateTime.UtcNow;
                        _unitOfWork.Users.Update(user);
                        await _unitOfWork.CompleteAsync();
                    }
                }
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error setting online status: UserId={UserId}, IsOnline={IsOnline}",
                    userId, isOnline);
                throw;
            }
        }

        public async Task<DateTime?> GetLastSeenAsync(long userId)
        {
            try
            {
                // TODO: Implement proper last seen tracking
                var user = await _unitOfWork.Users.GetByIdAsync(userId);
                return user?.UpdatedAt;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting last seen: UserId={UserId}", userId);
                throw;
            }
        }
    }
}

