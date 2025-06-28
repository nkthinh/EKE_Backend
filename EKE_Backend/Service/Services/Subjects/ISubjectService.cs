using Service.DTO.Request;
using Service.DTO.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.Services.Subjects
{
    public interface ISubjectService
    {
        Task<IEnumerable<SubjectResponseDto>> GetAllSubjectsAsync();
        Task<IEnumerable<SubjectResponseDto>> GetActiveSubjectsAsync();
        Task<IEnumerable<SubjectResponseDto>> GetSubjectsByCategoryAsync(string category);
        Task<IEnumerable<string>> GetCategoriesAsync();
        Task<SubjectResponseDto?> GetSubjectByIdAsync(long id);
        Task<SubjectResponseDto> CreateSubjectAsync(SubjectCreateDto subjectCreateDto);
        Task<SubjectResponseDto> UpdateSubjectAsync(long id, SubjectUpdateDto subjectUpdateDto);
        Task<bool> DeleteSubjectAsync(long id);
        Task<bool> SubjectExistsAsync(long id);
    }
}