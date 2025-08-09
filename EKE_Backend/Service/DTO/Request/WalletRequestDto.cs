namespace Service.DTO.Request
{
    // Dùng cho Admin
    public class WalletRequestDto
    {
        public long UserId { get; set; }
        public decimal Balance { get; set; }
    }

    // Dùng cho user hiện tại (UserId sẽ lấy từ token)
    public class WalletSelfUpdateRequestDto
    {
        public decimal Balance { get; set; }
    }
}
