using Repository.Repositories.Certifications;
using Repository.Repositories.Conversations;
using Repository.Repositories.Matches;
using Repository.Repositories.Messages;
using Repository.Repositories.Notifications;
using Repository.Repositories.Reviews;
using Repository.Repositories.Students;
using Repository.Repositories.Subjects;
using Repository.Repositories.SwipeActions;
using Repository.Repositories.Tutors;
using Repository.Repositories.Users;

namespace Repository.UnitOfWork
{
    public interface IUnitOfWork : IDisposable
    {
        IUserRepository Users { get; }
        IStudentRepository Students { get; }
        ITutorRepository Tutors { get; }
        ITutorSubjectRepository TutorSubjects { get; }
        ISubjectRepository Subjects { get; }
        ICertificationRepository Certifications { get; }
        ISwipeActionRepository SwipeActions { get; }
        IMatchRepository Matches { get; }
        IConversationRepository Conversations { get; }
        IMessageRepository Messages { get; }
        IReviewRepository Reviews { get; }
        INotificationRepository Notifications { get; }

        Task<int> CompleteAsync();
        Task BeginTransactionAsync();
        Task CommitTransactionAsync();
        Task RollbackTransactionAsync();
    }
}