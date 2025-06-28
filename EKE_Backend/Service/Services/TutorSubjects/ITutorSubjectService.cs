using Service.DTO.Request;
using Service.DTO.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.Services.TutorSubjects
{
    public interface ITutorSubjectService
    {
        Task<IEnumerable<TutorSubjectDto>> GetTutorSubjectsAsync(long tutorId);
        Task<TutorSubjectDto> AddTutorSubjectAsync(long tutorId, TutorSubjectCreateDto tutorSubjectDto);
        Task<TutorSubjectDto> UpdateTutorSubjectAsync(long tutorId, long subjectId, TutorSubjectUpdateDto tutorSubjectDto);
        Task<bool> RemoveTutorSubjectAsync(long tutorId, long subjectId);
        Task<bool> TutorHasSubjectAsync(long tutorId, long subjectId);

    }
}
