using Microsoft.EntityFrameworkCore;
using Repository.Entities;
using Repository.Repositories.BaseRepository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository.Repositories.Subjects
{
    public class SubjectRepository : BaseRepository<Subject>, ISubjectRepository
    {
        public SubjectRepository(ApplicationDbContext context) : base(context) { }

        public async Task<IEnumerable<Subject>> GetActiveSubjectsAsync()
        {
            return await _dbSet
                .Where(s => s.IsActive)
                .OrderBy(s => s.Category)
                .ThenBy(s => s.Name)
                .ToListAsync();
        }

        public async Task<IEnumerable<Subject>> GetSubjectsByCategoryAsync(string category)
        {
            return await _dbSet
                .Where(s => s.IsActive && s.Category == category)
                .OrderBy(s => s.Name)
                .ToListAsync();
        }

        public async Task<IEnumerable<string>> GetCategoriesAsync()
        {
            return await _dbSet
                .Where(s => s.IsActive && !string.IsNullOrEmpty(s.Category))
                .Select(s => s.Category!)
                .Distinct()
                .OrderBy(c => c)
                .ToListAsync();
        }

        public async Task<Subject?> GetByCodeAsync(string code)
        {
            return await _dbSet
                .FirstOrDefaultAsync(s => s.Code == code);
        }

        public async Task<bool> CodeExistsAsync(string code)
        {
            return await _dbSet
                .AnyAsync(s => s.Code == code);
        }
    }
}