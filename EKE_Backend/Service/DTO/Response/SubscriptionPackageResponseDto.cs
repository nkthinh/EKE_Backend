using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.DTO.Response
{
    public class SubscriptionPackageResponseDto
    {
        public long Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public string Description { get; set; } = string.Empty;

        public List<UserBasicInfoDto> Users { get; set; } = new();


        public bool HasPriorityMatching { get; set; }
        public bool HasAiAssistant { get; set; }
        public bool NoAds { get; set; }

        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}

