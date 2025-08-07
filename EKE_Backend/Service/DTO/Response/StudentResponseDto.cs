using Repository.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.DTO.Response
{
    public class StudentResponseDto
    {
        public long Id { get; set; }  // ID của học sinh
        public string FullName { get; set; }  // Tên đầy đủ học sinh từ bảng User
        public string? ProfileImage { get; set; }  // Ảnh đại diện học sinh từ bảng User
        public string? SchoolName { get; set; }  // Tên trường học của học sinh
     
        public string? GradeLevel { get; set; }  // Mức độ lớp của học sinh
        public string? LearningGoals { get; set; }  // Mục tiêu học tập của học sinh
        public string? PreferredSchedule { get; set; }  // Lịch học của học sinh
        public decimal? BudgetMin { get; set; }  // Ngân sách tối thiểu
        public decimal? BudgetMax { get; set; }  // Ngân sách tối đa
        public LearningStyle? LearningStyle { get; set; }  // Phong cách học của học sinh
        public bool IsOnline { get; set; }  // Trạng thái online của học sinh
    }

}
