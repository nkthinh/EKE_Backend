using Service.DTO.Request;
using Service.DTO.Response;

namespace Service.Services
{
    public interface ISubscriptionPackageService
    {
        Task<IEnumerable<SubscriptionPackageResponseDto>> GetAllPackagesAsync();
        Task<SubscriptionPackageResponseDto?> GetPackageByIdAsync(long id);
        Task<SubscriptionPackageResponseDto> CreatePackageAsync(SubscriptionPackageCreateDto packageDto);
        Task<SubscriptionPackageResponseDto?> UpdatePackageAsync(long id, SubscriptionPackageUpdateDto packageDto);
        Task<bool> DeletePackageAsync(long id);

        // Mua gói & trừ tiền trong Wallet
        Task<bool> PurchasePackageAsync(long userId, long packageId);

        Task<SubscriptionPackageResponseDto?> GetCurrentPackageAsync(long userId);
    }
}
