using Repository.Entities;
using Repository.Repositories.BaseRepository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository.Repositories.Students
{
    public interface IStudentRepository : IBaseRepository<Student>
    {
        Task<Student?> GetStudentWithUserInfoAsync(long studentId);
        Task<IEnumerable<Student>> GetStudentsWithUserInfoAsync();
        Task<Student?> GetStudentByUserIdAsync(long userId);
        Task<Student?> GetByUserIdAsync(long userId);
        Task UpdateAsync(Student student);
        Task<long?> GetStudentIdByUserIdAsync(long userId);
    }
}
