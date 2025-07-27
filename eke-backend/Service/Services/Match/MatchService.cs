using Application.DTOs;
using Application.Services.Interfaces;
using Repository.Entities;
using Repository.Enums;
using Repository.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Application.Services
{
    public class MatchService : IMatchService
    {
        private readonly IMatchRepository _matchRepository;

        public MatchService(IMatchRepository matchRepository)
        {
            _matchRepository = matchRepository;
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
            // Check if active match already exists
            var hasActiveMatch = await _matchRepository.HasActiveMatchAsync(requestDto.StudentId, requestDto.TutorId);
            if (hasActiveMatch)
                throw new InvalidOperationException("Active match already exists between this student and tutor");

            var match = new Match
            {
                StudentId = requestDto.StudentId,
                TutorId = requestDto.TutorId,
                Status = requestDto.Status ?? MatchStatus.Active,
                MatchedAt = DateTime.UtcNow,
                LastActivity = DateTime.UtcNow
            };

            var createdMatch = await _matchRepository.CreateAsync(match);
            var detailedMatch = await _matchRepository.GetMatchWithDetailsAsync(createdMatch.Id);
            return MapToResponseDto(detailedMatch);
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
    }
}