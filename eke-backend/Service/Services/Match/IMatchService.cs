using Application.DTOs;
using Repository.Entities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Application.Services.Interfaces
{
    public interface IMatchService
    {
        Task<MatchResponseDto> GetByIdAsync(long id);
        Task<IEnumerable<MatchResponseDto>> GetAllAsync();
        Task<MatchResponseDto> CreateAsync(MatchRequestDto requestDto);
        Task<MatchResponseDto> UpdateAsync(long id, MatchRequestDto requestDto);
        Task<bool> DeleteAsync(long id);
        Task<IEnumerable<MatchResponseDto>> GetMatchesByStudentAsync(long studentId);
        Task<IEnumerable<MatchResponseDto>> GetMatchesByTutorAsync(long tutorId);
        Task<bool> UpdateLastActivityAsync(long id);
    }
}