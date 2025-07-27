using AutoMapper;
using Repository.UnitOfWork;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
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
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ILogger<JwtService> _logger;

        public JwtService(
            IConfiguration configuration,
            IUnitOfWork unitOfWork,
            IMapper mapper,
            ILogger<JwtService> logger)
        {
            _configuration = configuration;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _logger = logger;
        }

        // Configuration Properties
        public int AccessTokenExpiryMinutes => Convert.ToInt32(_configuration["Jwt:AccessTokenExpiryMinutes"] ?? "15");
        public int RefreshTokenExpiryDays => Convert.ToInt32(_configuration["Jwt:RefreshTokenExpiryDays"] ?? "7");

        #region Token Generation

        public string GenerateAccessToken(User user)
        {
            try
            {
                var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]!));
                var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                    new Claim(ClaimTypes.Name, user.FullName),
                    new Claim(ClaimTypes.Email, user.Email),
                    new Claim(ClaimTypes.Role, user.Role.ToString()),
                    new Claim("UserId", user.Id.ToString()),
                    new Claim("Role", user.Role.ToString()),
                    new Claim("IsVerified", user.IsVerified.ToString()),
                    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                    new Claim(JwtRegisteredClaimNames.Iat, DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString(), ClaimValueTypes.Integer64)
                };

                // Add role-specific claims
                if (user.Student != null)
                {
                    claims.Add(new Claim("StudentId", user.Student.Id.ToString()));
                }

                if (user.Tutor != null)
                {
                    claims.Add(new Claim("TutorId", user.Tutor.Id.ToString()));
                    claims.Add(new Claim("TutorVerificationStatus", user.Tutor.VerificationStatus.ToString()));
                }

                var token = new JwtSecurityToken(
                    issuer: _configuration["Jwt:Issuer"],
                    audience: _configuration["Jwt:Audience"],
                    claims: claims,
                    expires: DateTime.UtcNow.AddMinutes(AccessTokenExpiryMinutes),
                    signingCredentials: credentials
                );

                return new JwtSecurityTokenHandler().WriteToken(token);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error generating access token for user: {UserId}", user.Id);
                throw;
            }
        }

        public string GenerateRefreshToken()
        {
            try
            {
                var randomNumber = new byte[64];
                using var rng = RandomNumberGenerator.Create();
                rng.GetBytes(randomNumber);
                return Convert.ToBase64String(randomNumber);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error generating refresh token");
                throw;
            }
        }

        #endregion

        #region Token Validation

        public bool ValidateToken(string token)
        {
            try
            {
                var principal = GetPrincipalFromToken(token);
                return principal != null;
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Token validation failed");
                return false;
            }
        }

        public ClaimsPrincipal? GetPrincipalFromToken(string token)
        {
            try
            {
                var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]!));
                var tokenHandler = new JwtSecurityTokenHandler();

                var principal = tokenHandler.ValidateToken(token, new TokenValidationParameters
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

                return principal;
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Failed to get principal from token");
                return null;
            }
        }

        public ClaimsPrincipal? GetPrincipalFromExpiredToken(string token)
        {
            try
            {
                var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]!));
                var tokenHandler = new JwtSecurityTokenHandler();

                var principal = tokenHandler.ValidateToken(token, new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = false, // Don't validate lifetime for expired tokens
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = _configuration["Jwt:Issuer"],
                    ValidAudience = _configuration["Jwt:Audience"],
                    IssuerSigningKey = key,
                    ClockSkew = TimeSpan.Zero
                }, out SecurityToken validatedToken);

                return principal;
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Failed to get principal from expired token");
                return null;
            }
        }

        #endregion

        #region Token Information Extraction

        public long? GetUserIdFromToken(string token)
        {
            try
            {
                var tokenHandler = new JwtSecurityTokenHandler();
                var jwt = tokenHandler.ReadJwtToken(token);

                var userIdClaim = jwt.Claims.FirstOrDefault(x => x.Type == "UserId");
                return userIdClaim != null ? long.Parse(userIdClaim.Value) : null;
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Failed to get user ID from token");
                return null;
            }
        }

        public string? GetUserRoleFromToken(string token)
        {
            try
            {
                var tokenHandler = new JwtSecurityTokenHandler();
                var jwt = tokenHandler.ReadJwtToken(token);

                var roleClaim = jwt.Claims.FirstOrDefault(x => x.Type == "Role");
                return roleClaim?.Value;
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Failed to get user role from token");
                return null;
            }
        }

        public string? GetUserEmailFromToken(string token)
        {
            try
            {
                var tokenHandler = new JwtSecurityTokenHandler();
                var jwt = tokenHandler.ReadJwtToken(token);

                var emailClaim = jwt.Claims.FirstOrDefault(x => x.Type == ClaimTypes.Email);
                return emailClaim?.Value;
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Failed to get user email from token");
                return null;
            }
        }

        public DateTime? GetTokenExpiryDate(string token)
        {
            try
            {
                var tokenHandler = new JwtSecurityTokenHandler();
                var jwt = tokenHandler.ReadJwtToken(token);

                return jwt.ValidTo;
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Failed to get token expiry date");
                return null;
            }
        }

        #endregion

        #region Refresh Token Operations

        public async Task<LoginResponseDto?> RefreshTokenAsync(string refreshToken)
        {
            try
            {
                // Validate refresh token format
                if (string.IsNullOrWhiteSpace(refreshToken))
                {
                    return null;
                }

                // Check if refresh token exists and is valid
                if (!await IsRefreshTokenValidAsync(refreshToken))
                {
                    return null;
                }

                // Extract user information from refresh token
                // In a real implementation, you would store refresh tokens in database
                // with user association and expiry dates

                // For now, we'll extract from the associated access token or user context
                // This is a simplified implementation - in production, store refresh tokens properly

                _logger.LogWarning("Refresh token functionality requires database storage implementation");
                return null;

                // TODO: Implement proper refresh token storage and validation
                /*
                var storedRefreshToken = await _unitOfWork.RefreshTokens.GetByTokenAsync(refreshToken);
                if (storedRefreshToken == null || storedRefreshToken.IsExpired || storedRefreshToken.IsRevoked)
                {
                    return null;
                }

                var user = await _unitOfWork.Users.GetByIdAsync(storedRefreshToken.UserId);
                if (user == null || !user.IsActive)
                {
                    return null;
                }

                // Generate new tokens
                var newAccessToken = GenerateAccessToken(user);
                var newRefreshToken = GenerateRefreshToken();

                // Update refresh token in database
                storedRefreshToken.Token = newRefreshToken;
                storedRefreshToken.ExpiresAt = DateTime.UtcNow.AddDays(RefreshTokenExpiryDays);
                
                await _unitOfWork.CompleteAsync();

                return new LoginResponseDto
                {
                    AccessToken = newAccessToken,
                    RefreshToken = newRefreshToken,
                    ExpiresAt = DateTime.UtcNow.AddMinutes(AccessTokenExpiryMinutes),
                    User = _mapper.Map<UserResponseDto>(user)
                };
                */
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error refreshing token");
                return null;
            }
        }

        public async Task<bool> RevokeRefreshTokenAsync(string refreshToken)
        {
            try
            {
                // TODO: Implement refresh token revocation
                /*
                var storedRefreshToken = await _unitOfWork.RefreshTokens.GetByTokenAsync(refreshToken);
                if (storedRefreshToken == null)
                {
                    return false;
                }

                storedRefreshToken.IsRevoked = true;
                storedRefreshToken.RevokedAt = DateTime.UtcNow;
                
                await _unitOfWork.CompleteAsync();
                return true;
                */

                _logger.LogWarning("Refresh token revocation requires database storage implementation");
                return false;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error revoking refresh token");
                return false;
            }
        }

        public async Task<bool> IsRefreshTokenValidAsync(string refreshToken)
        {
            try
            {
                // TODO: Implement refresh token validation
                /*
                var storedRefreshToken = await _unitOfWork.RefreshTokens.GetByTokenAsync(refreshToken);
                return storedRefreshToken != null && 
                       !storedRefreshToken.IsExpired && 
                       !storedRefreshToken.IsRevoked;
                */

                _logger.LogWarning("Refresh token validation requires database storage implementation");
                return false;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error validating refresh token");
                return false;
            }
        }

        #endregion
    }
}
