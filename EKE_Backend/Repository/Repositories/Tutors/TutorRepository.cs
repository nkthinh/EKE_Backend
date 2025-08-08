using Microsoft.EntityFrameworkCore;
using Repository.Entities;
using Repository.Enums;
using Repository.Repositories.BaseRepository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Repository.Repositories.Tutors
{
    public class TutorRepository : BaseRepository<Tutor>, ITutorRepository
    {
        public TutorRepository(ApplicationDbContext context) : base(context) { }

        // Implement the method to get TutorId from UserId
        public async Task<long?> GetTutorIdByUserIdAsync(long userId)
        {
            var tutor = await _dbSet
                .Where(s => s.UserId == userId)
                .Select(s => s.Id)  // Chỉ lấy Id của Student
                .FirstOrDefaultAsync();

            return tutor; // Trả về StudentId nếu tìm thấy, nếu không trả về null
        }

        public async Task<Tutor?> GetTutorWithDetailsAsync(long tutorId)
        {
            return await _dbSet
                .Include(t => t.User) // Đảm bảo bao gồm User để lấy ProfileImage
                .Include(t => t.TutorSubjects)
                    .ThenInclude(ts => ts.Subject)
                .Include(t => t.Certifications)
                .Include(t => t.ReviewsReceived.Take(5)) // Latest 5 reviews
                .FirstOrDefaultAsync(t => t.Id == tutorId);
        }

        public async Task<IEnumerable<Tutor>> GetTutorsWithDetailsAsync()
        {
            return await _dbSet
                .Include(t => t.User)
                .Include(t => t.TutorSubjects)
                    .ThenInclude(ts => ts.Subject)
                .Where(t => t.User.IsActive)
                .OrderByDescending(t => t.AverageRating)
                .ToListAsync();
        }

        public async Task<IEnumerable<Tutor>> GetVerifiedTutorsAsync()
        {
            return await _dbSet
                .Include(t => t.User)
                .Include(t => t.TutorSubjects)
                    .ThenInclude(ts => ts.Subject)
                .Where(t => t.VerificationStatus == VerificationStatus.Verified && t.User.IsActive)
                .OrderByDescending(t => t.AverageRating)
                .ToListAsync();
        }

        public async Task<IEnumerable<Tutor>> GetFeaturedTutorsAsync()
        {
            return await _dbSet
                .Include(t => t.User)
                .Include(t => t.TutorSubjects)
                    .ThenInclude(ts => ts.Subject)
                .Where(t => t.IsFeatured && t.User.IsActive)
                .OrderByDescending(t => t.AverageRating)
                .ToListAsync();
        }

        public async Task<Tutor?> GetTutorByUserIdAsync(long userId)
        {
            return await _dbSet
                .Include(t => t.User)
                .Include(t => t.TutorSubjects)
                    .ThenInclude(ts => ts.Subject)
                .Include(t => t.Certifications)
                .FirstOrDefaultAsync(t => t.UserId == userId);
        }

        public async Task<(IEnumerable<Tutor> Tutors, int TotalCount)> GetTutorsBySubjectAsync(long subjectId, int page, int pageSize)
        {
            var query = _dbSet
                .Include(t => t.User)
                .Include(t => t.TutorSubjects)
                    .ThenInclude(ts => ts.Subject)
                .Where(t => t.TutorSubjects.Any(ts => ts.SubjectId == subjectId) && t.User.IsActive);

            var totalCount = await query.CountAsync();
            var tutors = await query.Skip((page - 1) * pageSize).Take(pageSize).ToListAsync();

            return (tutors, totalCount);
        }

        public async Task<(IEnumerable<Tutor> Tutors, int TotalCount)> GetNearbyTutorsAsync(string city, string? district, int page, int pageSize)
        {
            var query = _dbSet
                .Include(t => t.User)
                .Include(t => t.TutorSubjects)
                    .ThenInclude(ts => ts.Subject)
                .Where(t => t.User.City == city && (district == null || t.User.District == district));

            var totalCount = await query.CountAsync();
            var tutors = await query.Skip((page - 1) * pageSize).Take(pageSize).ToListAsync();

            return (tutors, totalCount);
        }

        public async Task<IEnumerable<Tutor>> GetTutorsByRatingAsync(decimal minRating)
        {
            return await _dbSet
                .Include(t => t.User)
                .Where(t => t.AverageRating >= minRating && t.User.IsActive)
                .OrderByDescending(t => t.AverageRating)
                .ToListAsync();
        }

        public async Task<IEnumerable<Tutor>> GetAvailableTutorsForMatchingAsync(long studentId, IEnumerable<long> excludedTutorIds, int count)
        {
            var query = _dbSet
                .Include(t => t.User)
                .Include(t => t.TutorSubjects)
                    .ThenInclude(ts => ts.Subject)
                .Where(t => t.User.IsActive && t.VerificationStatus == VerificationStatus.Verified);

            if (excludedTutorIds.Any())
            {
                query = query.Where(t => !excludedTutorIds.Contains(t.Id));
            }

            return await query
                .OrderByDescending(t => t.AverageRating)
                .ThenByDescending(t => t.TotalReviews)
                .Take(count)
                .ToListAsync();
        }

        public async Task<(IEnumerable<Tutor> Tutors, int TotalCount)> SearchTutorsAsync(string keyword, int page, int pageSize)
        {
            var query = _dbSet
                .Include(t => t.User)  // Bao gồm cả thông tin User
                .Include(t => t.TutorSubjects)
                    .ThenInclude(ts => ts.Subject)
                .AsQueryable();

            if (!string.IsNullOrEmpty(keyword))
            {
                query = query.Where(t => t.Major.Contains(keyword));
            }

            var totalCount = await query.CountAsync();
            var tutors = await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return (tutors, totalCount);
        }
        public async Task<Tutor?> GetTutotWithUserInfoAsync(long tutorId)
        {
            return await _dbSet
                .Include(s => s.User)
                .FirstOrDefaultAsync(s => s.Id == tutorId);
        }

        public async Task<string> UpdateTutorAvailabilityAsync(long tutorId, string availability)
        {
            var tutor = await _dbSet.FirstOrDefaultAsync(t => t.Id == tutorId);
            if (tutor == null) return "Tutor not found";

            tutor.Availability = availability;
            await _context.SaveChangesAsync();
            return "Availability updated successfully";
        }

        public async Task<IEnumerable<Booking>> GetTutorBookingsAsync(long tutorId, DateTime startDate, DateTime endDate)
        {
            var bookings = await _context.Set<Booking>()
                .Where(b => b.TutorId == tutorId && b.StartTime >= startDate && b.EndTime <= endDate)
                .ToListAsync();

            return bookings;
        }

        public async Task<Tutor> UpdateTutorProfileAsync(long tutorId, Tutor updateData)
        {
            var tutor = await _dbSet.FirstOrDefaultAsync(t => t.Id == tutorId);
            if (tutor == null) throw new ArgumentException("Tutor not found");

            tutor.ExperienceYears = updateData.ExperienceYears;
            tutor.EducationLevel = updateData.EducationLevel;
            tutor.University = updateData.University;
            tutor.Major = updateData.Major;
            tutor.HourlyRate = updateData.HourlyRate;
            tutor.Introduction = updateData.Introduction;

            await _context.SaveChangesAsync();
            return tutor;
        }

        public async Task<bool> VerifyTutorAsync(long tutorId)
        {
            var tutor = await _dbSet.FirstOrDefaultAsync(t => t.Id == tutorId);
            if (tutor == null) return false;

            tutor.VerificationStatus = VerificationStatus.Verified;
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> UpdateVerificationStatusAsync(long tutorId, VerificationStatus status)
        {
            var tutor = await _dbSet.FirstOrDefaultAsync(t => t.Id == tutorId);
            if (tutor == null) return false;

            tutor.VerificationStatus = status;
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<(IEnumerable<Tutor> Tutors, int TotalCount)> GetTutorsPendingVerificationAsync(int page, int pageSize)
        {
            var query = _dbSet
                .Where(t => t.VerificationStatus == VerificationStatus.Pending)
                .AsQueryable();

            var totalCount = await query.CountAsync();
            var tutors = await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return (tutors, totalCount);
        }

        public async Task<(IEnumerable<Review> Reviews, int TotalCount, double AverageRating)> GetTutorReviewsAsync(long tutorId, int page, int pageSize)
        {
            // Truy vấn bảng Review thay vì Tutor
            var query = _context.Set<Review>()
                                .Where(r => r.TutorId == tutorId);

            // Lấy tổng số reviews
            var totalCount = await query.CountAsync();

            // Lấy reviews với phân trang
            var reviews = await query
                                .Skip((page - 1) * pageSize)
                                .Take(pageSize)
                                .ToListAsync();

            // Tính average rating từ các review
            double averageRating = reviews.Any() ? (double)reviews.Average(r => r.Rating) : 0;

            // Trả về kết quả dưới dạng tuple
            return (reviews, totalCount, averageRating);
        }



        public async Task<IEnumerable<Tutor>> GetRecommendedTutorsAsync(long studentId, int limit)
        {
            return await _dbSet
                .Where(t => t.User.IsActive)
                .OrderByDescending(t => t.AverageRating)
                .Take(limit)
                .ToListAsync();
        }

        public async Task<(IEnumerable<Tutor> Tutors, int TotalCount)> GetAllTutorsAsync(int page, int pageSize, VerificationStatus? status)
        {
            var query = _dbSet.AsQueryable();

            if (status.HasValue)
            {
                query = query.Where(t => t.VerificationStatus == status.Value);
            }

            var totalCount = await query.CountAsync();
            var tutors = await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return (tutors, totalCount);
        }

        public async Task<string> UploadProfileImageAsync(long tutorId, string imageUrl)
        {
            var tutor = await _dbSet.FirstOrDefaultAsync(t => t.Id == tutorId);
            if (tutor == null) throw new ArgumentException("Tutor not found");

            // Cập nhật ảnh đại diện của User thay vì Tutor
            tutor.User.ProfileImage = imageUrl;
            await _context.SaveChangesAsync();
            return imageUrl;
        }
    }
}
