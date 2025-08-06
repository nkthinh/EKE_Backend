using System.ComponentModel.DataAnnotations;

namespace Service.DTO.Request
{
    public class SubscriptionPackageCreateDto
    {
        [Required(ErrorMessage = "Tên gói không được để trống")]
        [MaxLength(255, ErrorMessage = "Tên gói tối đa 255 ký tự")]
        public string Name { get; set; } = string.Empty;

        [Required(ErrorMessage = "Giá gói không được để trống")]
        [Range(0.01, double.MaxValue, ErrorMessage = "Giá gói phải lớn hơn 0")]
        public decimal Price { get; set; }

        [MaxLength(1000, ErrorMessage = "Mô tả tối đa 1000 ký tự")]
        public string Description { get; set; } = string.Empty;

        // Tính năng tùy chọn
        public bool HasPriorityMatching { get; set; }
        public bool HasAiAssistant { get; set; }
        public bool NoAds { get; set; }
    }
}
