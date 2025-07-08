using Repository.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.DTO.Request
{
    public class SwipeActionDto
    {
        [Required]
        public long TutorId { get; set; }

        [Required]
        public SwipeActionType Action { get; set; } // Like, Dislike
    }

    public class MatchResponseDto
    {
        [Required]
        public long MatchId { get; set; }

        [Required]
        public bool Accept { get; set; }
    }
}
