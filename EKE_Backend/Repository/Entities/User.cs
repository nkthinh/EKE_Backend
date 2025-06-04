using IBTSS.Repository.Enum;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository.Entities
{
    public class User
    {
        [Key]
        public int UserId { get; set; }

        [Required, StringLength(100)]
        public string FullName { get; set; }

        [Required, EmailAddress]
        public string Email { get; set; }

        [Required, StringLength(20)]
        public string PhoneNumber { get; set; }

        [Required]
        public string PasswordHash { get; set; }

        [DataType(DataType.Date)]
        public DateTime DateOfBirth { get; set; }

        [StringLength(10)]
        public string Gender { get; set; } // "Nam" / "Nữ"

        [Required]
        public UserRole Role { get; set; } // ✅ Dùng enum thay cho string

        // Navigation
        public ICollection<StudentProfile> StudentProfiles { get; set; }
        public TutorProfile TutorProfile { get; set; }
    }
}
