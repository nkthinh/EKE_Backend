using Application.DTOs;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Repository.Entities;
using Repository.Enums;
using Repository.Repositories.Tutors;
using Service.DTO.Request;
using Service.DTO.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Service.Services.Tutors
{
    public class TutorService : ITutorService
    {
        private readonly ITutorRepository _tutorRepository;
        private readonly IMapper _mapper;
        private readonly ILogger<TutorService> _logger;

        public TutorService(
            ITutorRepository tutorRepository,
            IMapper mapper,
            ILogger<TutorService> logger)
        {
            _tutorRepository = tutorRepository;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<(IEnumerable<TutorSearchResultDto> Tutors, int TotalCount)> SearchTutorsAsync(TutorSearchDto searchParams)
        {
            try
            {
                var (tutors, totalCount) = await _tutorRepository.SearchTutorsAsync(searchParams.Keyword, searchParams.Page, searchParams.PageSize);
                var tutorDtos = _mapper.Map<IEnumerable<TutorSearchResultDto>>(tutors); // Ánh xạ từ Tutor sang TutorSearchResultDto

                return (tutorDtos, totalCount);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error searching tutors with params: {SearchParams}", searchParams);
                throw;
            }
        }




        public async Task<TutorProfileDto?> GetTutorProfileAsync(long tutorId)
        {
            try
            {
                var tutor = await _tutorRepository.GetTutorWithDetailsAsync(tutorId);
                if (tutor == null) return null;

                var tutorProfileDto = _mapper.Map<TutorProfileDto>(tutor);
                tutorProfileDto.ProfileImage = tutor.User.ProfileImage; // Đảm bảo lấy ảnh từ User
                return tutorProfileDto;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting tutor profile for tutor ID: {TutorId}", tutorId);
                throw;
            }
        }

        public async Task<TutorProfileDto?> GetCompleteTutorProfileAsync(long tutorId)
        {
            try
            {
                var tutor = await _tutorRepository.GetTutorWithDetailsAsync(tutorId);
                if (tutor == null) return null;

                var tutorProfileDto = _mapper.Map<TutorProfileDto>(tutor);
                tutorProfileDto.ProfileImage = tutor.User.ProfileImage; // Đảm bảo lấy ảnh từ User

                return tutorProfileDto;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting complete tutor profile for tutor ID: {TutorId}", tutorId);
                throw;
            }
        }

        public async Task<string> UpdateAvailabilityAsync(long tutorId, TutorAvailabilityUpdateDto availabilityDto)
        {
            try
            {
                var result = await _tutorRepository.UpdateTutorAvailabilityAsync(tutorId, availabilityDto.Availability);
                return result == "Availability updated successfully" ? "Availability updated successfully" : "Failed to update availability";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating availability for tutor ID: {TutorId}", tutorId);
                throw;
            }
        }

        public async Task<IEnumerable<BookingScheduleDto>> GetTutorBookingsAsync(long tutorId, DateTime startDate, DateTime endDate)
        {
            try
            {
                var bookings = await _tutorRepository.GetTutorBookingsAsync(tutorId, startDate, endDate);
                return _mapper.Map<IEnumerable<BookingScheduleDto>>(bookings);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting tutor bookings for tutor ID: {TutorId}", tutorId);
                throw;
            }
        }

        public async Task<TutorProfileDto> UpdateTutorProfileAsync(long tutorId, TutorUpdateDto updateDto)
        {
            try
            {
                var updatedTutor = await _tutorRepository.UpdateTutorProfileAsync(tutorId, _mapper.Map<Tutor>(updateDto));
                return _mapper.Map<TutorProfileDto>(updatedTutor);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating tutor profile for tutor ID: {TutorId}", tutorId);
                throw;
            }
        }

        public async Task<bool> VerifyTutorAsync(long tutorId, TutorVerificationDto verificationDto)
        {
            try
            {
                return await _tutorRepository.VerifyTutorAsync(tutorId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error verifying tutor for tutor ID: {TutorId}", tutorId);
                throw;
            }
        }

        public async Task<bool> UpdateVerificationStatusAsync(long tutorId, TutorVerificationStatusDto statusDto)
        {
            try
            {
                return await _tutorRepository.UpdateVerificationStatusAsync(tutorId, statusDto.VerificationStatus);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating verification status for tutor ID: {TutorId}", tutorId);
                throw;
            }
        }

        public async Task<(IEnumerable<TutorSearchResultDto> Tutors, int TotalCount)> GetTutorsBySubjectAsync(long subjectId, int page, int pageSize)
        {
            try
            {
                var (tutors, totalCount) = await _tutorRepository.GetTutorsBySubjectAsync(subjectId, page, pageSize);
                var tutorDtos = _mapper.Map<IEnumerable<TutorSearchResultDto>>(tutors);
                return (tutorDtos, totalCount);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting tutors by subject ID: {SubjectId}", subjectId);
                throw;
            }
        }

        public async Task<IEnumerable<TutorSearchResultDto>> GetFeaturedTutorsAsync(int limit)
        {
            try
            {
                var tutors = await _tutorRepository.GetFeaturedTutorsAsync();
                return _mapper.Map<IEnumerable<TutorSearchResultDto>>(tutors);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting featured tutors");
                throw;
            }
        }

        public async Task<(IEnumerable<TutorSearchResultDto> Tutors, int TotalCount)> GetNearbyTutorsAsync(string city, string? district, int page, int pageSize)
        {
            try
            {
                var (tutors, totalCount) = await _tutorRepository.GetNearbyTutorsAsync(city, district, page, pageSize);
                var tutorDtos = _mapper.Map<IEnumerable<TutorSearchResultDto>>(tutors);
                return (tutorDtos, totalCount);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting nearby tutors");
                throw;
            }
        }

        //public async Task<TutorStatisticsDto> GetTutorStatisticsAsync(long tutorId)
        //{
        //    try
        //    {
        //        var stats = await _tutorRepository.GetTutorStatisticsAsync(tutorId);
        //        return _mapper.Map<TutorStatisticsDto>(stats);
        //    }
        //    catch (Exception ex)
        //    {
        //        _logger.LogError(ex, "Error getting tutor statistics for tutor ID: {TutorId}", tutorId);
        //        throw;
        //    }
        //}

        public async Task<(IEnumerable<TutorSearchResultDto> Tutors, int TotalCount)> GetTutorsPendingVerificationAsync(int page, int pageSize)
        {
            try
            {
                var (tutors, totalCount) = await _tutorRepository.GetTutorsPendingVerificationAsync(page, pageSize);
                var tutorDtos = _mapper.Map<IEnumerable<TutorSearchResultDto>>(tutors);
                return (tutorDtos, totalCount);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting tutors pending verification");
                throw;
            }
        }

        public async Task<(IEnumerable<DTO.Response.ReviewResponseDto> Reviews, int TotalCount, double AverageRating)> GetTutorReviewsAsync(long tutorId, int page, int pageSize)
        {
            try
            {
                var (reviews, totalCount, averageRating) = await _tutorRepository.GetTutorReviewsAsync(tutorId, page, pageSize);
                var reviewDtos = _mapper.Map<IEnumerable<DTO.Response.ReviewResponseDto>>(reviews);
                return (reviewDtos, totalCount, averageRating);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting tutor reviews for tutor ID: {TutorId}", tutorId);
                throw;
            }
        }

        public async Task<IEnumerable<TutorSearchResultDto>> GetRecommendedTutorsAsync(long studentId, int limit)
        {
            try
            {
                var tutors = await _tutorRepository.GetRecommendedTutorsAsync(studentId, limit);
                return _mapper.Map<IEnumerable<TutorSearchResultDto>>(tutors);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting recommended tutors for student ID: {StudentId}", studentId);
                throw;
            }
        }

        public async Task<(IEnumerable<TutorSearchResultDto> Tutors, int TotalCount)> GetAllTutorsAsync(int page, int pageSize, VerificationStatus? status)
        {
            try
            {
                var (tutors, totalCount) = await _tutorRepository.GetAllTutorsAsync(page, pageSize, status);
                var tutorDtos = _mapper.Map<IEnumerable<TutorSearchResultDto>>(tutors);
                return (tutorDtos, totalCount);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting all tutors");
                throw;
            }
        }

        public async Task<string> UploadProfileImageAsync(long tutorId, IFormFile imageFile)
        {
            try
            {
                var imageUrl = "image_url"; // Giả sử bạn lưu ảnh và trả về URL
                var result = await _tutorRepository.UploadProfileImageAsync(tutorId, imageUrl); // Lưu ảnh
                return imageUrl;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error uploading profile image for tutor ID: {TutorId}", tutorId);
                throw;
            }
        }
    }
}
