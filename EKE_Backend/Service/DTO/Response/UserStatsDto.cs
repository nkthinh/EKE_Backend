using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.DTO.Response
{
    public class UserStatsDto
    {
        public int TotalUsers { get; set; }
        public int TotalStudents { get; set; }
        public int TotalTutors { get; set; }
        public int VerifiedTutors { get; set; }
        public int NewUsersThisMonth { get; set; }
    }

}
