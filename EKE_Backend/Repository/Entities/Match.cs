using Repository.Enums;
using Microsoft.VisualBasic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository.Entities
{
    public class Match : BaseEntity
    {
        public long StudentId { get; set; }
        public long TutorId { get; set; }
        public MatchStatus Status { get; set; } = MatchStatus.Active;
        public DateTime MatchedAt { get; set; } = DateTime.UtcNow;
        public DateTime LastActivity { get; set; } = DateTime.UtcNow;

        // Navigation Properties
        public Student Student { get; set; } = null!;
        public Tutor Tutor { get; set; } = null!;
        public ICollection<Conversation> Conversations { get; set; } = new List<Conversation>();
    }
}
