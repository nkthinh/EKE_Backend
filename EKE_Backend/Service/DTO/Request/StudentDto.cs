using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.DTO.Request
{
    public class StudentDto
    {
        public long Id { get; set; }
        public string SchoolName { get; set; }
        public string GradeLevel { get; set; }
        public string LearningGoals { get; set; }
        public string PreferredSchedule { get; set; }
        public decimal? BudgetMin { get; set; }
        public decimal? BudgetMax { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
    public class StudentUpdateDto
    {
        public string SchoolName { get; set; }
        public string GradeLevel { get; set; }
        public string LearningGoals { get; set; }
        public string PreferredSchedule { get; set; }
        public decimal? BudgetMin { get; set; }
        public decimal? BudgetMax { get; set; }
    }
}
