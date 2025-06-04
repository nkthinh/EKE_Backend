using AutoMapper;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Repository.Entities;
using Repository.Repositories.Users;
using Service.DTO.Response;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Service.Services.Jwt
{
    public class JwtService : IJwtService
    {
        private readonly IConfiguration _configuration;
        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;

        public JwtService(IConfiguration configuration, IUserRepository userRepository, IMapper mapper)
        {
            _configuration = configuration;
            _userRepository = userRepository;
            _mapper = mapper;
        }

        public string GenerateAccessToken(User user)
        {
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]!));
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, user.UserId.ToString()),
                new Claim(ClaimTypes.Name, user.FullName),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.Role, user.Role.ToString()),
                new Claim("UserId", user.UserId.ToString()),
                new Claim("Role", user.Role.ToString())
            };

            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(Convert.ToDouble(_configuration["Jwt:DurationInMinutes"] ?? "60")),
                signingCredentials: credentials
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public string GenerateRefreshToken()
        {
            var randomNumber = new byte[32];
            using var rng = RandomNumberGenerator.Create();
            rng.GetBytes(randomNumber);
            return Convert.ToBase64String(randomNumber);
        }

        public bool ValidateToken(string token)
        {
            try
            {
                var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]!));
                var tokenHandler = new JwtSecurityTokenHandler();

                tokenHandler.ValidateToken(token, new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = _configuration["Jwt:Issuer"],
                    ValidAudience = _configuration["Jwt:Audience"],
                    IssuerSigningKey = key,
                    ClockSkew = TimeSpan.Zero
                }, out SecurityToken validatedToken);

                return true;
            }
            catch
            {
                return false;
            }
        }

        public int? GetUserIdFromToken(string token)
        {
            try
            {
                var tokenHandler = new JwtSecurityTokenHandler();
                var jwt = tokenHandler.ReadJwtToken(token);

                var userIdClaim = jwt.Claims.FirstOrDefault(x => x.Type == "UserId");
                return userIdClaim != null ? int.Parse(userIdClaim.Value) : null;
            }
            catch
            {
                return null;
            }
        }

        public async Task<LoginResponseDto> RefreshTokenAsync(string refreshToken)
        {
            // Trong thực tế, bạn cần lưu refresh token vào database và validate
            // Đây là version đơn giản, bạn có thể mở rộng thêm
            throw new NotImplementedException("Refresh token functionality needs to be implemented with database storage");
        }
    }
}
