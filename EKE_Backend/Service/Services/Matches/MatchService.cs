using Application.DTOs;
using Application.Services.Interfaces;
using Microsoft.Extensions.Logging;
using Repository.Entities;
using Repository.Enums;
using Repository.Repositories;
using Repository.Repositories.Matches;
using Repository.Repositories.Students;
using Repository.Repositories.Tutors;
using Service.DTO.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Service.Services.Matches
{
    public class MatchService : IMatchService
    {
        private readonly IMatchRepository _matchRepository;
        private readonly IStudentRepository _studentRepository;
        private readonly ITutorRepository _tutorRepository;
        private readonly ILogger<MatchService> _logger;

        public MatchService(IMatchRepository matchRepository,IStudentRepository studentRepository, ITutorRepository tutorRepository, ILogger<MatchService> logger)
        {
            _matchRepository = matchRepository;
            _studentRepository = studentRepository;
            _tutorRepository = tutorRepository;
            _logger = logger;

        }

        public async Task<MatchResponseDto> GetByIdAsync(long id)
        {
            var match = await _matchRepository.GetMatchWithDetailsAsync(id);
            if (match == null)
                throw new KeyNotFoundException($"Match with ID {id} not found");

            return MapToResponseDto(match);
        }

        public async Task<IEnumerable<MatchResponseDto>> GetAllAsync()
        {
            var matches = await _matchRepository.GetMatchesWithDetailsAsync();
            return matches.Select(MapToResponseDto);
        }

        public async Task<MatchResponseDto> CreateAsync(MatchRequestDto requestDto)
        {
            try
            {
                // Kiểm tra nếu đã có match active
                _logger.LogInformation("Checking if active match exists for student {StudentId} and tutor {TutorId}", requestDto.StudentId, requestDto.TutorId);
                var hasActiveMatch = await _matchRepository.HasActiveMatchAsync(requestDto.StudentId, requestDto.TutorId);
                if (hasActiveMatch)
                {
                    _logger.LogWarning("Active match already exists between student {StudentId} and tutor {TutorId}", requestDto.StudentId, requestDto.TutorId);
                    throw new InvalidOperationException("Active match already exists between this student and tutor");
                }

                // Kiểm tra sự tồn tại của Student và Tutor
                _logger.LogInformation("Checking existence of student {StudentId} and tutor {TutorId}", requestDto.StudentId, requestDto.TutorId);
                var student = await _studentRepository.GetStudentWithUserInfoAsync(requestDto.StudentId);
                var tutor = await _tutorRepository.GetTutotWithUserInfoAsync(requestDto.TutorId);

                if (student == null)
                {
                    _logger.LogError("Student with ID {StudentId} not found", requestDto.StudentId);
                    throw new KeyNotFoundException($"Student with ID {requestDto.StudentId} not found");
                }
                if (tutor == null)
                {
                    _logger.LogError("Tutor with ID {TutorId} not found", requestDto.TutorId);
                    throw new KeyNotFoundException($"Tutor with ID {requestDto.TutorId} not found");
                }

                // Tạo match mới
                _logger.LogInformation("Creating new match for student {StudentId} and tutor {TutorId}", requestDto.StudentId, requestDto.TutorId);
                var match = new Match
                {
                    StudentId = requestDto.StudentId,
                    TutorId = requestDto.TutorId,
                    Status = requestDto.Status ?? MatchStatus.Active,
                    MatchedAt = DateTime.UtcNow,
                    LastActivity = DateTime.UtcNow
                };

                // Lưu match vào cơ sở dữ liệu
                _logger.LogInformation("Saving new match to the database");
                var createdMatch = await _matchRepository.CreateAsync(match);

                // Lấy thông tin match vừa tạo với chi tiết
                _logger.LogInformation("Fetching detailed match for match ID {MatchId}", createdMatch.Id);
                var detailedMatch = await _matchRepository.GetMatchWithDetailsAsync(createdMatch.Id);

                _logger.LogInformation("Match created successfully for student {StudentId} and tutor {TutorId}", requestDto.StudentId, requestDto.TutorId);

                return MapToResponseDto(detailedMatch);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating match for student {StudentId} and tutor {TutorId}", requestDto.StudentId, requestDto.TutorId);
                throw;  // Rethrow the exception after logging it
            }
        }



        public async Task<MatchResponseDto> UpdateAsync(long id, MatchRequestDto requestDto)
        {
            var match = await _matchRepository.GetByIdAsync(id);
            if (match == null)
                throw new KeyNotFoundException($"Match with ID {id} not found");

            match.StudentId = requestDto.StudentId;
            match.TutorId = requestDto.TutorId;
            if (requestDto.Status.HasValue)
                match.Status = requestDto.Status.Value;
            match.LastActivity = DateTime.UtcNow;

            await _matchRepository.UpdateAsync(match);
            var updatedMatch = await _matchRepository.GetMatchWithDetailsAsync(id);
            return MapToResponseDto(updatedMatch);
        }

        public async Task<bool> DeleteAsync(long id)
        {
            return await _matchRepository.DeleteAsync(id);
        }

        public async Task<IEnumerable<MatchResponseDto>> GetMatchesByStudentAsync(long studentId)
        {
            var matches = await _matchRepository.GetMatchesByStudentIdAsync(studentId);
            return matches.Select(MapToResponseDto);
        }

        public async Task<IEnumerable<MatchResponseDto>> GetMatchesByTutorAsync(long tutorId)
        {
            var matches = await _matchRepository.GetMatchesByTutorIdAsync(tutorId);
            return matches.Select(MapToResponseDto);
        }

        public async Task<bool> UpdateLastActivityAsync(long id)
        {
            return await _matchRepository.UpdateLastActivityAsync(id);
        }

        private MatchResponseDto MapToResponseDto(Match match)
        {
            return new MatchResponseDto
            {
                Id = match.Id,
                StudentId = match.StudentId,
                TutorId = match.TutorId,
                Status = match.Status,
                MatchedAt = match.MatchedAt,
                LastActivity = match.LastActivity,
                StudentName = match.Student?.User?.FullName, // Using your existing User structure
                TutorName = match.Tutor?.User?.FullName, // Using your existing User structure
                ConversationCount = match.Conversations?.Count ?? 0
            };
        }
        public async Task<bool> CheckActiveMatch(long studentId, long tutorId)
        {
            try
            {
                var studentMatches = await _matchRepository.GetMatchesByStudentIdAsync(studentId);
                return studentMatches.Any(m => m.TutorId == tutorId && m.Status == MatchStatus.Active);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error checking active match between student {StudentId} and tutor {TutorId}", studentId, tutorId);
                throw;  // Rethrow the exception
            }
        }

    }
}