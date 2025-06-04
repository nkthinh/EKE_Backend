using Repository.Entities;
using Service.DTO.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.Services.Jwt
{
    public interface IJwtService
    {
        string GenerateAccessToken(User user);
        string GenerateRefreshToken();
        bool ValidateToken(string token);
        int? GetUserIdFromToken(string token);
        Task<LoginResponseDto> RefreshTokenAsync(string refreshToken);
    }
}
