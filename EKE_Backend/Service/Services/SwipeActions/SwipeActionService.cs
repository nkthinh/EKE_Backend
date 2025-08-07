using Microsoft.Extensions.Logging;
using Repository.Entities;
using Repository.Enums;
using Repository.Repositories.Matches;
using Repository.Repositories.Students;
using Repository.Repositories.SwipeActions;
using Service.DTO.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.Services.SwipeActions
{
    public class SwipeActionService : ISwipeActionService
    {
        private readonly ISwipeActionRepository _swipeActionRepository;
        private readonly IMatchRepository _matchRepository;
        private readonly ILogger<SwipeActionService> _logger;
        private readonly IStudentRepository _studentRepository;


        public SwipeActionService(ISwipeActionRepository swipeActionRepository, IMatchRepository matchRepository,
            ILogger<SwipeActionService> logger, IStudentRepository studentRepository)
        {
            _swipeActionRepository = swipeActionRepository;
            _matchRepository = matchRepository;
            _logger = logger;
            _studentRepository = studentRepository;
        }

        public async Task<SwipeActionResponseDto> Swipe(long studentId, long tutorId, SwipeActionType action)
        {
            var existingSwipeAction = await _swipeActionRepository.GetSwipeActionAsync(studentId, tutorId);

            // Nếu đã có hành động swipe trước đó, cập nhật hành động mới
            if (existingSwipeAction != null)
            {
                existingSwipeAction.Action = action;
                await _swipeActionRepository.UpdateAsync(existingSwipeAction);
            }
            else
            {
                var newSwipeAction = new SwipeAction
                {
                    StudentId = studentId,
                    TutorId = tutorId,
                    Action = action
                };

                await _swipeActionRepository.CreateAsync(newSwipeAction);
            }

            // Kiểm tra nếu cả hai student và tutor đã like nhau
            if (action == SwipeActionType.Like)
            {
                var tutorSwipe = await _swipeActionRepository.GetSwipeActionAsync(tutorId, studentId);
                if (tutorSwipe != null && tutorSwipe.Action == SwipeActionType.Like)
                {
                    // Trả về thông báo rằng cả hai đã like nhau và tutor có thể chấp nhận hoặc từ chối match
                    return new SwipeActionResponseDto
                    {
                        Success = true,
                        Status = "Both liked, tutor can accept the match"
                    };
                }
            }

            return new SwipeActionResponseDto
            {
                Success = true,
                Status = "Swipe recorded"
            };
        }

        public async Task<SwipeActionResponseDto> AcceptMatch(long tutorId, long studentId)
        {
            // Kiểm tra xem student đã swipe like tutor chưa
            var studentSwipe = await _swipeActionRepository.GetSwipeActionAsync(studentId, tutorId);
            if (studentSwipe == null || studentSwipe.Action != SwipeActionType.Like)
            {
                throw new Exception("Student has not liked the tutor yet.");
            }

            // Tạo match mới ngay khi tutor click accept
            var match = new Match
            {
                StudentId = studentId,
                TutorId = tutorId,
                Status = MatchStatus.Active,
                MatchedAt = DateTime.UtcNow,
                LastActivity = DateTime.UtcNow
            };

            var createdMatch = await _matchRepository.CreateAsync(match);

            return new SwipeActionResponseDto
            {
                Success = true,
                MatchId = createdMatch.Id,
                Status = "Match created"
            };
        }


        public async Task<IEnumerable<StudentResponseDto>> GetLikedStudentsByTutorAsync(long tutorId)
        {
            // Lấy danh sách các studentId đã like tutor
            var likedStudentsIds = await _swipeActionRepository.GetSwipedStudentIdsByTutorAsync(tutorId);

            // Lấy thông tin chi tiết về các student đó
            var students = new List<StudentResponseDto>();
            foreach (var studentId in likedStudentsIds)
            {
                var student = await _studentRepository.GetStudentByUserIdAsync(studentId);
                if (student != null)
                {
                    students.Add(new StudentResponseDto
                    {
                        Id = student.Id,
                        FullName = student.User?.FullName, // Lấy tên đầy đủ từ bảng User
                        ProfileImage = student.User?.ProfileImage, // Lấy ảnh đại diện từ bảng User
                        IsOnline = true // Bạn có thể kiểm tra trạng thái online ở đây nếu sử dụng SignalR hoặc một hệ thống khác
                    });
                }
            }

            return students;
        }


    }
}
