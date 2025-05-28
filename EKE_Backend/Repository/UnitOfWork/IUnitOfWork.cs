
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IBTSS.Repository.UnitOfWork
{
    public interface IUnitOfWork : IDisposable
    {      
        Task<int> CompleteAsync();
    }
}
