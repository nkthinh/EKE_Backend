using Repository.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.DTO.Response
{
    public class SwipeActionResponseDto
    {
        public bool Success { get; set; }
        public long? MatchId { get; set; }
        public string Status { get; set; }
    }
    public class SwipeActionRequestDto
    {
        public long TutorId { get; set; }
        public SwipeActionType Action { get; set; }
    }
}
