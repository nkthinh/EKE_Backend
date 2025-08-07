using Application.DTOs;
using Service.DTO.Request;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Service.Services.Students
{
    public interface IStudentService
    {
        Task<StudentDto> GetStudentProfileAsync(long studentId);
        Task<IEnumerable<StudentDto>> GetAllStudentsAsync(int page, int pageSize);
        Task<StudentDto> UpdateStudentProfileAsync(long studentId, StudentUpdateDto updateDto);
        Task<bool> VerifyStudentAsync(long studentId);

    }
}
