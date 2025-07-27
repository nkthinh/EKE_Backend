
using AutoMapper;
using Microsoft.Extensions.Logging;
using Repository.Entities;
using Repository.UnitOfWork;
using Service.DTO.Request;
using Service.DTO.Response;
using Service.Firebase;

namespace Service.Services.Certifications
{   public class CertificationService : ICertificationService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ILogger<CertificationService> _logger;
        private readonly IFirebaseStorageService _firebaseStorageService;

        public CertificationService(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            ILogger<CertificationService> logger,
            IFirebaseStorageService firebaseStorageService)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _logger = logger;
            _firebaseStorageService = firebaseStorageService;
        }

        public async Task<IEnumerable<CertificationResponseDto>> GetCertificationsByTutorIdAsync(long tutorId)
        {
            try
            {
                var certifications = await _unitOfWork.Certifications.GetByTutorIdAsync(tutorId);
                return _mapper.Map<IEnumerable<CertificationResponseDto>>(certifications);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting certifications for tutor: {TutorId}", tutorId);
                throw;
            }
        }

        public async Task<CertificationResponseDto?> GetCertificationByIdAsync(long id)
        {
            try
            {
                var certification = await _unitOfWork.Certifications.GetByIdAsync(id);
                return certification != null ? _mapper.Map<CertificationResponseDto>(certification) : null;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting certification by id: {CertificationId}", id);
                throw;
            }
        }

        public async Task<CertificationResponseDto> CreateCertificationAsync(long tutorId, CertificationCreateDto certificationDto)
        {
            try
            {
                // Check if tutor exists
                var tutorExists = await _unitOfWork.Tutors.AnyAsync(t => t.Id == tutorId);
                if (!tutorExists)
                {
                    throw new ArgumentException("Không tìm thấy gia sư");
                }

                // Check if tutor already has this certification
                if (await _unitOfWork.Certifications.TutorHasCertificationAsync(tutorId, certificationDto.Name))
                {
                    throw new InvalidOperationException("Gia sư đã có chứng chỉ này");
                }

                var certification = _mapper.Map<Certification>(certificationDto);
                certification.TutorId = tutorId;
                certification.CreatedAt = DateTime.UtcNow;
                certification.UpdatedAt = DateTime.UtcNow;

                // Upload certificate file if provided
                if (certificationDto.CertificateFile != null)
                {
                    var certificateUrl = await _firebaseStorageService.UploadImageAsync(
                        certificationDto.CertificateFile, "certifications", tutorId);
                    certification.CertificateUrl = certificateUrl;
                }

                await _unitOfWork.Certifications.AddAsync(certification);
                await _unitOfWork.CompleteAsync();

                return _mapper.Map<CertificationResponseDto>(certification);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating certification for tutor: {TutorId}", tutorId);
                throw;
            }
        }

        public async Task<CertificationResponseDto> UpdateCertificationAsync(long id, CertificationUpdateDto certificationDto)
        {
            try
            {
                var existingCertification = await _unitOfWork.Certifications.GetByIdAsync(id);
                if (existingCertification == null)
                {
                    throw new ArgumentException("Không tìm thấy chứng chỉ");
                }

                // Update basic information
                _mapper.Map(certificationDto, existingCertification);
                existingCertification.UpdatedAt = DateTime.UtcNow;

                // Upload new certificate file if provided
                if (certificationDto.CertificateFile != null)
                {
                    // Delete old file if exists
                    if (!string.IsNullOrEmpty(existingCertification.CertificateUrl))
                    {
                        await _firebaseStorageService.DeleteImageAsync(existingCertification.CertificateUrl);
                    }

                    // Upload new file
                    var certificateUrl = await _firebaseStorageService.UploadImageAsync(
                        certificationDto.CertificateFile, "certifications", existingCertification.TutorId);
                    existingCertification.CertificateUrl = certificateUrl;

                    // Reset verification status when certificate is updated
                    existingCertification.IsVerified = false;
                }

                _unitOfWork.Certifications.Update(existingCertification);
                await _unitOfWork.CompleteAsync();

                return _mapper.Map<CertificationResponseDto>(existingCertification);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating certification: {CertificationId}", id);
                throw;
            }
        }

        public async Task<bool> DeleteCertificationAsync(long id)
        {
            try
            {
                var certification = await _unitOfWork.Certifications.GetByIdAsync(id);
                if (certification == null) return false;

                // Delete certificate file if exists
                if (!string.IsNullOrEmpty(certification.CertificateUrl))
                {
                    await _firebaseStorageService.DeleteImageAsync(certification.CertificateUrl);
                }

                _unitOfWork.Certifications.Remove(certification);
                await _unitOfWork.CompleteAsync();

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting certification: {CertificationId}", id);
                throw;
            }
        }

        public async Task<bool> VerifyCertificationAsync(long id)
        {
            try
            {
                var certification = await _unitOfWork.Certifications.GetByIdAsync(id);
                if (certification == null) return false;

                certification.IsVerified = true;
                certification.UpdatedAt = DateTime.UtcNow;

                _unitOfWork.Certifications.Update(certification);
                await _unitOfWork.CompleteAsync();

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error verifying certification: {CertificationId}", id);
                throw;
            }
        }

        public async Task<IEnumerable<CertificationResponseDto>> GetPendingVerificationAsync()
        {
            try
            {
                var certifications = await _unitOfWork.Certifications.GetPendingVerificationAsync();
                return _mapper.Map<IEnumerable<CertificationResponseDto>>(certifications);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting pending verification certifications");
                throw;
            }
        }
    }
}