
using Repository.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.DTO.Request
{
    public class StudentCreateDto
    {
        public string? SchoolName { get; set; }
        public string? GradeLevel { get; set; }
        public string? LearningGoals { get; set; }
        public string? PreferredSchedule { get; set; }

        [Range(0, double.MaxValue, ErrorMessage = "Ngân sách tối thiểu phải >= 0")]
        public decimal? BudgetMin { get; set; }

        [Range(0, double.MaxValue, ErrorMessage = "Ngân sách tối đa phải >= 0")]
        public decimal? BudgetMax { get; set; }

        public LearningStyle? LearningStyle { get; set; }
    }
}
