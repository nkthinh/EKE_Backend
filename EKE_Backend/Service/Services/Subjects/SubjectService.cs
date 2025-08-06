using AutoMapper;
using Service.DTO.Request;
using Service.DTO.Response;
using Microsoft.Extensions.Logging;
using Repository.Entities;
using Repository.UnitOfWork;

    namespace Service.Services.Subjects
    {
    public class SubjectService : ISubjectService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ILogger<SubjectService> _logger;

        public SubjectService(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            ILogger<SubjectService> logger)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<IEnumerable<SubjectResponseDto>> GetAllSubjectsAsync()
        {
            try
            {
                var subjects = await _unitOfWork.Subjects.GetAllAsync();
                return _mapper.Map<IEnumerable<SubjectResponseDto>>(subjects);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting all subjects");
                throw;
            }
        }

        public async Task<IEnumerable<SubjectResponseDto>> GetActiveSubjectsAsync()
        {
            try
            {
                var subjects = await _unitOfWork.Subjects.GetActiveSubjectsAsync();
                return _mapper.Map<IEnumerable<SubjectResponseDto>>(subjects);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting active subjects");
                throw;
            }
        }

        public async Task<IEnumerable<SubjectResponseDto>> GetSubjectsByCategoryAsync(string category)
        {
            try
            {
                var subjects = await _unitOfWork.Subjects.GetSubjectsByCategoryAsync(category);
                return _mapper.Map<IEnumerable<SubjectResponseDto>>(subjects);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting subjects by category: {Category}", category);
                throw;
            }
        }

        public async Task<IEnumerable<string>> GetCategoriesAsync()
        {
            try
            {
                return await _unitOfWork.Subjects.GetCategoriesAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting subject categories");
                throw;
            }
        }

        public async Task<SubjectResponseDto?> GetSubjectByIdAsync(long id)
        {
            try
            {
                var subject = await _unitOfWork.Subjects.GetByIdAsync(id);
                return subject != null ? _mapper.Map<SubjectResponseDto>(subject) : null;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting subject by id: {SubjectId}", id);
                throw;
            }
        }

        public async Task<SubjectResponseDto> CreateSubjectAsync(SubjectCreateDto subjectCreateDto)
        {
            try
            {
                // Check if code already exists
                if (!string.IsNullOrEmpty(subjectCreateDto.Code) &&
                    await _unitOfWork.Subjects.CodeExistsAsync(subjectCreateDto.Code))
                {
                    throw new InvalidOperationException("Mã môn học đã tồn tại");
                }

                var subject = _mapper.Map<Subject>(subjectCreateDto);
                subject.CreatedAt = DateTime.UtcNow;
                subject.UpdatedAt = DateTime.UtcNow;

                await _unitOfWork.Subjects.AddAsync(subject);
                await _unitOfWork.CompleteAsync();

                return _mapper.Map<SubjectResponseDto>(subject);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating subject: {SubjectName}", subjectCreateDto.Name);
                throw;
            }
        }

        public async Task<SubjectResponseDto> UpdateSubjectAsync(long id, SubjectUpdateDto subjectUpdateDto)
        {
            try
            {
                var existingSubject = await _unitOfWork.Subjects.GetByIdAsync(id);
                if (existingSubject == null)
                {
                    throw new ArgumentException("Không tìm thấy môn học");
                }

                // Check if code is being changed and if new code already exists
                if (!string.IsNullOrEmpty(subjectUpdateDto.Code) &&
                    existingSubject.Code != subjectUpdateDto.Code &&
                    await _unitOfWork.Subjects.CodeExistsAsync(subjectUpdateDto.Code))
                {
                    throw new InvalidOperationException("Mã môn học đã tồn tại");
                }

                _mapper.Map(subjectUpdateDto, existingSubject);
                existingSubject.UpdatedAt = DateTime.UtcNow;

                _unitOfWork.Subjects.Update(existingSubject);
                await _unitOfWork.CompleteAsync();

                return _mapper.Map<SubjectResponseDto>(existingSubject);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating subject: {SubjectId}", id);
                throw;
            }
        }

        public async Task<bool> DeleteSubjectAsync(long id)
        {
            try
            {
                var subject = await _unitOfWork.Subjects.GetByIdAsync(id);
                if (subject == null) return false;

                // Soft delete
                subject.IsActive = false;
                subject.UpdatedAt = DateTime.UtcNow;

                _unitOfWork.Subjects.Update(subject);
                await _unitOfWork.CompleteAsync();

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting subject: {SubjectId}", id);
                throw;
            }
        }

        public async Task<bool> SubjectExistsAsync(long id)
        {
            try
            {
                return await _unitOfWork.Subjects.AnyAsync(s => s.Id == id && s.IsActive);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error checking if subject exists: {SubjectId}", id);
                throw;
            }
        }
    }
}

    