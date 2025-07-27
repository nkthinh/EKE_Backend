using Service.DTO.Request;
using Service.DTO.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace Service.Services.Matching
{
    public interface IMatchingService
    {
        // For Students - Discovery
        Task<IEnumerable<TutorCardDto>> GetTutorCardsForStudentAsync(long studentId, int count = 10);
        Task<SwipeResultDto> SwipeTutorAsync(long studentId, SwipeActionDto swipeDto);

        // For Tutors - Pending matches
        Task<IEnumerable<DTO.Response.MatchResponseDto>> GetPendingMatchesForTutorAsync(long tutorId);
        Task<MatchResponseDto> RespondToMatchAsync(long tutorId, long matchId, bool accept);

        // For Both - Active matches
        Task<IEnumerable<MatchResponseDto>> GetActiveMatchesAsync(long userId, string userRole);
        Task<MatchResponseDto?> GetMatchByIdAsync(long matchId);

        // Statistics
        Task<int> GetTotalMatchesCountAsync(long userId, string userRole);
        Task<int> GetPendingMatchesCountAsync(long tutorId);
    }
}
