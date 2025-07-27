using AutoMapper;
using Microsoft.Extensions.Logging;
using Repository.Entities;
using Repository.UnitOfWork;
using Service.DTO.Request;
using Service.DTO.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.Services.TutorSubjects
{
    public class TutorSubjectService : ITutorSubjectService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ILogger<TutorSubjectService> _logger;

        public TutorSubjectService(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            ILogger<TutorSubjectService> logger)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<IEnumerable<TutorSubjectDto>> GetTutorSubjectsAsync(long tutorId)
        {
            try
            {
                var tutorSubjects = await _unitOfWork.TutorSubjects.GetTutorSubjectsWithDetailsAsync(tutorId);
                return _mapper.Map<IEnumerable<TutorSubjectDto>>(tutorSubjects);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting tutor subjects for tutor: {TutorId}", tutorId);
                throw;
            }
        }

        public async Task<TutorSubjectDto> AddTutorSubjectAsync(long tutorId, TutorSubjectCreateDto tutorSubjectDto)
        {
            try
            {
                // Check if tutor exists
                var tutorExists = await _unitOfWork.Tutors.AnyAsync(t => t.Id == tutorId);
                if (!tutorExists)
                {
                    throw new ArgumentException("Không tìm thấy gia sư");
                }

                // Check if subject exists
                var subjectExists = await _unitOfWork.Subjects.AnyAsync(s => s.Id == tutorSubjectDto.SubjectId && s.IsActive);
                if (!subjectExists)
                {
                    throw new ArgumentException("Không tìm thấy môn học");
                }

                // Check if tutor already has this subject
                var existingTutorSubject = await _unitOfWork.TutorSubjects
                    .FirstOrDefaultAsync(ts => ts.TutorId == tutorId && ts.SubjectId == tutorSubjectDto.SubjectId);

                if (existingTutorSubject != null)
                {
                    throw new InvalidOperationException("Gia sư đã có môn học này");
                }

                var tutorSubject = _mapper.Map<TutorSubject>(tutorSubjectDto);
                tutorSubject.TutorId = tutorId;
                tutorSubject.CreatedAt = DateTime.UtcNow;
                tutorSubject.UpdatedAt = DateTime.UtcNow;

                await _unitOfWork.TutorSubjects.AddAsync(tutorSubject);
                await _unitOfWork.CompleteAsync();

                // Get the created tutor subject with details
                var createdTutorSubject = await _unitOfWork.TutorSubjects.GetTutorSubjectsWithDetailsAsync(tutorSubject.Id);
                return _mapper.Map<TutorSubjectDto>(createdTutorSubject);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error adding tutor subject for tutor: {TutorId}", tutorId);
                throw;
            }
        }

        public async Task<TutorSubjectDto> UpdateTutorSubjectAsync(long tutorId, long subjectId, TutorSubjectUpdateDto tutorSubjectDto)
        {
            try
            {
                var existingTutorSubject = await _unitOfWork.TutorSubjects
                    .FirstOrDefaultAsync(ts => ts.TutorId == tutorId && ts.SubjectId == subjectId);

                if (existingTutorSubject == null)
                {
                    throw new ArgumentException("Không tìm thấy môn học của gia sư");
                }

                _mapper.Map(tutorSubjectDto, existingTutorSubject);
                existingTutorSubject.UpdatedAt = DateTime.UtcNow;

                _unitOfWork.TutorSubjects.Update(existingTutorSubject);
                await _unitOfWork.CompleteAsync();

                var updatedTutorSubject = await _unitOfWork.TutorSubjects.GetTutorSubjectsWithDetailsAsync(existingTutorSubject.Id);
                return _mapper.Map<TutorSubjectDto>(updatedTutorSubject);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating tutor subject: TutorId={TutorId}, SubjectId={SubjectId}", tutorId, subjectId);
                throw;
            }
        }

        public async Task<bool> RemoveTutorSubjectAsync(long tutorId, long subjectId)
        {
            try
            {
                var tutorSubject = await _unitOfWork.TutorSubjects
                    .FirstOrDefaultAsync(ts => ts.TutorId == tutorId && ts.SubjectId == subjectId);

                if (tutorSubject == null) return false;

                _unitOfWork.TutorSubjects.Remove(tutorSubject);
                await _unitOfWork.CompleteAsync();

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error removing tutor subject: TutorId={TutorId}, SubjectId={SubjectId}", tutorId, subjectId);
                throw;
            }
        }

        public async Task<bool> TutorHasSubjectAsync(long tutorId, long subjectId)
        {
            try
            {
                return await _unitOfWork.TutorSubjects
                    .AnyAsync(ts => ts.TutorId == tutorId && ts.SubjectId == subjectId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error checking if tutor has subject: TutorId={TutorId}, SubjectId={SubjectId}", tutorId, subjectId);
                throw;
            }
        }

        public Task<TutorSubject?> GetByTutorAndSubjectIdAsync(long tutorId, long subjectId)
        {
            throw new NotImplementedException();
        }
    }
}