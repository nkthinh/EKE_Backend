
using Repository.Repositories.Students;
using Repository.Repositories.Tutors;
using Repository.Repositories.Users;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository.UnitOfWork
{
    public interface IUnitOfWork : IDisposable
    {
        // Repository Properties
        IUserRepository Users { get; }
        IStudentService Students { get; }
        ITutorRepository Tutors { get; }
        // Add other repositories as needed
        // ISubjectRepository Subjects { get; }
        // ISwipeActionRepository SwipeActions { get; }
        // IMatchRepository Matches { get; }
        // INotificationRepository Notifications { get; }
        // IBookingRepository Bookings { get; }

        // Transaction Methods
        Task<int> CompleteAsync();
        Task BeginTransactionAsync();
        Task CommitTransactionAsync();
        Task RollbackTransactionAsync();
    }
}
