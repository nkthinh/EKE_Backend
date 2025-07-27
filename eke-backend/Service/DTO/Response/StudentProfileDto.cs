using Repository.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.DTO.Response
{
    public class StudentProfileDto
    {
        public long Id { get; set; }
        public long UserId { get; set; }
        public string? SchoolName { get; set; }
        public string? GradeLevel { get; set; }
        public string? LearningGoals { get; set; }
        public string? PreferredSchedule { get; set; }
        public decimal? BudgetMin { get; set; }
        public decimal? BudgetMax { get; set; }
        public LearningStyle? LearningStyle { get; set; }
        //public string? LearningStyleText => LearningStyle switch
        //{
        //    Repository.Enum.LearningStyle.Visual => "Thị giác",
        //    Repository.Enum.LearningStyle.Auditory => "Thính giác",
        //    Repository.Enum.LearningStyle.Kinesthetic => "Vận động",
        //    Repository.Enum.LearningStyle.Reading => "Đọc viết",
        //    _ => "Không xác định"
        //};
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

        // Computed properties
        public string BudgetRange => BudgetMin.HasValue && BudgetMax.HasValue
            ? $"{BudgetMin:N0} - {BudgetMax:N0} VNĐ"
            : "Chưa xác định";
    }
}
