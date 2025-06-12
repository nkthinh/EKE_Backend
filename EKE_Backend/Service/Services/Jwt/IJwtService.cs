using Repository.Entities;
using Service.DTO.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Service.Services.Jwt
{
    public interface IJwtService
    {
        // Token Generation
        string GenerateAccessToken(User user);
        string GenerateRefreshToken();

        // Token Validation
        bool ValidateToken(string token);
        ClaimsPrincipal? GetPrincipalFromExpiredToken(string token);
        ClaimsPrincipal? GetPrincipalFromToken(string token);

        // Token Information Extraction
        long? GetUserIdFromToken(string token);
        string? GetUserRoleFromToken(string token);
        string? GetUserEmailFromToken(string token);
        DateTime? GetTokenExpiryDate(string token);

        // Refresh Token Operations
        Task<LoginResponseDto?> RefreshTokenAsync(string refreshToken);
        Task<bool> RevokeRefreshTokenAsync(string refreshToken);
        Task<bool> IsRefreshTokenValidAsync(string refreshToken);

        // Configuration Properties
        int AccessTokenExpiryMinutes { get; }
        int RefreshTokenExpiryDays { get; }
    }
}
