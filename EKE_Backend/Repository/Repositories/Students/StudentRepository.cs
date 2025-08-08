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
        public async Task<Student?> GetByUserIdAsync(long userId)
        {
            return await _dbSet
                .Include(s => s.User)
                .FirstOrDefaultAsync(s => s.UserId == userId);
        }
        public async Task UpdateAsync(Student student)
        {
            _dbSet.Update(student); // Cập nhật entity Student
            await _context.SaveChangesAsync(); // Lưu thay đổi vào cơ sở dữ liệu
        }
        // Implement the method to get StudentId from UserId
        public async Task<long?> GetStudentIdByUserIdAsync(long userId)
        {
            var student = await _dbSet
                .Where(s => s.UserId == userId)
                .Select(s => s.Id)  // Chỉ lấy Id của Student
                .FirstOrDefaultAsync();

            return student; // Trả về StudentId nếu tìm thấy, nếu không trả về null
        }

    }
}
