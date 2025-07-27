using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository.Enums
{
    public enum UserRole
    {
        Unspecified = 0,
        Student = 1,
        Tutor = 2,
        Admin = 3
    }

    public enum Gender
    {
        Male = 1,
        Female = 2,
        Other = 3
    }

    public enum SwipeActionType
    {
        Like = 1,
        Dislike = 2,
        SuperLike = 3
    }

    public enum MatchStatus
    {
        Active = 1,
        Inactive = 2,
        Blocked = 3,
        Pending=4,
        Rejected = 5
    }

    public enum MessageType
    {
        Text = 1,
        Image = 2,
        File = 3,
        System = 4
    }

    public enum BookingStatus
    {
        Pending = 1,
        Confirmed = 2,
        Completed = 3,
        Cancelled = 4
    }

    public enum LocationType
    {
        Online = 1,
        TutorPlace = 2,
        StudentPlace = 3,
        PublicPlace = 4
    }

 

    public enum VerificationStatus
    {
        Pending = 1,
        Verified = 2,
        Rejected = 3
    }

    public enum ProficiencyLevel
    {
        Beginner = 1,
        Intermediate = 2,
        Advanced = 3,
        Expert = 4
    }

    public enum LearningStyle
    {
        Visual = 1,
        Auditory = 2,
        Kinesthetic = 3,
        Reading = 4
    }

    public enum SenderType
    {
        User = 1,
        Ai = 2
    }
    public enum PaymentMethod
    {
        Wallet = 1,      // Ví nội bộ
        VNPay = 2,       // VNPay
        Momo = 3,        // Momo
        BankTransfer = 4, // Chuyển khoản ngân hàng
        Cash = 5         // Tiền mặt (cho offline)
    }

    public enum PaymentStatus
    {
        Pending = 1,     // Đang chờ xử lý
        Processing = 2,   // Đang xử lý
        Completed = 3,    // Hoàn thành
        Failed = 4,       // Thất bại
        Cancelled = 5,    // Đã hủy
        Refunded = 6      // Đã hoàn tiền
    }

    public enum TransactionType
    {
        Deposit = 1,      // Nạp tiền
        Withdraw = 2,     // Rút tiền
        Payment = 3,      // Thanh toán
        Refund = 4,       // Hoàn tiền
        Commission = 5,   // Hoa hồng
        Transfer = 6,     // Chuyển tiền
        Bonus = 7,        // Thưởng
        Penalty = 8       // Phạt
    }

    public enum TransactionStatus
    {
        Pending = 1,      // Đang chờ
        Completed = 2,    // Hoàn thành
        Failed = 3,       // Thất bại
        Cancelled = 4     // Đã hủy
    }

    public enum NotificationType
    {
        BookingCreated = 1,
        BookingConfirmed = 2,
        BookingCancelled = 3,
        BookingCompleted = 4,
        BookingReminder = 5,
        PaymentReceived = 6,
        PaymentFailed = 7,
        ReviewReceived = 8,
        TutorVerified = 9,
        MessageReceived = 10,
        MatchFound = 11,
        SystemAnnouncement = 12,
        PromotionAlert = 13
    }
}

