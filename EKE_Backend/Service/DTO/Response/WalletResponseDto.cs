using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.DTO.Response
{
    public class WalletResponseDto
    {
        public long Id { get; set; }
        public long UserId { get; set; }
        public decimal Balance { get; set; }
        public string? UserFullName { get; set; }
    }
}
