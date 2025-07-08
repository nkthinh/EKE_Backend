using Microsoft.EntityFrameworkCore.Storage;
using Repository;
using Repository.Entities;
using Repository.Repositories.Certifications;
using Repository.Repositories.Matches;
using Repository.Repositories.Students;
using Repository.Repositories.Subjects;
using Repository.Repositories.SwipeActions;
using Repository.Repositories.Tutors;
using Repository.Repositories.Users;
using System;


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
        private IMatchRepository? _matches;
        private ISwipeActionRepository? _swipeActions;
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
        public IMatchRepository Matches => _matches ??= new MatchRepository(_context);
        public ISwipeActionRepository SwipeActions => _swipeActions=new SwipeActionRepository(_context);       

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



    
