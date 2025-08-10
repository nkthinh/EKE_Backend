using AutoMapper;
using Microsoft.Extensions.Logging;
using Repository.Entities;
using Repository.Enums;
using Repository.UnitOfWork;
using Service.DTO.Request;
using Service.DTO.Response;
using Service.Services.Jwt;

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

        // STEP 1: Register Account
        public async Task<RegistrationStepResponseDto> RegisterAccountAsync(AccountRegistrationDto accountDto)
        {
            try
            {
                // Check if email exists
                if (await _unitOfWork.Users.EmailExistsAsync(accountDto.Email))
                {
                    throw new InvalidOperationException("Email đã được sử dụng");
                }

                // Create basic user account
                var user = new User
                {
                    Email = accountDto.Email,
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword(accountDto.Password),
                    FullName = accountDto.FullName,
                    Role = UserRole.Unspecified,
                    IsActive = true,
                    SubscriptionPackageId = 1, // ✅ Gán gói mặc định ID=1
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                };

                await _unitOfWork.Users.AddAsync(user);
                await _unitOfWork.CompleteAsync();

                // Tạo ví mặc định
                var wallet = new Wallet
                {
                    UserId = user.Id,
                    Balance = 0,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                };
                await _unitOfWork.Wallets.AddAsync(wallet);
                await _unitOfWork.CompleteAsync();

                // Generate tokens
                var accessToken = _jwtService.GenerateAccessToken(user);
                var refreshToken = _jwtService.GenerateRefreshToken();

                return new RegistrationStepResponseDto
                {
                    AccessToken = accessToken,
                    RefreshToken = refreshToken,
                    ExpiresAt = DateTime.UtcNow.AddMinutes(_jwtService.AccessTokenExpiryMinutes),
                    User = _mapper.Map<UserResponseDto>(user),
                    NextStep = "SelectRole",
                    IsCompleted = false
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error registering account: {Email}", accountDto.Email);
                throw;
            }
        }


        // STEP 2: Select Role
        public async Task<RegistrationStepResponseDto> SelectRoleAsync(long userId, RoleSelectionDto roleDto)
        {
            try
            {
                var user = await _unitOfWork.Users.GetByIdAsync(userId);
                if (user == null)
                {
                    throw new InvalidOperationException("Người dùng không tồn tại");
                }

                if (user.Role != UserRole.Unspecified)
                {
                    throw new InvalidOperationException("Người dùng đã chọn vai trò");
                }

                // Validate and set role
                if (!Enum.TryParse<UserRole>(roleDto.Role, out var role) || role == UserRole.Unspecified)
                {
                    throw new InvalidOperationException("Vai trò không hợp lệ");
                }


                user.Role = role;
                user.UpdatedAt = DateTime.UtcNow;

                _unitOfWork.Users.Update(user);
                await _unitOfWork.CompleteAsync();

                // Generate new tokens with updated role
                var accessToken = _jwtService.GenerateAccessToken(user);
                var refreshToken = _jwtService.GenerateRefreshToken();

                return new RegistrationStepResponseDto
                {
                    AccessToken = accessToken,
                    RefreshToken = refreshToken,
                    ExpiresAt = DateTime.UtcNow.AddMinutes(_jwtService.AccessTokenExpiryMinutes),
                    User = _mapper.Map<UserResponseDto>(user),
                    NextStep = "CompleteProfile",
                    IsCompleted = false
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error selecting role for user: {UserId}", userId);
                throw;
            }
        }

        // STEP 3: Complete Profile
        public async Task<RegistrationStepResponseDto> CompleteProfileAsync(long userId, ProfileCompletionDto profileDto)
        {
            await _unitOfWork.BeginTransactionAsync();
            try
            {
                var user = await _unitOfWork.Users.GetByIdAsync(userId);
                if (user == null)
                {
                    throw new InvalidOperationException("Người dùng không tồn tại");
                }

                if (user.Role == UserRole.Unspecified)
                {
                    throw new InvalidOperationException("Người dùng chưa chọn vai trò");
                }

                // Update user basic info
                user.Phone = profileDto.Phone;
                user.DateOfBirth = profileDto.DateOfBirth;
                user.UpdatedAt = DateTime.UtcNow;

                _unitOfWork.Users.Update(user);
                await _unitOfWork.CompleteAsync();

                // Create role-specific profile
                if (user.Role == UserRole.Student)
                {
                    await CreateStudentProfile(user.Id, profileDto);
                }
                else if (user.Role == UserRole.Tutor)
                {
                    await CreateTutorProfile(user.Id, profileDto);
                }

                await _unitOfWork.CommitTransactionAsync();

                // Load user with complete profile - giả sử có method này
                var userWithProfile = user; // Có thể cần reload từ DB nếu có navigation properties

                // Generate final tokens
                var accessToken = _jwtService.GenerateAccessToken(userWithProfile);
                var refreshToken = _jwtService.GenerateRefreshToken();

                return new RegistrationStepResponseDto
                {
                    AccessToken = accessToken,
                    RefreshToken = refreshToken,
                    ExpiresAt = DateTime.UtcNow.AddMinutes(_jwtService.AccessTokenExpiryMinutes),
                    User = _mapper.Map<UserResponseDto>(userWithProfile),
                    NextStep = "Completed",
                    IsCompleted = true
                };
            }
            catch (Exception ex)
            {
                await _unitOfWork.RollbackTransactionAsync();
                _logger.LogError(ex, "Error completing profile for user: {UserId}", userId);
                throw;
            }
        }

        private async Task CreateStudentProfile(long userId, ProfileCompletionDto profileDto)
        {
            var student = new Student
            {
                UserId = userId,
                SchoolName = profileDto.SchoolName,
                GradeLevel = profileDto.GradeLevel,
                LearningGoals = profileDto.LearningGoals,
                BudgetMin = profileDto.BudgetMin,
                BudgetMax = profileDto.BudgetMax,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            await _unitOfWork.Students.AddAsync(student);
            await _unitOfWork.CompleteAsync();
        }

        private async Task CreateTutorProfile(long userId, ProfileCompletionDto profileDto)
        {
            var tutor = new Tutor
            {
                UserId = userId,
                EducationLevel = profileDto.EducationLevel ?? "",
                University = profileDto.University,
                Major = profileDto.Major,
                ExperienceYears = profileDto.ExperienceYears ?? 0,
                HourlyRate = profileDto.HourlyRate,
                Introduction = profileDto.Introduction,
                VerificationStatus = VerificationStatus.Pending,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            await _unitOfWork.Tutors.AddAsync(tutor);
            await _unitOfWork.CompleteAsync();

            // Add tutor subjects if provided
            if (profileDto.SubjectIds != null && profileDto.SubjectIds.Any())
            {
                var tutorSubjects = profileDto.SubjectIds.Select(subjectId => new TutorSubject
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
        }

        // Existing methods - sửa lại cho phù hợp với UnitOfWork
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
                if (await _unitOfWork.Users.EmailExistsAsync(studentSignUpDto.Email))
                {
                    throw new InvalidOperationException("Email đã được sử dụng");
                }

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

                return _mapper.Map<UserResponseDto>(user);
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
                if (await _unitOfWork.Users.EmailExistsAsync(tutorSignUpDto.Email))
                {
                    throw new InvalidOperationException("Email đã được sử dụng");
                }

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

                if (tutorSignUpDto.SubjectIds != null && tutorSignUpDto.SubjectIds.Any())
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

                return _mapper.Map<UserResponseDto>(user);
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