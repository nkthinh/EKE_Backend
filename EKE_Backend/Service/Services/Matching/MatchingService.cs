using AutoMapper;
using Microsoft.Extensions.Logging;
using Repository.Entities;
using Repository.Enums;
using Repository.UnitOfWork;
using Service.DTO.Request;
using Service.DTO.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.Services.Matching
{
    public class MatchingService : IMatchingService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ILogger<MatchingService> _logger;

        public MatchingService(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            ILogger<MatchingService> logger)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<IEnumerable<TutorCardDto>> GetTutorCardsForStudentAsync(long studentId, int count = 10)
        {
            try
            {
                // Get student preferences (could be from Student entity or separate preference table)
                var student = await _unitOfWork.Students.GetStudentWithUserInfoAsync(studentId);
                if (student == null)
                {
                    throw new ArgumentException("Student not found");
                }

                // Get tutors that student hasn't swiped yet
                var swipedTutorIds = await _unitOfWork.SwipeActions.GetSwipedTutorIdsByStudentAsync(studentId);

                // Get available tutors (excluding already swiped ones)
                var tutors = await _unitOfWork.Tutors.GetAvailableTutorsForMatchingAsync(
                    studentId, swipedTutorIds, count);

                return _mapper.Map<IEnumerable<TutorCardDto>>(tutors);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting tutor cards for student: {StudentId}", studentId);
                throw;
            }
        }

        public async Task<SwipeResultDto> SwipeTutorAsync(long studentId, SwipeActionDto swipeDto)
        {
            try
            {
                // Create swipe action
                var swipeAction = new SwipeAction
                {
                    StudentId = studentId,
                    TutorId = swipeDto.TutorId,
                    Action = swipeDto.Action,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                };

                await _unitOfWork.SwipeActions.AddAsync(swipeAction);

                // Check if it's a like and if tutor would like this student back
                if (swipeDto.Action == SwipeActionType.Like)
                {
                    // Check if tutor has liked this student (in future, could have tutor preferences)
                    // For now, auto-create pending match
                    var existingMatch = await _unitOfWork.Matches.GetByStudentAndTutorAsync(studentId, swipeDto.TutorId);

                    if (existingMatch == null)
                    {
                        var match = new Match
                        {
                            StudentId = studentId,
                            TutorId = swipeDto.TutorId,
                            Status = MatchStatus.Pending, // Waiting for tutor approval
                            MatchedAt = DateTime.UtcNow,
                            LastActivity = DateTime.UtcNow,
                            CreatedAt = DateTime.UtcNow,
                            UpdatedAt = DateTime.UtcNow
                        };

                        await _unitOfWork.Matches.AddAsync(match);
                        await _unitOfWork.CompleteAsync();

                        return new SwipeResultDto
                        {
                            IsMatch = true,
                            MatchId = match.Id,
                            Message = "Đã gửi yêu cầu kết nối đến gia sư!"
                        };
                    }
                }

                await _unitOfWork.CompleteAsync();

                return new SwipeResultDto
                {
                    IsMatch = false,
                    Message = swipeDto.Action == SwipeActionType.Like ? "Đã thích!" : "Đã bỏ qua!"
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error swiping tutor: StudentId={StudentId}, TutorId={TutorId}",
                    studentId, swipeDto.TutorId);
                throw;
            }
        }

        public async Task<IEnumerable<DTO.Response.MatchResponseDto>> GetPendingMatchesForTutorAsync(long tutorId)
        {
            try
            {
                var matches = await _unitOfWork.Matches.GetPendingMatchesForTutorAsync(tutorId);
                return _mapper.Map<IEnumerable<DTO.Response.MatchResponseDto>>(matches);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting pending matches for tutor: {TutorId}", tutorId);
                throw;
            }
        }

        public async Task<MatchResponseDto> RespondToMatchAsync(long tutorId, long matchId, bool accept)
        {
            try
            {
                var match = await _unitOfWork.Matches.GetByIdAsync(matchId);
                if (match == null || match.TutorId != tutorId)
                {
                    throw new ArgumentException("Match not found or unauthorized");
                }

                if (match.Status != MatchStatus.Pending)
                {
                    throw new InvalidOperationException("Match is not in pending status");
                }

                match.Status = accept ? MatchStatus.Active : MatchStatus.Rejected;
                match.LastActivity = DateTime.UtcNow;
                match.UpdatedAt = DateTime.UtcNow;

                // Create conversation if accepted
                if (accept)
                {
                    var conversation = new Conversation
                    {
                        MatchId = matchId,
                        CreatedAt = DateTime.UtcNow,
                        UpdatedAt = DateTime.UtcNow
                    };

                    await _unitOfWork.Conversations.AddAsync(conversation);
                }

                _unitOfWork.Matches.Update(match);
                await _unitOfWork.CompleteAsync();

                // Return updated match with details
                var updatedMatch = await _unitOfWork.Matches.GetMatchWithDetailsAsync(matchId);
                return _mapper.Map<DTO.Response.MatchResponseDto>(updatedMatch);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error responding to match: MatchId={MatchId}, Accept={Accept}",
                    matchId, accept);
                throw;
            }
        }

        public async Task<IEnumerable<DTO.Response.MatchResponseDto>> GetActiveMatchesAsync(long userId, string userRole)
        {
            try
            {
                IEnumerable<Match> matches;

                if (userRole == "Student")
                {
                    var student = await _unitOfWork.Students.GetByUserIdAsync(userId);
                    if (student == null) throw new ArgumentException("Student not found");

                    matches = await _unitOfWork.Matches.GetActiveMatchesForStudentAsync(student.Id);
                }
                else if (userRole == "Tutor")
                {
                    var tutor = await _unitOfWork.Tutors.GetTutorByUserIdAsync(userId);
                    if (tutor == null) throw new ArgumentException("Tutor not found");

                    matches = await _unitOfWork.Matches.GetActiveMatchesForTutorAsync(tutor.Id);
                }
                else
                {
                    throw new ArgumentException("Invalid user role");
                }

                return _mapper.Map<IEnumerable<MatchResponseDto>>(matches);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting active matches: UserId={UserId}, Role={Role}",
                    userId, userRole);
                throw;
            }
        }

        public async Task<DTO.Response.MatchResponseDto?> GetMatchByIdAsync(long matchId)
        {
            try
            {
                var match = await _unitOfWork.Matches.GetMatchWithDetailsAsync(matchId);
                return match != null ? _mapper.Map<DTO.Response.MatchResponseDto>(match) : null;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting match by id: {MatchId}", matchId);
                throw;
            }
        }

        public async Task<int> GetTotalMatchesCountAsync(long userId, string userRole)
        {
            try
            {
                if (userRole == "Student")
                {
                    var student = await _unitOfWork.Students.GetByUserIdAsync(userId);
                    if (student == null) return 0;

                    return await _unitOfWork.Matches.CountActiveMatchesForStudentAsync(student.Id);
                }
                else if (userRole == "Tutor")
                {
                    var tutor = await _unitOfWork.Tutors.GetTutorByUserIdAsync(userId);
                    if (tutor == null) return 0;

                    return await _unitOfWork.Matches.CountActiveMatchesForTutorAsync(tutor.Id);
                }

                return 0;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting total matches count: UserId={UserId}, Role={Role}",
                    userId, userRole);
                throw;
            }
        }

        public async Task<int> GetPendingMatchesCountAsync(long tutorId)
        {
            try
            {
                return await _unitOfWork.Matches.CountPendingMatchesForTutorAsync(tutorId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting pending matches count for tutor: {TutorId}", tutorId);
                throw;
            }
        }

  
    }
}
