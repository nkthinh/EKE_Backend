using Repository.Entities;
using Repository.Enums;
using Repository.Repositories.BaseRepository;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Repository.Repositories.Tutors
{
    public interface ITutorRepository : IBaseRepository<Tutor>
    {
        // Lấy thông tin chi tiết của một tutor, bao gồm cả thông tin User và các thông tin liên quan
        Task<Tutor?> GetTutorWithDetailsAsync(long tutorId);

        // Lấy danh sách tất cả tutors có chi tiết (bao gồm User và các thông tin liên quan)
        Task<IEnumerable<Tutor>> GetTutorsWithDetailsAsync();

        // Lấy danh sách các tutors đã xác minh
        Task<IEnumerable<Tutor>> GetVerifiedTutorsAsync();

        // Lấy danh sách các tutors nổi bật
        Task<IEnumerable<Tutor>> GetFeaturedTutorsAsync();

        // Lấy thông tin tutor dựa trên UserId
        Task<Tutor?> GetTutorByUserIdAsync(long userId);

        // Lấy các tutor theo môn học (kèm theo phân trang)
        Task<(IEnumerable<Tutor> Tutors, int TotalCount)> GetTutorsBySubjectAsync(long subjectId, int page, int pageSize);

        // Lấy danh sách các tutor có đánh giá từ một mức tối thiểu (minRating)
        Task<IEnumerable<Tutor>> GetTutorsByRatingAsync(decimal minRating);
        Task<(IEnumerable<Review> Reviews, int TotalCount, double AverageRating)> GetTutorReviewsAsync(long tutorId, int page, int pageSize);


        // Lấy danh sách các tutor sẵn sàng cho việc matching với học sinh, loại trừ các tutor không cần thiết
        Task<IEnumerable<Tutor>> GetAvailableTutorsForMatchingAsync(long studentId, IEnumerable<long> excludedTutorIds, int count);

        // Lấy danh sách các tutor gần học sinh (theo thành phố và quận, với phân trang)
        Task<(IEnumerable<Tutor> Tutors, int TotalCount)> GetNearbyTutorsAsync(string city, string? district, int page, int pageSize);

        // Tìm kiếm tutor dựa trên từ khóa (một số tiêu chí như Major) và phân trang
        Task<(IEnumerable<Tutor> Tutors, int TotalCount)> SearchTutorsAsync(string keyword, int page, int pageSize);

        // Cập nhật sự sẵn sàng (availability) của tutor
        Task<string> UpdateTutorAvailabilityAsync(long tutorId, string availability);

        // Lấy danh sách các booking của một tutor trong khoảng thời gian
        Task<IEnumerable<Booking>> GetTutorBookingsAsync(long tutorId, DateTime startDate, DateTime endDate);

        // Cập nhật thông tin của tutor
        Task<Tutor> UpdateTutorProfileAsync(long tutorId, Tutor updateData);

        // Xác minh một tutor
        Task<bool> VerifyTutorAsync(long tutorId);

        // Cập nhật trạng thái xác minh của tutor
        Task<bool> UpdateVerificationStatusAsync(long tutorId, VerificationStatus status);

        // Lấy danh sách các tutor đang chờ xác minh (với phân trang)
        Task<(IEnumerable<Tutor> Tutors, int TotalCount)> GetTutorsPendingVerificationAsync(int page, int pageSize);

  

        // Lấy danh sách các tutor được đề xuất cho học sinh (với số lượng giới hạn)
        Task<IEnumerable<Tutor>> GetRecommendedTutorsAsync(long studentId, int limit);

        // Lấy tất cả tutor (với phân trang và trạng thái xác minh nếu có)
        Task<(IEnumerable<Tutor> Tutors, int TotalCount)> GetAllTutorsAsync(int page, int pageSize, VerificationStatus? status);

        // Tải và lưu ảnh đại diện cho tutor
        Task<string> UploadProfileImageAsync(long tutorId, string imageUrl);
    }
}
