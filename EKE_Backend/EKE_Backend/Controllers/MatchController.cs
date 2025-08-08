using Application.DTOs;
using Application.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Service.DTO.Response;
using Service.Services.Conversations;
using Service.Services.Matches;
using System;
using System.Collections.Generic;
using System.Security.AccessControl;
using System.Threading.Tasks;

namespace WebAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class MatchController : ControllerBase
    {
        private readonly IMatchService _matchService;
        private readonly IConversationService _conversationService;

        public MatchController(IMatchService matchService, IConversationService conversationService)
        {
            _matchService = matchService;
            _conversationService = conversationService;
        }

        /// <summary>
        /// Get all matches
        /// </summary>
        /// <returns>List of all matches</returns>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<MatchResponseDto>>> GetAllMatches()
        {
            try
            {
                var matches = await _matchService.GetAllAsync();
                return Ok(matches);
            }
            catch (Exception ex)
            {
                return BadRequest($"Error retrieving matches: {ex.Message}");
            }
        }
    

        /// <summary>
        /// Get match by ID
        /// </summary>
        /// <param name="id">Match ID</param>
        /// <returns>Match details</returns>
        [HttpGet("{id}")]
        public async Task<ActionResult<MatchResponseDto>> GetMatch(long id)
        {
            try
            {
                var match = await _matchService.GetByIdAsync(id);
                return Ok(match);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                return BadRequest($"Error retrieving match: {ex.Message}");
            }
        }

        /// <summary>
        /// Create a new match
        /// </summary>
        /// <param name="requestDto">Match creation data</param>
        /// <returns>Created match</returns>
        [HttpPost]
        public async Task<ActionResult<MatchResponseDto>> CreateMatch([FromBody] MatchRequestDto requestDto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var match = await _matchService.CreateAsync(requestDto);
                return CreatedAtAction(nameof(GetMatch), new { id = match.Id }, match);
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(ex.Message);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                return BadRequest($"Error creating match: {ex.Message}");
            }
        }

        /// <summary>
        /// Update an existing match
        /// </summary>
        /// <param name="id">Match ID</param>
        /// <param name="requestDto">Match update data</param>
        /// <returns>Updated match</returns>
        [HttpPut("{id}")]
        public async Task<ActionResult<MatchResponseDto>> UpdateMatch(long id, [FromBody] MatchRequestDto requestDto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var match = await _matchService.UpdateAsync(id, requestDto);
                return Ok(match);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                return BadRequest($"Error updating match: {ex.Message}");
            }
        }

        /// <summary>
        /// Delete a match
        /// </summary>
        /// <param name="id">Match ID</param>
        /// <returns>No content if successful</returns>
        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteMatch(long id)
        {
            try
            {
                var result = await _matchService.DeleteAsync(id);
                if (!result)
                    return NotFound($"Match with ID {id} not found");

                return NoContent();
            }
            catch (Exception ex)
            {
                return BadRequest($"Error deleting match: {ex.Message}");
            }
        }

        /// <summary>
        /// Get all matches for a specific student
        /// </summary>
        /// <param name="studentId">Student ID</param>
        /// <returns>List of student's matches</returns>
        [HttpGet("student/{studentId}")]
        public async Task<ActionResult<IEnumerable<MatchResponseDto>>> GetMatchesByStudent(long studentId)
        {
            try
            {
                var matches = await _matchService.GetMatchesByStudentAsync(studentId);
                return Ok(matches);
            }
            catch (Exception ex)
            {
                return BadRequest($"Error retrieving student matches: {ex.Message}");
            }
        }

        /// <summary>
        /// Get all matches for a specific tutor
        /// </summary>
        /// <param name="tutorId">Tutor ID</param>
        /// <returns>List of tutor's matches</returns>
        [HttpGet("tutor/{tutorId}")]
        public async Task<ActionResult<IEnumerable<MatchResponseDto>>> GetMatchesByTutor(long tutorId)
        {
            try
            {
                var matches = await _matchService.GetMatchesByTutorAsync(tutorId);
                return Ok(matches);
            }
            catch (Exception ex)
            {
                return BadRequest($"Error retrieving tutor matches: {ex.Message}");
            }
        }

        /// <summary>
        /// Update the last activity timestamp for a match
        /// </summary>
        /// <param name="id">Match ID</param>
        /// <returns>Success message</returns>
        [HttpPatch("{id}/activity")]
        public async Task<ActionResult> UpdateLastActivity(long id)
        {
            try
            {
                var result = await _matchService.UpdateLastActivityAsync(id);
                if (!result)
                    return NotFound($"Match with ID {id} not found");

                return Ok(new { message = "Last activity updated successfully" });
            }
            catch (Exception ex)
            {
                return BadRequest($"Error updating last activity: {ex.Message}");
            }
        }

        /// <summary>
        /// Check if an active match exists between student and tutor
        /// </summary>
        /// <param name="studentId">Student ID</param>
        /// <param name="tutorId">Tutor ID</param>
        /// <returns>Boolean indicating if active match exists</returns>
        [HttpGet("check/{studentId}/{tutorId}")]
        public async Task<ActionResult<bool>> CheckActiveMatch(long studentId, long tutorId)
        {
            try
            {
                // This would require adding the method to IMatchService
                // For now, we can get matches and check on the client side
                var studentMatches = await _matchService.GetMatchesByStudentAsync(studentId);
                var hasActiveMatch = studentMatches.Any(m => m.TutorId == tutorId && m.Status == Repository.Enums.MatchStatus.Active);

                return Ok(new { hasActiveMatch });
            }
            catch (Exception ex)
            {
                return BadRequest($"Error checking active match: {ex.Message}");
            }
        }

        /// <summary>
        /// Get match statistics (optional - for dashboard)
        /// </summary>
        /// <returns>Match statistics</returns>
        [HttpGet("statistics")]
        public async Task<ActionResult> GetMatchStatistics()
        {
            try
            {
                var allMatches = await _matchService.GetAllAsync();
                var statistics = new
                {
                    TotalMatches = allMatches.Count(),
                    ActiveMatches = allMatches.Count(m => m.Status == Repository.Enums.MatchStatus.Active),
                    InactiveMatches = allMatches.Count(m => m.Status != Repository.Enums.MatchStatus.Active),
                    MatchesToday = allMatches.Count(m => m.MatchedAt.Date == DateTime.UtcNow.Date),
                    RecentActivity = allMatches.Count(m => m.LastActivity > DateTime.UtcNow.AddDays(-7))
                };

                return Ok(statistics);
            }
            catch (Exception ex)
            {
                return BadRequest($"Error retrieving statistics: {ex.Message}");
            }
        }
    }
}