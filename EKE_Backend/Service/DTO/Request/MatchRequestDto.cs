using Repository.Enums;
using System;
using System.ComponentModel.DataAnnotations;

namespace Application.DTOs
{
    public class MatchRequestDto
    {
        [Required]
        public long StudentId { get; set; }

        [Required]
        public long TutorId { get; set; }

        public MatchStatus? Status { get; set; }
    }
}