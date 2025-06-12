using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.DTO.Response
{
    public class StudentResponseDto
    {
        public long Id { get; set; }
        public long UserId { get; set; }
        public string? SchoolName { get; set; }
        public string? GradeLevel { get; set; }
        public string? LearningGoals { get; set; }
        public decimal? BudgetMin { get; set; }
        public decimal? BudgetMax { get; set; }
        public UserResponseDto User { get; set; } = null!;
    }
}
