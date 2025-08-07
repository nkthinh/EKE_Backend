using Repository.Enums;
using Service.DTO.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.Services.SwipeActions
{
    public interface ISwipeActionService
    {
        Task<SwipeActionResponseDto> Swipe(long studentId, long tutorId, SwipeActionType action);
        Task<SwipeActionResponseDto> AcceptMatch(long tutorId, long studentId);
        Task<IEnumerable<StudentResponseDto>> GetLikedStudentsByTutorAsync(long tutorId);
    }

}
