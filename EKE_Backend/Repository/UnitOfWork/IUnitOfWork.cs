
using Repository.Repositories.Certifications;
using Repository.Repositories.Matches;
using Repository.Repositories.Students;
using Repository.Repositories.Subjects;
using Repository.Repositories.SwipeActions;
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
        IStudentRepository Students { get; }
        ITutorRepository Tutors { get; }
        ITutorSubjectRepository TutorSubjects { get; }
        ISubjectRepository Subjects { get; }
        ICertificationRepository Certifications { get; } 
        ISwipeActionRepository SwipeActions { get; }
        IMatchRepository Matches { get; }

        // INotificationRepository Notifications { get; }
        // IBookingRepository Bookings { get; }

        // Transaction Methods
        Task<int> CompleteAsync();
        Task BeginTransactionAsync();
        Task CommitTransactionAsync();
        Task RollbackTransactionAsync();
    }
}
