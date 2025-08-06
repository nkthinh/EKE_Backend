using Repository.Entities;
using Repository.Repositories.BaseRepository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository.Repositories
{
    

    namespace Repository.Repositories.SubscriptionPackages
    {
        public interface ISubscriptionPackageRepository : IBaseRepository<SubscriptionPackage>
        {
            // Nếu cần thêm các hàm đặc thù cho SubscriptionPackage thì thêm ở đây
        }
    }

}
