using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.DTO.Request
{
    public class CreateReviewRequestDto
    {
        [Required]
        public long TutorId { get; set; }

        [Required]
        [Range(1, 5, ErrorMessage = "Rating must be between 1 and 5")]
        public int Rating { get; set; }

        [MaxLength(1000, ErrorMessage = "Comment cannot exceed 1000 characters")]
        public string? Comment { get; set; }

        public bool IsAnonymous { get; set; } = false;
    }

    public class UpdateReviewRequestDto
    {
        [Required]
        [Range(1, 5, ErrorMessage = "Rating must be between 1 and 5")]
        public int Rating { get; set; }

        [MaxLength(1000, ErrorMessage = "Comment cannot exceed 1000 characters")]
        public string? Comment { get; set; }

        public bool IsAnonymous { get; set; } = false;
    }

    public class ApproveReviewRequestDto
    {
        [Required]
        public bool IsApproved { get; set; }
    }
}
