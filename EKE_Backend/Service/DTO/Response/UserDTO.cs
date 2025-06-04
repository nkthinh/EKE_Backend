using IBTSS.Repository.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.DTO.Response
{
    public class LoginResponseDto
    {
        public string Token { get; set; }
        public string RefreshToken { get; set; }
        public DateTime Expires { get; set; }
        public UserResponseDto User { get; set; }
    }
    public class UserResponseDto
    {
        public int UserId { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public DateTime DateOfBirth { get; set; }
        public string Gender { get; set; }
        public UserRole Role { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
