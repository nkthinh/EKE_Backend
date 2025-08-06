namespace Repository.Entities
{
    public class SubscriptionPackage : BaseEntity
    {
        public string Name { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public string Description { get; set; } = string.Empty;

        // Tính năng (tùy chọn)
        public bool HasPriorityMatching { get; set; } // Ví dụ: Tăng giới hạn kết nối
        public bool HasAiAssistant { get; set; }      // Trợ lý học tập AI
        public bool NoAds { get; set; }               // Không hiển thị quảng cáo

        public ICollection<User> Users { get; set; } = new List<User>();
    }

}