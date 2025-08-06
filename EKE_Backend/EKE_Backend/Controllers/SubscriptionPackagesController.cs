using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Service.DTO.Request;
using Service.Services;
using Service.Services.SubscriptionPackages;
using System.Security.Claims;

namespace EKE_Backend.Controllers
{
    [ApiController]
    [Route("api/subscriptions")]
    [Authorize]
    public class SubscriptionPackagesController : ControllerBase
    {
        private readonly ISubscriptionPackageService _subscriptionService;

        public SubscriptionPackagesController(ISubscriptionPackageService subscriptionService)
        {
            _subscriptionService = subscriptionService;
        }

        // GET: api/subscriptions
        [HttpGet]
        [AllowAnonymous] // Cho phép xem gói mà không cần đăng nhập
        public async Task<IActionResult> GetAllPackages()
        {
            try
            {
                var packages = await _subscriptionService.GetAllPackagesAsync();

                return Ok(new { success = true, data = packages });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }

        // GET: api/subscriptions/{packageId}
        [HttpGet("{packageId:long}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetPackageById(long packageId)
        {
            try
            {
                var package = await _subscriptionService.GetPackageByIdAsync(packageId);
                if (package == null)
                    return NotFound(new { success = false, message = "Không tìm thấy gói" });

                return Ok(new { success = true, data = package });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }

        // POST: api/subscriptions
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> CreatePackage([FromBody] SubscriptionPackageCreateDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(new { success = false, message = "Dữ liệu không hợp lệ", errors = ModelState });

            try
            {
                var package = await _subscriptionService.CreatePackageAsync(dto);
                return Ok(new { success = true, data = package, message = "Tạo gói thành công" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }

        // PUT: api/subscriptions/{packageId}
        [HttpPut("{packageId:long}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdatePackage(long packageId, [FromBody] SubscriptionPackageUpdateDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(new { success = false, message = "Dữ liệu không hợp lệ", errors = ModelState });

            try
            {
                var updated = await _subscriptionService.UpdatePackageAsync(packageId, dto);
                if (updated == null)
                    return NotFound(new { success = false, message = "Không tìm thấy gói" });

                return Ok(new { success = true, data = updated, message = "Cập nhật gói thành công" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }

        // DELETE: api/subscriptions/{packageId}
        [HttpDelete("{packageId:long}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeletePackage(long packageId)
        {
            try
            {
                var deleted = await _subscriptionService.DeletePackageAsync(packageId);
                if (!deleted)
                    return NotFound(new { success = false, message = "Không tìm thấy gói" });

                return Ok(new { success = true, message = "Xóa gói thành công" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }

        // POST: api/subscriptions/{packageId}/purchase
        [HttpPost("{packageId:long}/purchase")]
        public async Task<IActionResult> PurchasePackage(long packageId)
        {
            try
            {
                var currentUserId = long.Parse(User.FindFirst("UserId")?.Value ?? "0");
                if (currentUserId <= 0)
                    return Unauthorized(new { success = false, message = "Không xác định được người dùng" });

                var success = await _subscriptionService.PurchasePackageAsync(currentUserId, packageId);
                if (!success)
                    return BadRequest(new { success = false, message = "Số dư không đủ hoặc lỗi giao dịch" });

                return Ok(new { success = true, message = "Mua gói thành công" });
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
    }
}
