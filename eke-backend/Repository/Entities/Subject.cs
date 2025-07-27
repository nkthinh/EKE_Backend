using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository.Entities
{
    public class Subject : BaseEntity
    {
        public string Name { get; set; } = string.Empty;
        public string? Code { get; set; }
        public string? Category { get; set; }
        public string? Description { get; set; }
        public string? Icon { get; set; }
        public bool IsActive { get; set; } = true;

        // Navigation Properties
        public ICollection<TutorSubject> TutorSubjects { get; set; } = new List<TutorSubject>();
        public ICollection<Booking> Bookings { get; set; } = new List<Booking>();
    }
}
