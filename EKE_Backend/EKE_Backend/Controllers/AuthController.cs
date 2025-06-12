using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Service.DTO.Request;
using Service.Services.Auth;

namespace EKE_Backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        // POST: api/auth/login
        [HttpPost("login")]
        [AllowAnonymous]
        public async Task<IActionResult> Login([FromBody] UserLoginDto loginDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new { success = false, message = "Dữ liệu không hợp lệ", errors = ModelState });
            }

            try
            {
                var loginResponse = await _authService.LoginAsync(loginDto);
                if (loginResponse == null)
                {
                    return Unauthorized(new { success = false, message = "Email hoặc mật khẩu không đúng" });
                }

                return Ok(new { success = true, data = loginResponse });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }

        // POST: api/auth/register/student
        [HttpPost("register/student")]
        [AllowAnonymous]
        public async Task<IActionResult> RegisterStudent([FromBody] StudentSignUpDto studentSignUpDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new { success = false, message = "Dữ liệu không hợp lệ", errors = ModelState });
            }

            try
            {
                var user = await _authService.RegisterStudentAsync(studentSignUpDto);
                return CreatedAtAction(nameof(Login), new { },
                    new
                    {
                        success = true,
                        message = "Đăng ký học sinh thành công",
                        data = user
                    });
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(new { success = false, message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = "Lỗi hệ thống" });
            }
        }

        // POST: api/auth/register/tutor
        [HttpPost("register/tutor")]
        [AllowAnonymous]
        public async Task<IActionResult> RegisterTutor([FromBody] TutorSignUpDto tutorSignUpDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new { success = false, message = "Dữ liệu không hợp lệ", errors = ModelState });
            }

            try
            {
                var user = await _authService.RegisterTutorAsync(tutorSignUpDto);
                return CreatedAtAction(nameof(Login), new { },
                    new
                    {
                        success = true,
                        message = "Đăng ký gia sư thành công. Tài khoản đang chờ xác minh.",
                        data = user
                    });
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(new { success = false, message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = "Lỗi hệ thống" });
            }
        }

        // POST: api/auth/refresh
        [HttpPost("refresh")]
        [AllowAnonymous]
        public async Task<IActionResult> RefreshToken([FromBody] RefreshTokenDto refreshTokenDto)
        {
            try
            {
                var loginResponse = await _authService.RefreshTokenAsync(refreshTokenDto.RefreshToken);
                if (loginResponse == null)
                {
                    return Unauthorized(new { success = false, message = "Refresh token không hợp lệ" });
                }

                return Ok(new { success = true, data = loginResponse });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }

        // POST: api/auth/logout
        [HttpPost("logout")]
        [Authorize]
        public async Task<IActionResult> Logout([FromBody] RefreshTokenDto refreshTokenDto)
        {
            try
            {
                var success = await _authService.LogoutAsync(refreshTokenDto.RefreshToken);
                return Ok(new { success = true, message = "Đăng xuất thành công" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }

        // GET: api/auth/check-email/{email}
        [HttpGet("check-email/{email}")]
        [AllowAnonymous]
        public async Task<IActionResult> CheckEmail(string email)
        {
            try
            {
                var exists = await _authService.EmailExistsAsync(email);
                return Ok(new { success = true, exists, message = exists ? "Email đã được sử dụng" : "Email có thể sử dụng" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }
    }
}
