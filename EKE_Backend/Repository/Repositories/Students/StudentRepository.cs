using Microsoft.EntityFrameworkCore;
using Repository.Entities;
using Repository.Repositories.BaseRepository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository.Repositories.Students
{
    public class StudentRepository : BaseRepository<Student>, IStudentRepository
    {
        public StudentRepository(ApplicationDbContext context) : base(context) { }

        public async Task<Student?> GetStudentWithUserInfoAsync(long studentId)
        {
            return await _dbSet
                .Include(s => s.User)
                .FirstOrDefaultAsync(s => s.Id == studentId);
        }

        public async Task<IEnumerable<Student>> GetStudentsWithUserInfoAsync()
        {
            return await _dbSet
                .Include(s => s.User)
                .Where(s => s.User.IsActive)
                .OrderByDescending(s => s.CreatedAt)
                .ToListAsync();
        }

        public async Task<Student?> GetStudentByUserIdAsync(long userId)
        {
            return await _dbSet
                .Include(s => s.User)
                .FirstOrDefaultAsync(s => s.UserId == userId);
        }
    }
}
