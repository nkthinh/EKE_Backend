using Repository.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Service.DTO.Request;
using Service.Services.Users;
using System.Security.Claims;

namespace EKE_Backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UsersController : ControllerBase
    {
        private readonly IUserService _userService;

        public UsersController(IUserService userService)
        {
            _userService = userService;
        }

        // GET: api/users - Chỉ Admin có thể xem tất cả users
        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetAllUsers()
        {
            try
            {
                var users = await _userService.GetAllUsersAsync();
                return Ok(new { success = true, data = users });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }

        // GET: api/users/5 - Admin có thể xem bất kỳ user nào, user khác chỉ xem được chính mình
        [HttpGet("{id}")]
        [Authorize]
        public async Task<IActionResult> GetUser(long id)
        {
            try
            {
                var currentUserId = long.Parse(User.FindFirst("UserId")?.Value ?? "0");
                var currentUserRole = User.FindFirst(ClaimTypes.Role)?.Value;

                // Admin có thể xem bất kỳ user nào, user khác chỉ xem được chính mình
                if (currentUserRole != "Admin" && currentUserId != id)
                {
                    return Forbid("You can only access your own profile");
                }

                var user = await _userService.GetUserByIdAsync(id);
                if (user == null)
                {
                    return NotFound(new { success = false, message = "User not found" });
                }

                return Ok(new { success = true, data = user });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }

        // GET: api/users/profile - Lấy thông tin profile của user hiện tại
        [HttpGet("profile")]
        [Authorize]
        public async Task<IActionResult> GetCurrentUserProfile()
        {
            try
            {
                var currentUserId = long.Parse(User.FindFirst("UserId")?.Value ?? "0");
                var user = await _userService.GetUserByIdAsync(currentUserId);

                if (user == null)
                {
                    return NotFound(new { success = false, message = "User not found" });
                }

                return Ok(new { success = true, data = user });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }

        // GET: api/users/paged?page=1&pageSize=10 - Chỉ Admin
        [HttpGet("paged")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetPagedUsers([FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            try
            {
                var (users, totalCount) = await _userService.GetPagedUsersAsync(page, pageSize);

                var response = new
                {
                    success = true,
                    data = users,
                    pagination = new
                    {
                        page,
                        pageSize,
                        totalCount,
                        totalPages = (int)Math.Ceiling((double)totalCount / pageSize)
                    }
                };

                return Ok(response);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }

        // POST: api/users - Public endpoint cho registration
        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> CreateUser([FromBody] UserCreateDto userCreateDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new { success = false, message = "Invalid data", errors = ModelState });
            }

            try
            {
                var user = await _userService.CreateUserAsync(userCreateDto);
                return CreatedAtAction(nameof(GetUser), new { id = user.Id },
                    new { success = true, data = user });
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(new { success = false, message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }

        // PUT: api/users/5 - Admin có thể update bất kỳ user nào, user khác chỉ update được chính mình
        [HttpPut("{id}")]
        [Authorize]
        public async Task<IActionResult> UpdateUser(long id, [FromBody] UserUpdateDto userUpdateDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new { success = false, message = "Invalid data", errors = ModelState });
            }

            try
            {
                var currentUserId = long.Parse(User.FindFirst("UserId")?.Value ?? "0");
                var currentUserRole = User.FindFirst(ClaimTypes.Role)?.Value;

                // Admin có thể update bất kỳ user nào, user khác chỉ update được chính mình
                if (currentUserRole != "Admin" && currentUserId != id)
                {
                    return Forbid("You can only update your own profile");
                }

                // User thường không thể thay đổi role của mình
                if (currentUserRole != "Admin" && currentUserId == id)
                {
                    var currentUser = await _userService.GetUserByIdAsync(currentUserId);
                    if (currentUser != null && currentUser.Role != userUpdateDto.Role)
                    {
                        return Forbid("You cannot change your own role");
                    }
                }

                var updatedUser = await _userService.UpdateUserAsync(id, userUpdateDto);
                return Ok(new { success = true, data = updatedUser });
            }
            catch (ArgumentException ex)
            {
                return NotFound(new { success = false, message = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(new { success = false, message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }

        // DELETE: api/users/5 - Chỉ Admin
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteUser(long id)
        {
            try
            {
                var deleted = await _userService.DeleteUserAsync(id);
                if (!deleted)
                {
                    return NotFound(new { success = false, message = "User not found" });
                }

                return Ok(new { success = true, message = "User deleted successfully" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }

        // POST: api/users/login - Public endpoint
        [HttpPost("login")]
        [AllowAnonymous]
        public async Task<IActionResult> Login([FromBody] UserLoginDto loginDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new { success = false, message = "Invalid data", errors = ModelState });
            }

            try
            {
                var loginResponse = await _userService.AuthenticateAsync(loginDto);
                if (loginResponse == null)
                {
                    return Unauthorized(new { success = false, message = "Invalid email or password" });
                }

                return Ok(new { success = true, data = loginResponse });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }

        // PUT: api/users/change-password - User có thể đổi password của chính mình
        [HttpPut("change-password")]
        [Authorize]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordDto changePasswordDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new { success = false, message = "Invalid data", errors = ModelState });
            }

            try
            {
                var currentUserId = long.Parse(User.FindFirst("UserId")?.Value ?? "0");

                var success = await _userService.ChangePasswordAsync(currentUserId,
                    changePasswordDto.CurrentPassword, changePasswordDto.NewPassword);

                if (!success)
                {
                    return BadRequest(new { success = false, message = "Current password is incorrect" });
                }

                return Ok(new { success = true, message = "Password changed successfully" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }

        // GET: api/users/check-email/{email} - Public endpoint
        [HttpGet("check-email/{email}")]
        [AllowAnonymous]
        public async Task<IActionResult> CheckEmailExists(string email)
        {
            try
            {
                var exists = await _userService.EmailExistsAsync(email);
                return Ok(new { success = true, exists });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }

        // GET: api/users/students - Admin và Tutor có thể xem danh sách students
        [HttpGet("students")]
        [Authorize(Roles = "Admin,Tutor")]
        public async Task<IActionResult> GetStudents()
        {
            try
            {
                var students = await _userService.GetStudentsAsync();
                return Ok(new { success = true, data = students });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }

        // GET: api/users/tutors - Admin và Student có thể xem danh sách tutors
        [HttpGet("tutors")]
        [Authorize(Roles = "Admin,Student")]
        public async Task<IActionResult> GetTutors()
        {
            try
            {
                var tutors = await _userService.GetTutorsAsync();
                return Ok(new { success = true, data = tutors });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }

        // GET: api/users/stats - Admin only
        [HttpGet("stats")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetUserStats()
        {
            try
            {
                var stats = await _userService.GetUserStatsAsync();
                return Ok(new { success = true, data = stats });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }

        // GET: api/users/dashboard - Authenticated users
        [HttpGet("dashboard")]
        [Authorize]
        public async Task<IActionResult> GetUserDashboard()
        {
            try
            {
                var currentUserId = long.Parse(User.FindFirst("UserId")?.Value ?? "0");
                var dashboard = await _userService.GetUserDashboardAsync(currentUserId);
                return Ok(new { success = true, data = dashboard });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }
    }
}

