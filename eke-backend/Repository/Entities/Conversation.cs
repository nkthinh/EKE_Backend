using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository.Entities
{
    public class Conversation : BaseEntity
    {
        public long MatchId { get; set; }
        public long? LastMessageId { get; set; }
        public DateTime? LastMessageAt { get; set; }

        // Navigation Properties
        public Match Match { get; set; } = null!;
        public ICollection<Message> Messages { get; set; } = new List<Message>();
        public Message? GetLastMessage()
        {
            return Messages?.OrderByDescending(m => m.CreatedAt).FirstOrDefault();
        }
    }
}
