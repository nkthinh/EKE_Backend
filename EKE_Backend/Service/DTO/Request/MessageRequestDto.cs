using Microsoft.AspNetCore.Http;
using Repository.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.DTO.Request
{
    public class MessageCreateDto
    {
        public long ConversationId { get; set; }  // Updated to match existing entity
        public long SenderId { get; set; }
        public string Content { get; set; } = string.Empty;
        public MessageType MessageType { get; set; } = MessageType.Text;  // Updated property name
    }

    public class MessageWithFileDto
    {
        public long ConversationId { get; set; }  // Updated to match existing entity
        public long SenderId { get; set; }
        public string? Content { get; set; }
        public IFormFile? File { get; set; }
        public MessageType MessageType { get; set; } = MessageType.File;  // Updated property name
        public string FileUrl { get; set; }
    }
}
