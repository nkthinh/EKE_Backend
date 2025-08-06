
using Repository.Enums; // Import để dùng Wallet, PaymentTransaction

namespace Repository.Entities
{
    public class User : BaseEntity
    {
        public string Email { get; set; } = string.Empty;
        public string PasswordHash { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public string? Phone { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public Gender? Gender { get; set; }
        public UserRole? Role { get; set; }
        public string? ProfileImage { get; set; }
        public string? Address { get; set; }
        public string? City { get; set; }
        public string? District { get; set; }
        public string? Bio { get; set; }
        public bool IsVerified { get; set; } = false;
        public bool IsActive { get; set; } = true;

        // Navigation Properties
        public Student? Student { get; set; }
        public Tutor? Tutor { get; set; }
        public ICollection<Message> Messages { get; set; } = new List<Message>();
        public ICollection<Notification> Notifications { get; set; } = new List<Notification>();
        public ICollection<AiChatSession> AiChatSessions { get; set; } = new List<AiChatSession>();

        // 🔁 PayOS Integration
        public Wallet? Wallet { get; set; }
        public ICollection<PaymentTransaction> PaymentTransactions { get; set; } = new List<PaymentTransaction>();

        public long? SubscriptionPackageId { get; set; }  // Gói hiện tại
        public SubscriptionPackage? SubscriptionPackage { get; set; }

    }
}
