using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository.Entities
{
    public class AppSetting : BaseEntity
    {
        public string KeyName { get; set; } = string.Empty;
        public string? Value { get; set; }
        public string? Description { get; set; }
    }
}
