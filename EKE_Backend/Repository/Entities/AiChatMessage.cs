using Repository.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository.Entities
{
    public class AiChatMessage : BaseEntity
    {
        public long SessionId { get; set; }
        public SenderType SenderType { get; set; }
        public string Message { get; set; } = string.Empty;

        // Navigation Properties
        public AiChatSession Session { get; set; } = null!;
    }
}
