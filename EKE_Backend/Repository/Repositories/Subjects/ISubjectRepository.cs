using Repository.Entities;
using Repository.Repositories.BaseRepository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository.Repositories.Subjects
{
    public interface ISubjectRepository : IBaseRepository<Subject>
    {
        Task<IEnumerable<Subject>> GetActiveSubjectsAsync();
        Task<IEnumerable<Subject>> GetSubjectsByCategoryAsync(string category);
        Task<IEnumerable<string>> GetCategoriesAsync();
        Task<Subject?> GetByCodeAsync(string code);
        Task<bool> CodeExistsAsync(string code);
    }
}
