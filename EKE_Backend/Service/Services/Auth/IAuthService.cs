using Service.DTO.Request;
using Service.DTO.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.Services.Auth
{
    public interface IAuthService
    {
        // Login (common for both roles)
        Task<LoginResponseDto?> LoginAsync(UserLoginDto loginDto);

        // Separate sign up methods
        Task<UserResponseDto> RegisterStudentAsync(StudentSignUpDto studentSignUpDto);
        Task<UserResponseDto> RegisterTutorAsync(TutorSignUpDto tutorSignUpDto);

        // Common methods
        Task<LoginResponseDto?> RefreshTokenAsync(string refreshToken);
        Task<bool> LogoutAsync(string refreshToken);
        Task<bool> EmailExistsAsync(string email);
    }
}
