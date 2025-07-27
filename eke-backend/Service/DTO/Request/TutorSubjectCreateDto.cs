using Repository.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.DTO.Request
{
    public class SubjectCreateDto
    {
        [Required(ErrorMessage = "Tên môn học là bắt buộc")]
        [StringLength(100, ErrorMessage = "Tên môn học không được vượt quá 100 ký tự")]
        public string Name { get; set; } = string.Empty;

        [StringLength(20, ErrorMessage = "Mã môn học không được vượt quá 20 ký tự")]
        public string? Code { get; set; }

        [Required(ErrorMessage = "Danh mục là bắt buộc")]
        [StringLength(50, ErrorMessage = "Danh mục không được vượt quá 50 ký tự")]
        public string Category { get; set; } = string.Empty;

        [StringLength(500, ErrorMessage = "Mô tả không được vượt quá 500 ký tự")]
        public string? Description { get; set; }

        public string? Icon { get; set; }
        public bool IsActive { get; set; } = true;
    }
    public class TutorSubjectCreateDto
    {
        public long TutorId { get; set; } // Sẽ được set từ controller

        [Required(ErrorMessage = "SubjectId is required")]
        public long SubjectId { get; set; }

        [Required(ErrorMessage = "ProficiencyLevel is required")]
        public ProficiencyLevel ProficiencyLevel { get; set; }

        [Range(0, 50, ErrorMessage = "Số năm kinh nghiệm phải từ 0 đến 50")]
        public int YearsExperience { get; set; } = 0;
    }
}
