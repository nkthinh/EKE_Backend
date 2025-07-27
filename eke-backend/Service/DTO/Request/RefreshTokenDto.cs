using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.DTO.Request
{
    public class RefreshTokenDto
    {
        [Required(ErrorMessage = "Refresh token là bắt buộc")]
        public string RefreshToken { get; set; } = string.Empty;
    }
}
