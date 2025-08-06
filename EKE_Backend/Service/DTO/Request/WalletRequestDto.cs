using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.DTO.Request
{
    public class WalletRequestDto
    {
        public long UserId { get; set; }
        public decimal Balance { get; set; }
    }
}
