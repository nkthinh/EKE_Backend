using Microsoft.EntityFrameworkCore.Storage;
using Repository;
using Repository.Entities;
using Repository.Repositories;
using Repository.Repositories.Certifications;
using Repository.Repositories.Conversations;
using Repository.Repositories.Matches;
using Repository.Repositories.Messages;
using Repository.Repositories.Notifications;
using Repository.Repositories.Repository.Repositories.SubscriptionPackages;
using Repository.Repositories.Reviews;
using Repository.Repositories.Students;
using Repository.Repositories.Subjects;
using Repository.Repositories.SwipeActions;
using Repository.Repositories.Tutors;
using Repository.Repositories.Users;
using System;
using IMatchRepository = Repository.Repositories.Matches.IMatchRepository;

namespace Repository.UnitOfWork
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ApplicationDbContext _context;
        private IDbContextTransaction? _transaction;

        // Repository instances
        private IUserRepository? _users;
        private IStudentRepository? _students;
        private ITutorRepository? _tutors;
        private ITutorSubjectRepository? _tutorSubjects;
        private ISubjectRepository? _subjects;
        private ICertificationRepository? _certifications;
        private ISwipeActionRepository? _swipeActions;
        private IMatchRepository? _matches;
        private IConversationRepository? _conversations;
        private IMessageRepository? _messages;
        private IReviewRepository? _reviews;
        private INotificationRepository? _notifications;

        public UnitOfWork(ApplicationDbContext context)
        {
            _context = context;
        }

        // Repository Properties - Lazy Loading
        public IUserRepository Users => _users ??= new UserRepository(_context);
        public IStudentRepository Students => _students ??= new StudentRepository(_context);
        public ITutorRepository Tutors => _tutors ??= new TutorRepository(_context);
        public ITutorSubjectRepository TutorSubjects => _tutorSubjects ??= new TutorSubjectRepository(_context);
        public ISubjectRepository Subjects => _subjects ??= new SubjectRepository(_context);
        public ICertificationRepository Certifications => _certifications ??= new CertificationRepository(_context);
        public ISwipeActionRepository SwipeActions => _swipeActions ??= new SwipeActionRepository(_context);
        public Repositories.Matches.IMatchRepository Matches => _matches ??= new Repositories.Matches.MatchRepository(_context);
        public IConversationRepository Conversations => _conversations ??= new ConversationRepository(_context);
        public IMessageRepository Messages => _messages ??= new MessageRepository(_context);
        public IReviewRepository Reviews => _reviews ??= new ReviewRepository(_context);
        public INotificationRepository Notifications => _notifications ??= new NotificationRepository(_context);

        private IWalletRepository? _wallets;
        public IWalletRepository Wallets => _wallets ??= new WalletRepository(_context);
        private ISubscriptionPackageRepository? _subscriptionPackages;

        public ISubscriptionPackageRepository SubscriptionPackages => _subscriptionPackages ??= new SubscriptionPackageRepository(_context);

        public async Task<int> CompleteAsync()
        {
            try
            {
                return await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                // Log exception nếu cần
                throw;
            }
        }

        public async Task BeginTransactionAsync()
        {
            if (_transaction != null)
            {
                throw new InvalidOperationException("Transaction đã được bắt đầu");
            }

            _transaction = await _context.Database.BeginTransactionAsync();
        }

        public async Task CommitTransactionAsync()
        {
            if (_transaction == null)
            {
                throw new InvalidOperationException("Không có transaction nào để commit");
            }

            try
            {
                await _transaction.CommitAsync();
            }
            catch
            {
                await _transaction.RollbackAsync();
                throw;
            }
            finally
            {
                await _transaction.DisposeAsync();
                _transaction = null;
            }
        }

        public async Task RollbackTransactionAsync()
        {
            if (_transaction == null)
            {
                throw new InvalidOperationException("Không có transaction nào để rollback");
            }

            try
            {
                await _transaction.RollbackAsync();
            }
            finally
            {
                await _transaction.DisposeAsync();
                _transaction = null;
            }
        }

        public void Dispose()
        {
            _transaction?.Dispose();
            _context.Dispose();
        }
    }
}