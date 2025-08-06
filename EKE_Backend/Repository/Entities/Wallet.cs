using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository.Entities
{
    // Models/Wallet.cs
    
        public class Wallet : BaseEntity
        {
            public long UserId { get; set; }
            public decimal Balance { get; set; } = 0;

            // Navigation
            public User User { get; set; }
        }
    
}
