using Repository.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository.Entities
{
    public class SwipeAction : BaseEntity
    {
        public long StudentId { get; set; }
        public long TutorId { get; set; }
        public SwipeActionType Action { get; set; }

        // Navigation Properties
        public Student Student { get; set; } = null!;
        public Tutor Tutor { get; set; } = null!;
    }

}
