using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository.Entities
{
    public class AiChatSession : BaseEntity
    {
        public long UserId { get; set; }
        public string SessionId { get; set; } = string.Empty;
        public bool IsActive { get; set; } = true;
        public DateTime? EndedAt { get; set; }

        // Navigation Properties
        public User User { get; set; } = null!;
        public ICollection<AiChatMessage> AiChatMessages { get; set; } = new List<AiChatMessage>();
    }
}
