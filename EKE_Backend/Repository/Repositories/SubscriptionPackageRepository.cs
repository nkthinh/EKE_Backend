using Repository.Entities;
using Repository.Repositories.BaseRepository;
using Repository.Repositories.Repository.Repositories.SubscriptionPackages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository.Repositories
{
    public class SubscriptionPackageRepository
        : BaseRepository<SubscriptionPackage>, ISubscriptionPackageRepository
    {
        public SubscriptionPackageRepository(ApplicationDbContext context)
            : base(context)
        {
        }
    }
}
