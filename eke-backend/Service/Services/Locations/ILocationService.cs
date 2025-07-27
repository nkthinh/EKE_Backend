using Service.DTO.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.Services.Locations
{
    public interface ILocationService
    {
        Task<IEnumerable<ProvinceResponseDto>> GetProvincesAsync();
        Task<IEnumerable<DistrictResponseDto>> GetDistrictsByProvinceIdAsync(int provinceId);
    }
}
