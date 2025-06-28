using AutoMapper;
using Repository.Enums;
using Repository.UnitOfWork;
using Microsoft.Extensions.Logging;
using Repository.Entities;
using Repository.Repositories.Users;
using Repository.UnitOfWork;
using Service.DTO.Request;
using Service.DTO.Response;
using Service.Services.Jwt;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Service.Firebase;

namespace Service.Services.Users
{
    public class UserService : IUserService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IJwtService _jwtService;
        private readonly ILogger<UserService> _logger;
        private readonly IFirebaseStorageService _firebaseStorageService; 
        
        public UserService(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            IJwtService jwtService,
            ILogger<UserService> logger,
            IFirebaseStorageService firebaseStorageService)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _jwtService = jwtService;
            _logger = logger;
            _firebaseStorageService = firebaseStorageService;
        }

        #region Basic CRUD Operations

        public async Task<UserResponseDto?> GetUserByIdAsync(long id)
        {
            try
            {
                var user = await _unitOfWork.Users.GetUserWithDetailsAsync(id);
                return user != null ? _mapper.Map<UserResponseDto>(user) : null;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting user by id: {UserId}", id);
                throw;
            }
        }

        public async Task<UserResponseDto?> GetUserByEmailAsync(string email)
        {
            try
            {
                var user = await _unitOfWork.Users.GetByEmailAsync(email);
                return user != null ? _mapper.Map<UserResponseDto>(user) : null;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting user by email: {Email}", email);
                throw;
            }
        }

        public async Task<IEnumerable<UserResponseDto>> GetAllUsersAsync()
        {
            try
            {
                var users = await _unitOfWork.Users.GetAllActiveUsersAsync();
                return _mapper.Map<IEnumerable<UserResponseDto>>(users);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting all users");
                throw;
            }
        }

        public async Task<UserResponseDto> CreateUserAsync(UserCreateDto userCreateDto)
        {
            try
            {
                // Check if email already exists
                if (await _unitOfWork.Users.EmailExistsAsync(userCreateDto.Email))
                {
                    throw new InvalidOperationException("Email đã tồn tại");
                }

                // Map DTO to entity
                var user = _mapper.Map<User>(userCreateDto);

                // Hash password
                user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(userCreateDto.Password);
                user.CreatedAt = DateTime.UtcNow;
                user.UpdatedAt = DateTime.UtcNow;

                // Create user
                await _unitOfWork.Users.AddAsync(user);
                await _unitOfWork.CompleteAsync();

                // Create role-specific record
                if (user.Role == UserRole.Student && userCreateDto.StudentProfile != null)
                {
                    var student = _mapper.Map<Student>(userCreateDto.StudentProfile);
                    student.UserId = user.Id;
                    await _unitOfWork.Students.AddAsync(student);
                }
                else if (user.Role == UserRole.Tutor && userCreateDto.TutorProfile != null)
                {
                    var tutor = _mapper.Map<Tutor>(userCreateDto.TutorProfile);
                    tutor.UserId = user.Id;
                    await _unitOfWork.Tutors.AddAsync(tutor);
                }

                await _unitOfWork.CompleteAsync();

                return _mapper.Map<UserResponseDto>(user);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating user with email: {Email}", userCreateDto.Email);
                throw;
            }
        }

        public async Task<UserResponseDto> UpdateUserAsync(long id, UserUpdateDto userUpdateDto)
        {
            try
            {
                var existingUser = await _unitOfWork.Users.GetByIdAsync(id);
                if (existingUser == null)
                {
                    throw new ArgumentException("Không tìm thấy người dùng");
                }

                // Check if email is being changed and if new email already exists
                if (existingUser.Email != userUpdateDto.Email &&
                    await _unitOfWork.Users.EmailExistsAsync(userUpdateDto.Email))
                {
                    throw new InvalidOperationException("Email đã tồn tại");
                }

                // Map updated properties
                _mapper.Map(userUpdateDto, existingUser);
                existingUser.UpdatedAt = DateTime.UtcNow;

                _unitOfWork.Users.Update(existingUser);
                await _unitOfWork.CompleteAsync();

                return _mapper.Map<UserResponseDto>(existingUser);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating user: {UserId}", id);
                throw;
            }
        }

        public async Task<bool> DeleteUserAsync(long id)
        {
            try
            {
                var user = await _unitOfWork.Users.GetByIdAsync(id);
                if (user == null) return false;

                // Soft delete - set IsActive to false
                user.IsActive = false;
                user.UpdatedAt = DateTime.UtcNow;

                _unitOfWork.Users.Update(user);
                await _unitOfWork.CompleteAsync();

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting user: {UserId}", id);
                throw;
            }
        }

        public async Task<bool> UserExistsAsync(long id)
        {
            try
            {
                return await _unitOfWork.Users.AnyAsync(u => u.Id == id && u.IsActive);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error checking if user exists: {UserId}", id);
                throw;
            }
        }

        public async Task<bool> EmailExistsAsync(string email)
        {
            try
            {
                return await _unitOfWork.Users.EmailExistsAsync(email);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error checking if email exists: {Email}", email);
                throw;
            }
        }

        #endregion

        #region Authentication

        public async Task<LoginResponseDto?> AuthenticateAsync(UserLoginDto loginDto)
        {
            try
            {
                var user = await _unitOfWork.Users.GetByEmailAsync(loginDto.Email);
                if (user == null || !user.IsActive || !BCrypt.Net.BCrypt.Verify(loginDto.Password, user.PasswordHash))
                {
                    return null;
                }

                // Generate JWT tokens
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
                _logger.LogError(ex, "Error authenticating user: {Email}", loginDto.Email);
                throw;
            }
        }

        public async Task<bool> ChangePasswordAsync(long userId, string currentPassword, string newPassword)
        {
            try
            {
                var user = await _unitOfWork.Users.GetByIdAsync(userId);
                if (user == null || !BCrypt.Net.BCrypt.Verify(currentPassword, user.PasswordHash))
                {
                    return false;
                }

                user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(newPassword);
                user.UpdatedAt = DateTime.UtcNow;

                _unitOfWork.Users.Update(user);
                await _unitOfWork.CompleteAsync();

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error changing password for user: {UserId}", userId);
                throw;
            }
        }

        public async Task<LoginResponseDto?> RefreshTokenAsync(string refreshToken)
        {
            try
            {
                return await _jwtService.RefreshTokenAsync(refreshToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error refreshing token");
                throw;
            }
        }

        #endregion

        #region Pagination & Filtering

        public async Task<(IEnumerable<UserResponseDto> Users, int TotalCount)> GetPagedUsersAsync(int page, int pageSize)
        {
            try
            {
                var (users, totalCount) = await _unitOfWork.Users.GetPagedUsersAsync(page, pageSize);
                var userDtos = _mapper.Map<IEnumerable<UserResponseDto>>(users);
                return (userDtos, totalCount);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting paged users");
                throw;
            }
        }

        public async Task<IEnumerable<UserResponseDto>> GetUsersByRoleAsync(UserRole role)
        {
            try
            {
                var users = await _unitOfWork.Users.GetUsersByRoleAsync(role);
                return _mapper.Map<IEnumerable<UserResponseDto>>(users);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting users by role: {Role}", role);
                throw;
            }
        }

        public async Task<IEnumerable<UserResponseDto>> SearchUsersAsync(string searchTerm)
        {
            try
            {
                var users = await _unitOfWork.Users.SearchUsersAsync(searchTerm);
                return _mapper.Map<IEnumerable<UserResponseDto>>(users);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error searching users with term: {SearchTerm}", searchTerm);
                throw;
            }
        }

        #endregion

        #region Role-specific Operations

        public async Task<IEnumerable<StudentResponseDto>> GetStudentsAsync()
        {
            try
            {
                var students = await _unitOfWork.Students.GetStudentsWithUserInfoAsync();
                return _mapper.Map<IEnumerable<StudentResponseDto>>(students);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting students");
                throw;
            }
        }

        public async Task<IEnumerable<TutorResponseDto>> GetTutorsAsync()
        {
            try
            {
                var tutors = await _unitOfWork.Tutors.GetTutorsWithDetailsAsync();
                return _mapper.Map<IEnumerable<TutorResponseDto>>(tutors);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting tutors");
                throw;
            }
        }

        public async Task<IEnumerable<TutorResponseDto>> GetVerifiedTutorsAsync()
        {
            try
            {
                var tutors = await _unitOfWork.Tutors.GetVerifiedTutorsAsync();
                return _mapper.Map<IEnumerable<TutorResponseDto>>(tutors);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting verified tutors");
                throw;
            }
        }

        public async Task<IEnumerable<TutorResponseDto>> GetFeaturedTutorsAsync()
        {
            try
            {
                var tutors = await _unitOfWork.Tutors.GetFeaturedTutorsAsync();
                return _mapper.Map<IEnumerable<TutorResponseDto>>(tutors);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting featured tutors");
                throw;
            }
        }

        #endregion

        #region Profile Management

        public async Task<UserResponseDto> UpdateProfileAsync(long userId, UserProfileUpdateDto profileUpdateDto)
        {
            try
            {
                var user = await _unitOfWork.Users.GetByIdAsync(userId);
                if (user == null)
                {
                    throw new ArgumentException("Không tìm thấy người dùng");
                }

                _mapper.Map(profileUpdateDto, user);
                user.UpdatedAt = DateTime.UtcNow;

                _unitOfWork.Users.Update(user);
                await _unitOfWork.CompleteAsync();

                return _mapper.Map<UserResponseDto>(user);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating profile for user: {UserId}", userId);
                throw;
            }
        }

        public async Task<bool> UploadProfileImageAsync(long userId, string imageUrl)
        {
            try
            {
                var user = await _unitOfWork.Users.GetByIdAsync(userId);
                if (user == null) return false;

                user.ProfileImage = imageUrl;
                user.UpdatedAt = DateTime.UtcNow;

                _unitOfWork.Users.Update(user);
                await _unitOfWork.CompleteAsync();

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error uploading profile image for user: {UserId}", userId);
                throw;
            }
        }

        public async Task<bool> VerifyUserAsync(long userId)
        {
            try
            {
                var user = await _unitOfWork.Users.GetByIdAsync(userId);
                if (user == null) return false;

                user.IsVerified = true;
                user.UpdatedAt = DateTime.UtcNow;

                _unitOfWork.Users.Update(user);
                await _unitOfWork.CompleteAsync();

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error verifying user: {UserId}", userId);
                throw;
            }
        }

        public async Task<bool> DeactivateUserAsync(long userId)
        {
            try
            {
                var user = await _unitOfWork.Users.GetByIdAsync(userId);
                if (user == null) return false;

                user.IsActive = false;
                user.UpdatedAt = DateTime.UtcNow;

                _unitOfWork.Users.Update(user);
                await _unitOfWork.CompleteAsync();

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deactivating user: {UserId}", userId);
                throw;
            }
        }

        public async Task<bool> ActivateUserAsync(long userId)
        {
            try
            {
                var user = await _unitOfWork.Users.GetByIdAsync(userId);
                if (user == null) return false;

                user.IsActive = true;
                user.UpdatedAt = DateTime.UtcNow;

                _unitOfWork.Users.Update(user);
                await _unitOfWork.CompleteAsync();

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error activating user: {UserId}", userId);
                throw;
            }
        }

        #endregion

        #region Statistics - SIMPLE IMPLEMENTATION

        public async Task<UserStatsDto> GetUserStatsAsync()
        {
            try
            {
                var totalUsers = await _unitOfWork.Users.CountAsync(u => u.IsActive);
                var totalStudents = await _unitOfWork.Users.CountAsync(u => u.Role == UserRole.Student && u.IsActive);
                var totalTutors = await _unitOfWork.Users.CountAsync(u => u.Role == UserRole.Tutor && u.IsActive);
                var verifiedTutors = await _unitOfWork.Tutors.CountAsync(t => t.VerificationStatus == VerificationStatus.Verified);

                return new UserStatsDto
                {
                    TotalUsers = totalUsers,
                    TotalStudents = totalStudents,
                    TotalTutors = totalTutors,
                    VerifiedTutors = verifiedTutors,
                    NewUsersThisMonth = await GetNewUsersCountThisMonth()
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting user stats");
                throw;
            }
        }

        public async Task<UserDashboardDto> GetUserDashboardAsync(long userId)
        {
            try
            {
                var user = await _unitOfWork.Users.GetUserWithDetailsAsync(userId);
                if (user == null)
                {
                    throw new ArgumentException("Không tìm thấy người dùng");
                }

                var dashboard = new UserDashboardDto
                {
                    User = _mapper.Map<UserResponseDto>(user),
                    UnreadNotifications = 0, // Will implement when notification repo is ready
                    ActiveMatches = 0, // Will implement when match repo is ready
                    PendingBookings = 0 // Will implement when booking repo is ready
                };

                return dashboard;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting user dashboard: {UserId}", userId);
                throw;
            }
        }


        // Thêm vào UserService
        public async Task<string> UploadProfileImageAsync(long userId, IFormFile imageFile)
        {
            try
            {
                var user = await _unitOfWork.Users.GetByIdAsync(userId);
                if (user == null)
                    throw new ArgumentException("Không tìm thấy người dùng");

                // Xóa ảnh cũ nếu có
                if (!string.IsNullOrEmpty(user.ProfileImage))
                {
                    await _firebaseStorageService.DeleteImageAsync(user.ProfileImage);
                }

                // Upload ảnh mới
                var imageUrl = await _firebaseStorageService.UploadImageAsync(imageFile, "profiles", userId);

                // Cập nhật database
                user.ProfileImage = imageUrl;
                user.UpdatedAt = DateTime.UtcNow;

                _unitOfWork.Users.Update(user);
                await _unitOfWork.CompleteAsync();

                return imageUrl;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error uploading profile image for user: {UserId}", userId);
                throw;
            }
        }

        #endregion

        #region Private Helper Methods

        private async Task<int> GetNewUsersCountThisMonth()
        {
            var startOfMonth = new DateTime(DateTime.UtcNow.Year, DateTime.UtcNow.Month, 1);
            return await _unitOfWork.Users.CountAsync(u => u.CreatedAt >= startOfMonth && u.IsActive);
        }

        #endregion
    }

}