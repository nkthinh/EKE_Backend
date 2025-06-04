using IBTSS.Repository.Enum;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.DTO.Request
{
    public class UserLoginDto
    {
        [Required, EmailAddress]
        public string Email { get; set; }

        [Required]
        public string Password { get; set; }
    }
    public class RefreshTokenDto
    {
        [Required]
        public string RefreshToken { get; set; }
    }

    public class ChangePasswordDto
    {
        [Required]
        public string CurrentPassword { get; set; }

        [Required, MinLength(6)]
        public string NewPassword { get; set; }
    }
    public class UserCreateDto
    {
        [Required, StringLength(100)]
        public string FullName { get; set; }

        [Required, EmailAddress]
        public string Email { get; set; }

        [Required, StringLength(20)]
        public string PhoneNumber { get; set; }

        [Required, MinLength(6)]
        public string Password { get; set; }

        [DataType(DataType.Date)]
        public DateTime DateOfBirth { get; set; }

        [StringLength(10)]
        public string Gender { get; set; }

        [Required]
        public UserRole Role { get; set; }
    }

    public class UserUpdateDto
    {
        [Required, StringLength(100)]
        public string FullName { get; set; }

        [Required, EmailAddress]
        public string Email { get; set; }

        [Required, StringLength(20)]
        public string PhoneNumber { get; set; }

        [DataType(DataType.Date)]
        public DateTime DateOfBirth { get; set; }

        [StringLength(10)]
        public string Gender { get; set; }

        [Required]
        public UserRole Role { get; set; }
    }
}
