using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Service.Services.Locations;

namespace EKE_Backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class LocationsController : ControllerBase
    {
        private readonly ILocationService _locationService;

        public LocationsController(ILocationService locationService)
        {
            _locationService = locationService;
        }

        // GET: api/locations/provinces
        [HttpGet("provinces")]
        [AllowAnonymous]
        public async Task<IActionResult> GetProvinces()
        {
            try
            {
                var provinces = await _locationService.GetProvincesAsync();
                return Ok(new { success = true, data = provinces });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }

        // GET: api/locations/districts/{provinceId}
        [HttpGet("districts/{provinceId}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetDistricts(int provinceId)
        {
            try
            {
                var districts = await _locationService.GetDistrictsByProvinceIdAsync(provinceId);
                return Ok(new { success = true, data = districts });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }
    }
}