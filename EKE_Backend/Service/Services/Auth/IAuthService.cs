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
        // Existing methods
        Task<LoginResponseDto?> LoginAsync(UserLoginDto loginDto);
        Task<UserResponseDto> RegisterStudentAsync(StudentSignUpDto studentSignUpDto);
        Task<UserResponseDto> RegisterTutorAsync(TutorSignUpDto tutorSignUpDto);
        Task<LoginResponseDto?> RefreshTokenAsync(string refreshToken);
        Task<bool> LogoutAsync(string refreshToken);
        Task<bool> EmailExistsAsync(string email);

        // New multi-step registration methods
        Task<RegistrationStepResponseDto> RegisterAccountAsync(AccountRegistrationDto accountDto);
        Task<RegistrationStepResponseDto> SelectRoleAsync(long userId, RoleSelectionDto roleDto);
        Task<RegistrationStepResponseDto> CompleteProfileAsync(long userId, ProfileCompletionDto profileDto);
    }
}
