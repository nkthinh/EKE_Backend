using Application.DTOs;
using AutoMapper;
using Microsoft.Extensions.Logging;
using Repository.Entities;
using Repository.Repositories.Students;
using Repository.UnitOfWork;
using Service.DTO.Request;
using Service.DTO.Response;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Service.Services.Students
{
    public class StudentService : IStudentService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ILogger<StudentService> _logger;

        public StudentService(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            ILogger<StudentService> logger)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<StudentDto> GetStudentProfileAsync(long studentId)
        {
            try
            {
                var student = await _unitOfWork.Students.GetStudentWithUserInfoAsync(studentId);
                if (student == null) return null;

                var studentDto = _mapper.Map<StudentDto>(student);
                return studentDto;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting student profile for student ID: {StudentId}", studentId);
                throw;
            }
        }

        public async Task<IEnumerable<StudentDto>> GetAllStudentsAsync(int page, int pageSize)
        {
            try
            {
                var students = await _unitOfWork.Students.GetStudentsWithUserInfoAsync();
                var studentDtos = _mapper.Map<IEnumerable<StudentDto>>(students);
                return studentDtos;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting all students");
                throw;
            }
        }

        public async Task<StudentDto> UpdateStudentProfileAsync(long studentId, StudentUpdateDto updateDto)
        {
            try
            {
                var student = await _unitOfWork.Students.GetByUserIdAsync(studentId);
                if (student == null) return null;

                _mapper.Map(updateDto, student); // Ánh xạ từ DTO sang Entity

                await _unitOfWork.Students.UpdateAsync(student); // Cập nhật dữ liệu vào DB
                var updatedStudentDto = _mapper.Map<StudentDto>(student);
                return updatedStudentDto;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating student profile for student ID: {StudentId}", studentId);
                throw;
            }
        }

        public async Task<bool> VerifyStudentAsync(long studentId)
        {
            try
            {
                var student = await _unitOfWork.Students.GetStudentWithUserInfoAsync(studentId);
                if (student == null) return false;

                // Thực hiện kiểm tra xác thực student (logic này sẽ tùy thuộc vào yêu cầu của bạn)
                student.User.IsVerified = true; // Ví dụ: đánh dấu học sinh đã được xác thực
                await _unitOfWork.Students.UpdateAsync(student);

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error verifying student for student ID: {StudentId}", studentId);
                throw;
            }
        }
    }
}
