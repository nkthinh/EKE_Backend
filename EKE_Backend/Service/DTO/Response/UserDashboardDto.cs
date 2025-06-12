using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.DTO.Response
{
    public class UserDashboardDto
    {
        public UserResponseDto User { get; set; } = null!;
        public int UnreadNotifications { get; set; }
        public int ActiveMatches { get; set; }
        public int PendingBookings { get; set; }
    }
}
