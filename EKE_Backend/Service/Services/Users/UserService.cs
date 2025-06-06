using AutoMapper;
using Repository.Entities;
using Repository.Repositories.Users;
using Service.DTO.Request;
using Service.DTO.Response;
using Service.Services.Jwt;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.Services.Users
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;
        private readonly IJwtService _jwtService;

        public UserService(IUserRepository userRepository, IMapper mapper, IJwtService jwtService)
        {
            _userRepository = userRepository;
            _mapper = mapper;
            _jwtService = jwtService;
        }

        public async Task<UserResponseDto> GetUserByIdAsync(int id)
        {
            var user = await _userRepository.GetByIdAsync(id);
            return _mapper.Map<UserResponseDto>(user);
        }

        public async Task<UserResponseDto> GetUserByEmailAsync(string email)
        {
            var user = await _userRepository.GetByEmailAsync(email);
            return _mapper.Map<UserResponseDto>(user);
        }

        public async Task<IEnumerable<UserResponseDto>> GetAllUsersAsync()
        {
            var users = await _userRepository.GetAllAsync();
            return _mapper.Map<IEnumerable<UserResponseDto>>(users);
        }

        public async Task<UserResponseDto> CreateUserAsync(UserCreateDto userCreateDto)
        {
            // Check if email already exists
            if (await _userRepository.EmailExistsAsync(userCreateDto.Email))
            {
                throw new InvalidOperationException("Email already exists");
            }

            // Map DTO to entity
            var user = _mapper.Map<User>(userCreateDto);

            // Hash password
            user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(userCreateDto.Password);

            // Save user
            var createdUser = await _userRepository.AddAsync(user);

            return _mapper.Map<UserResponseDto>(createdUser);
        }

        public async Task<UserResponseDto> UpdateUserAsync(int id, UserUpdateDto userUpdateDto)
        {
            var existingUser = await _userRepository.GetByIdAsync(id);
            if (existingUser == null)
            {
                throw new ArgumentException("User not found");
            }

            // Check if email is being changed and if new email already exists
            if (existingUser.Email != userUpdateDto.Email &&
                await _userRepository.EmailExistsAsync(userUpdateDto.Email))
            {
                throw new InvalidOperationException("Email already exists");
            }

            // Map updated properties
            _mapper.Map(userUpdateDto, existingUser);

            var updatedUser = await _userRepository.UpdateAsync(existingUser);
            return _mapper.Map<UserResponseDto>(updatedUser);
        }

        public async Task<bool> DeleteUserAsync(int id)
        {
            return await _userRepository.DeleteAsync(id);
        }

        public async Task<bool> UserExistsAsync(int id)
        {
            return await _userRepository.ExistsAsync(id);
        }

        public async Task<bool> EmailExistsAsync(string email)
        {
            return await _userRepository.EmailExistsAsync(email);
        }

        public async Task<LoginResponseDto> AuthenticateAsync(UserLoginDto loginDto)
        {
            var user = await _userRepository.GetByEmailAsync(loginDto.Email);
            if (user == null || !BCrypt.Net.BCrypt.Verify(loginDto.Password, user.PasswordHash))
            {
                return null;
            }

            // Generate JWT token
            var token = _jwtService.GenerateAccessToken(user);
            var refreshToken = _jwtService.GenerateRefreshToken();

            return new LoginResponseDto
            {
                Token = token,
                RefreshToken = refreshToken,
                Expires = DateTime.UtcNow.AddMinutes(5), // Or get from config
                User = _mapper.Map<UserResponseDto>(user)
            };
        }

        public async Task<(IEnumerable<UserResponseDto> Users, int TotalCount)> GetPagedUsersAsync(int page, int pageSize)
        {
            var users = await _userRepository.GetPagedAsync(page, pageSize);
            var totalCount = await _userRepository.CountAsync();

            var userDtos = _mapper.Map<IEnumerable<UserResponseDto>>(users);
            return (userDtos, totalCount);
        }

        public async Task<bool> ChangePasswordAsync(int userId, string currentPassword, string newPassword)
        {
            var user = await _userRepository.GetByIdAsync(userId);
            if (user == null || !BCrypt.Net.BCrypt.Verify(currentPassword, user.PasswordHash))
            {
                return false;
            }

            user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(newPassword);
            await _userRepository.UpdateAsync(user);
            return true;
        }
    }
}
