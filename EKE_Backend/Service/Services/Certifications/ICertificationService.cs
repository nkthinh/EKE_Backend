using Service.DTO.Request;
using Service.DTO.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.Services.Certifications
{
    public interface ICertificationService
    {
        Task<IEnumerable<CertificationResponseDto>> GetCertificationsByTutorIdAsync(long tutorId);
        Task<CertificationResponseDto?> GetCertificationByIdAsync(long id);
        Task<CertificationResponseDto> CreateCertificationAsync(long tutorId, CertificationCreateDto certificationDto);
        Task<CertificationResponseDto> UpdateCertificationAsync(long id, CertificationUpdateDto certificationDto);
        Task<bool> DeleteCertificationAsync(long id);
        Task<bool> VerifyCertificationAsync(long id);
        Task<IEnumerable<CertificationResponseDto>> GetPendingVerificationAsync();
    }
}
