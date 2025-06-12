using AutoMapper;
using Microsoft.Extensions.Logging;
using Repository.Entities;
using Repository.Enums;
using Repository.UnitOfWork;
using Service.DTO.Request;
using Service.DTO.Response;
using Service.Services.Jwt;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.Services.Auth
{
    public class AuthService : IAuthService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IJwtService _jwtService;
        private readonly ILogger<AuthService> _logger;

        public AuthService(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            IJwtService jwtService,
            ILogger<AuthService> logger)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _jwtService = jwtService;
            _logger = logger;
        }

        public async Task<LoginResponseDto?> LoginAsync(UserLoginDto loginDto)
        {
            try
            {
                var user = await _unitOfWork.Users.GetByEmailAsync(loginDto.Email);
                if (user == null || !user.IsActive || !BCrypt.Net.BCrypt.Verify(loginDto.Password, user.PasswordHash))
                {
                    return null;
                }

                var accessToken = _jwtService.GenerateAccessToken(user);
                var refreshToken = _jwtService.GenerateRefreshToken();

                return new LoginResponseDto
                {
                    AccessToken = accessToken,
                    RefreshToken = refreshToken,
                    ExpiresAt = DateTime.UtcNow.AddMinutes(_jwtService.AccessTokenExpiryMinutes),
                    User = _mapper.Map<UserResponseDto>(user)
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during login for email: {Email}", loginDto.Email);
                throw;
            }
        }

        public async Task<UserResponseDto> RegisterStudentAsync(StudentSignUpDto studentSignUpDto)
        {
            await _unitOfWork.BeginTransactionAsync();
            try
            {
                // Check if email exists
                if (await _unitOfWork.Users.EmailExistsAsync(studentSignUpDto.Email))
                {
                    throw new InvalidOperationException("Email đã được sử dụng");
                }

                // Create User entity
                var user = new User
                {
                    Email = studentSignUpDto.Email,
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword(studentSignUpDto.Password),
                    FullName = studentSignUpDto.FullName,
                    Phone = studentSignUpDto.Phone,
                    DateOfBirth = studentSignUpDto.DateOfBirth,
                    Role = UserRole.Student,
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                };

                await _unitOfWork.Users.AddAsync(user);
                await _unitOfWork.CompleteAsync();

                // Create Student profile
                var student = new Student
                {
                    UserId = user.Id,
                    SchoolName = studentSignUpDto.SchoolName,
                    GradeLevel = studentSignUpDto.GradeLevel,
                    LearningGoals = studentSignUpDto.LearningGoals,
                    BudgetMin = studentSignUpDto.BudgetMin,
                    BudgetMax = studentSignUpDto.BudgetMax,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                };

                await _unitOfWork.Students.AddAsync(student);
                await _unitOfWork.CompleteAsync();

                await _unitOfWork.CommitTransactionAsync();

                // Load user with student profile
                var userWithProfile = await _unitOfWork.Users.GetUserWithDetailsAsync(user.Id);
                return _mapper.Map<UserResponseDto>(userWithProfile);
            }
            catch (Exception ex)
            {
                await _unitOfWork.RollbackTransactionAsync();
                _logger.LogError(ex, "Error registering student: {Email}", studentSignUpDto.Email);
                throw;
            }
        }

        public async Task<UserResponseDto> RegisterTutorAsync(TutorSignUpDto tutorSignUpDto)
        {
            await _unitOfWork.BeginTransactionAsync();
            try
            {
                // Check if email exists
                if (await _unitOfWork.Users.EmailExistsAsync(tutorSignUpDto.Email))
                {
                    throw new InvalidOperationException("Email đã được sử dụng");
                }

                // Create User entity
                var user = new User
                {
                    Email = tutorSignUpDto.Email,
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword(tutorSignUpDto.Password),
                    FullName = tutorSignUpDto.FullName,
                    Phone = tutorSignUpDto.Phone,
                    DateOfBirth = tutorSignUpDto.DateOfBirth,
                    Role = UserRole.Tutor,
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                };

                await _unitOfWork.Users.AddAsync(user);
                await _unitOfWork.CompleteAsync();

                // Create Tutor profile
                var tutor = new Tutor
                {
                    UserId = user.Id,
                    EducationLevel = tutorSignUpDto.EducationLevel,
                    University = tutorSignUpDto.University,
                    Major = tutorSignUpDto.Major,
                    ExperienceYears = tutorSignUpDto.ExperienceYears,
                    HourlyRate = tutorSignUpDto.HourlyRate,
                    Introduction = tutorSignUpDto.Introduction,
                    VerificationStatus = VerificationStatus.Pending,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                };

                await _unitOfWork.Tutors.AddAsync(tutor);
                await _unitOfWork.CompleteAsync();

                // Add tutor subjects if provided
                if (tutorSignUpDto.SubjectIds.Any())
                {
                    var tutorSubjects = tutorSignUpDto.SubjectIds.Select(subjectId => new TutorSubject
                    {
                        TutorId = tutor.Id,
                        SubjectId = subjectId,
                        ProficiencyLevel = ProficiencyLevel.Intermediate,
                        YearsExperience = 0,
                        CreatedAt = DateTime.UtcNow,
                        UpdatedAt = DateTime.UtcNow
                    });

                    await _unitOfWork.TutorSubjects.AddRangeAsync(tutorSubjects);
                    await _unitOfWork.CompleteAsync();
                }

                await _unitOfWork.CommitTransactionAsync();

                // Load user with tutor profile
                var userWithProfile = await _unitOfWork.Users.GetUserWithDetailsAsync(user.Id);
                return _mapper.Map<UserResponseDto>(userWithProfile);
            }
            catch (Exception ex)
            {
                await _unitOfWork.RollbackTransactionAsync();
                _logger.LogError(ex, "Error registering tutor: {Email}", tutorSignUpDto.Email);
                throw;
            }
        }

        public async Task<LoginResponseDto?> RefreshTokenAsync(string refreshToken)
        {
            return await _jwtService.RefreshTokenAsync(refreshToken);
        }

        public async Task<bool> LogoutAsync(string refreshToken)
        {
            return await _jwtService.RevokeRefreshTokenAsync(refreshToken);
        }

        public async Task<bool> EmailExistsAsync(string email)
        {
            return await _unitOfWork.Users.EmailExistsAsync(email);
        }
    }
}
