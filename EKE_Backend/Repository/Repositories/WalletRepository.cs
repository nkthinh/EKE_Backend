using Microsoft.EntityFrameworkCore;
using Repository;
using Repository.Entities;
using Repository.Repositories;
using Repository.Repositories.BaseRepository;
using System.Threading.Tasks;

public class WalletRepository : BaseRepository<Wallet>, IWalletRepository
{
    public WalletRepository(ApplicationDbContext context) : base(context) { }

    public async Task<Wallet> GetWalletWithUserAsync(long id)
    {
        return await _dbSet.Include(w => w.User).FirstOrDefaultAsync(w => w.Id == id);
    }
}
