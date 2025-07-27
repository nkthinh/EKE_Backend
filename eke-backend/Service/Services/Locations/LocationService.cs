using Microsoft.Extensions.Logging;
using Service.DTO.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.Services.Locations
{
    public class LocationService : ILocationService
    {
        private readonly ILogger<LocationService> _logger;

        public LocationService(ILogger<LocationService> logger)
        {
            _logger = logger;
        }

        public async Task<IEnumerable<ProvinceResponseDto>> GetProvincesAsync()
        {
            try
            {
                // Static data for Vietnam provinces - you can replace with database or API call
                var provinces = new List<ProvinceResponseDto>
                {
                    new() { Id = 1, Name = "Hà Nội", Code = "HN" },
                    new() { Id = 2, Name = "Hồ Chí Minh", Code = "HCM" },
                    new() { Id = 3, Name = "Đà Nẵng", Code = "DN" },
                    new() { Id = 4, Name = "Hải Phòng", Code = "HP" },
                    new() { Id = 5, Name = "Cần Thơ", Code = "CT" },
                    new() { Id = 6, Name = "An Giang", Code = "AG" },
                    new() { Id = 7, Name = "Bà Rịa - Vũng Tàu", Code = "BRVT" },
                    new() { Id = 8, Name = "Bắc Giang", Code = "BG" },
                    new() { Id = 9, Name = "Bắc Kạn", Code = "BK" },
                    new() { Id = 10, Name = "Bạc Liêu", Code = "BL" },
                    new() { Id = 11, Name = "Bắc Ninh", Code = "BN" },
                    new() { Id = 12, Name = "Bến Tre", Code = "BT" },
                    new() { Id = 13, Name = "Bình Định", Code = "BD" },
                    new() { Id = 14, Name = "Bình Dương", Code = "BDG" },
                    new() { Id = 15, Name = "Bình Phước", Code = "BP" },
                    new() { Id = 16, Name = "Bình Thuận", Code = "BTH" },
                    new() { Id = 17, Name = "Cà Mau", Code = "CM" },
                    new() { Id = 18, Name = "Cao Bằng", Code = "CB" },
                    new() { Id = 19, Name = "Đắk Lắk", Code = "DL" },
                    new() { Id = 20, Name = "Đắk Nông", Code = "DN2" },
                    new() { Id = 21, Name = "Điện Biên", Code = "DB" },
                    new() { Id = 22, Name = "Đồng Nai", Code = "DNA" },
                    new() { Id = 23, Name = "Đồng Tháp", Code = "DT" },
                    new() { Id = 24, Name = "Gia Lai", Code = "GL" },
                    new() { Id = 25, Name = "Hà Giang", Code = "HG" },
                    new() { Id = 26, Name = "Hà Nam", Code = "HNA" },
                    new() { Id = 27, Name = "Hà Tĩnh", Code = "HT" },
                    new() { Id = 28, Name = "Hải Dương", Code = "HD" },
                    new() { Id = 29, Name = "Hậu Giang", Code = "HGI" },
                    new() { Id = 30, Name = "Hòa Bình", Code = "HB" },
                    new() { Id = 31, Name = "Hưng Yên", Code = "HY" },
                    new() { Id = 32, Name = "Khánh Hòa", Code = "KH" },
                    new() { Id = 33, Name = "Kiên Giang", Code = "KG" },
                    new() { Id = 34, Name = "Kon Tum", Code = "KT" },
                    new() { Id = 35, Name = "Lai Châu", Code = "LC" },
                    new() { Id = 36, Name = "Lâm Đồng", Code = "LD" },
                    new() { Id = 37, Name = "Lạng Sơn", Code = "LS" },
                    new() { Id = 38, Name = "Lào Cai", Code = "LCA" },
                    new() { Id = 39, Name = "Long An", Code = "LA" },
                    new() { Id = 40, Name = "Nam Định", Code = "ND" },
                    new() { Id = 41, Name = "Nghệ An", Code = "NA" },
                    new() { Id = 42, Name = "Ninh Bình", Code = "NB" },
                    new() { Id = 43, Name = "Ninh Thuận", Code = "NT" },
                    new() { Id = 44, Name = "Phú Thọ", Code = "PT" },
                    new() { Id = 45, Name = "Phú Yên", Code = "PY" },
                    new() { Id = 46, Name = "Quảng Bình", Code = "QB" },
                    new() { Id = 47, Name = "Quảng Nam", Code = "QNA" },
                    new() { Id = 48, Name = "Quảng Ngãi", Code = "QNG" },
                    new() { Id = 49, Name = "Quảng Ninh", Code = "QNI" },
                    new() { Id = 50, Name = "Quảng Trị", Code = "QT" },
                    new() { Id = 51, Name = "Sóc Trăng", Code = "ST" },
                    new() { Id = 52, Name = "Sơn La", Code = "SL" },
                    new() { Id = 53, Name = "Tây Ninh", Code = "TN" },
                    new() { Id = 54, Name = "Thái Bình", Code = "TB" },
                    new() { Id = 55, Name = "Thái Nguyên", Code = "TNY" },
                    new() { Id = 56, Name = "Thanh Hóa", Code = "TH" },
                    new() { Id = 57, Name = "Thừa Thiên Huế", Code = "TTH" },
                    new() { Id = 58, Name = "Tiền Giang", Code = "TG" },
                    new() { Id = 59, Name = "Trà Vinh", Code = "TV" },
                    new() { Id = 60, Name = "Tuyên Quang", Code = "TQ" },
                    new() { Id = 61, Name = "Vĩnh Long", Code = "VL" },
                    new() { Id = 62, Name = "Vĩnh Phúc", Code = "VP" },
                    new() { Id = 63, Name = "Yên Bái", Code = "YB" }
                };

                return await Task.FromResult(provinces.AsEnumerable());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting provinces");
                throw;
            }
        }

        public async Task<IEnumerable<DistrictResponseDto>> GetDistrictsByProvinceIdAsync(int provinceId)
        {
            try
            {
                // Static data for some major districts - you can expand this or use database/API
                var districts = new Dictionary<int, List<DistrictResponseDto>>
                {
                    [1] = new() // Hà Nội
                    {
                        new() { Id = 1, Name = "Ba Đình", Code = "BD", ProvinceId = 1, ProvinceName = "Hà Nội" },
                        new() { Id = 2, Name = "Hoàn Kiếm", Code = "HK", ProvinceId = 1, ProvinceName = "Hà Nội" },
                        new() { Id = 3, Name = "Tây Hồ", Code = "TH", ProvinceId = 1, ProvinceName = "Hà Nội" },
                        new() { Id = 4, Name = "Long Biên", Code = "LB", ProvinceId = 1, ProvinceName = "Hà Nội" },
                        new() { Id = 5, Name = "Cầu Giấy", Code = "CG", ProvinceId = 1, ProvinceName = "Hà Nội" },
                        new() { Id = 6, Name = "Đống Đa", Code = "DD", ProvinceId = 1, ProvinceName = "Hà Nội" },
                        new() { Id = 7, Name = "Hai Bà Trưng", Code = "HBT", ProvinceId = 1, ProvinceName = "Hà Nội" },
                        new() { Id = 8, Name = "Hoàng Mai", Code = "HM", ProvinceId = 1, ProvinceName = "Hà Nội" },
                        new() { Id = 9, Name = "Thanh Xuân", Code = "TX", ProvinceId = 1, ProvinceName = "Hà Nội" },
                        new() { Id = 10, Name = "Sóc Sơn", Code = "SS", ProvinceId = 1, ProvinceName = "Hà Nội" },
                        new() { Id = 11, Name = "Đông Anh", Code = "DA", ProvinceId = 1, ProvinceName = "Hà Nội" },
                        new() { Id = 12, Name = "Gia Lâm", Code = "GL", ProvinceId = 1, ProvinceName = "Hà Nội" }
                    },
                    [2] = new() // Hồ Chí Minh
                    {
                        new() { Id = 101, Name = "Quận 1", Code = "Q1", ProvinceId = 2, ProvinceName = "Hồ Chí Minh" },
                        new() { Id = 102, Name = "Quận 2", Code = "Q2", ProvinceId = 2, ProvinceName = "Hồ Chí Minh" },
                        new() { Id = 103, Name = "Quận 3", Code = "Q3", ProvinceId = 2, ProvinceName = "Hồ Chí Minh" },
                        new() { Id = 104, Name = "Quận 4", Code = "Q4", ProvinceId = 2, ProvinceName = "Hồ Chí Minh" },
                        new() { Id = 105, Name = "Quận 5", Code = "Q5", ProvinceId = 2, ProvinceName = "Hồ Chí Minh" },
                        new() { Id = 106, Name = "Quận 6", Code = "Q6", ProvinceId = 2, ProvinceName = "Hồ Chí Minh" },
                        new() { Id = 107, Name = "Quận 7", Code = "Q7", ProvinceId = 2, ProvinceName = "Hồ Chí Minh" },
                        new() { Id = 108, Name = "Quận 8", Code = "Q8", ProvinceId = 2, ProvinceName = "Hồ Chí Minh" },
                        new() { Id = 109, Name = "Quận 9", Code = "Q9", ProvinceId = 2, ProvinceName = "Hồ Chí Minh" },
                        new() { Id = 110, Name = "Quận 10", Code = "Q10", ProvinceId = 2, ProvinceName = "Hồ Chí Minh" },
                        new() { Id = 111, Name = "Quận 11", Code = "Q11", ProvinceId = 2, ProvinceName = "Hồ Chí Minh" },
                        new() { Id = 112, Name = "Quận 12", Code = "Q12", ProvinceId = 2, ProvinceName = "Hồ Chí Minh" },
                        new() { Id = 113, Name = "Quận Bình Thạnh", Code = "BT", ProvinceId = 2, ProvinceName = "Hồ Chí Minh" },
                        new() { Id = 114, Name = "Quận Gò Vấp", Code = "GV", ProvinceId = 2, ProvinceName = "Hồ Chí Minh" },
                        new() { Id = 115, Name = "Quận Phú Nhuận", Code = "PN", ProvinceId = 2, ProvinceName = "Hồ Chí Minh" },
                        new() { Id = 116, Name = "Quận Tân Bình", Code = "TB", ProvinceId = 2, ProvinceName = "Hồ Chí Minh" },
                        new() { Id = 117, Name = "Quận Tân Phú", Code = "TP", ProvinceId = 2, ProvinceName = "Hồ Chí Minh" },
                        new() { Id = 118, Name = "Quận Thủ Đức", Code = "TD", ProvinceId = 2, ProvinceName = "Hồ Chí Minh" },
                        new() { Id = 119, Name = "Huyện Bình Chánh", Code = "BC", ProvinceId = 2, ProvinceName = "Hồ Chí Minh" },
                        new() { Id = 120, Name = "Huyện Hóc Môn", Code = "HM", ProvinceId = 2, ProvinceName = "Hồ Chí Minh" },
                        new() { Id = 121, Name = "Huyện Củ Chi", Code = "CC", ProvinceId = 2, ProvinceName = "Hồ Chí Minh" }
                    },
                    [58] = new() // Tiền Giang
                    {
                        new() { Id = 501, Name = "Mỹ Tho", Code = "MT", ProvinceId = 58, ProvinceName = "Tiền Giang" },
                        new() { Id = 502, Name = "Gò Công", Code = "GC", ProvinceId = 58, ProvinceName = "Tiền Giang" },
                        new() { Id = 503, Name = "Cai Lậy", Code = "CL", ProvinceId = 58, ProvinceName = "Tiền Giang" },
                        new() { Id = 504, Name = "Tân Phước", Code = "TP", ProvinceId = 58, ProvinceName = "Tiền Giang" },
                        new() { Id = 505, Name = "Cái Bè", Code = "CB", ProvinceId = 58, ProvinceName = "Tiền Giang" },
                        new() { Id = 506, Name = "Châu Thành", Code = "CT", ProvinceId = 58, ProvinceName = "Tiền Giang" },
                        new() { Id = 507, Name = "Chợ Gạo", Code = "CGA", ProvinceId = 58, ProvinceName = "Tiền Giang" },
                        new() { Id = 508, Name = "Gò Công Đông", Code = "GCD", ProvinceId = 58, ProvinceName = "Tiền Giang" },
                        new() { Id = 509, Name = "Gò Công Tây", Code = "GCT", ProvinceId = 58, ProvinceName = "Tiền Giang" },
                        new() { Id = 510, Name = "Tân Phú Đông", Code = "TPD", ProvinceId = 58, ProvinceName = "Tiền Giang" }
                    }
                };

                return await Task.FromResult(districts.ContainsKey(provinceId)
                    ? districts[provinceId].AsEnumerable()
                    : new List<DistrictResponseDto>().AsEnumerable());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting districts for province: {ProvinceId}", provinceId);
                throw;
            }
        }
    }
}