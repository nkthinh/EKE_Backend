using Repository.Entities;
using Repository.Repositories.BaseRepository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository.Repositories
{

    public interface IWalletRepository : IBaseRepository<Wallet>
    {
        Task<Wallet> GetWalletWithUserAsync(long id);
    }

}
