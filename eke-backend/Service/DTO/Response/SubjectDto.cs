using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.DTO.Response
{
    public class SubjectResponseDto
    {
        public long Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Code { get; set; }
        public string? Category { get; set; }
        public string? Description { get; set; }
        public string? Icon { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }

    public class SubjectCategoryResponseDto
    {
        public string Category { get; set; } = string.Empty;
        public int SubjectCount { get; set; }
    }
}
