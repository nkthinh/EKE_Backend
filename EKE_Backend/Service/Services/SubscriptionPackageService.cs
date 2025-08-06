using AutoMapper;
using Microsoft.Extensions.Logging;
using Repository.Entities;
using Repository.UnitOfWork;
using Service.DTO.Request;
using Service.DTO.Response;

namespace Service.Services.SubscriptionPackages
{
    public class SubscriptionPackageService : ISubscriptionPackageService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ILogger<SubscriptionPackageService> _logger;

        public SubscriptionPackageService(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            ILogger<SubscriptionPackageService> logger)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<IEnumerable<SubscriptionPackageResponseDto>> GetAllPackagesAsync()
        {
            try
            {
                var packages = await _unitOfWork.SubscriptionPackages.GetAllAsync();
                return _mapper.Map<IEnumerable<SubscriptionPackageResponseDto>>(packages);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting subscription packages");
                throw;
            }
        }

        public async Task<SubscriptionPackageResponseDto?> GetPackageByIdAsync(long id)
        {
            try
            {
                var package = await _unitOfWork.SubscriptionPackages.GetByIdAsync(id);
                return package == null ? null : _mapper.Map<SubscriptionPackageResponseDto>(package);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting subscription package {Id}", id);
                throw;
            }
        }

        public async Task<SubscriptionPackageResponseDto> CreatePackageAsync(SubscriptionPackageCreateDto packageDto)
        {
            try
            {
                var package = _mapper.Map<SubscriptionPackage>(packageDto);
                await _unitOfWork.SubscriptionPackages.AddAsync(package);
                await _unitOfWork.CompleteAsync();
                return _mapper.Map<SubscriptionPackageResponseDto>(package);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating subscription package");
                throw;
            }
        }

        public async Task<SubscriptionPackageResponseDto?> UpdatePackageAsync(long id, SubscriptionPackageUpdateDto packageDto)
        {
            try
            {
                var package = await _unitOfWork.SubscriptionPackages.GetByIdAsync(id);
                if (package == null) return null;

                _mapper.Map(packageDto, package);
                _unitOfWork.SubscriptionPackages.Update(package);
                await _unitOfWork.CompleteAsync();

                return _mapper.Map<SubscriptionPackageResponseDto>(package);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating subscription package {Id}", id);
                throw;
            }
        }

        public async Task<bool> DeletePackageAsync(long id)
        {
            try
            {
                var deleted = await _unitOfWork.SubscriptionPackages.RemoveByIdAsync(id);
                if (!deleted) return false;

                await _unitOfWork.CompleteAsync();
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting subscription package {Id}", id);
                throw;
            }
        }

        public async Task<bool> PurchasePackageAsync(long userId, long packageId)
        {
            try
            {
                await _unitOfWork.BeginTransactionAsync();

                var package = await _unitOfWork.SubscriptionPackages.GetByIdAsync(packageId);
                if (package == null) throw new ArgumentException("Gói không tồn tại");

                var wallet = await _unitOfWork.Wallets.GetByUserIdAsync(userId);
                if (wallet == null) throw new ArgumentException("Ví không tồn tại");

                if (wallet.Balance < package.Price)
                    throw new InvalidOperationException("Số dư không đủ để mua gói");

                wallet.Balance -= package.Price;
                _unitOfWork.Wallets.Update(wallet);

                var user = await _unitOfWork.Users.GetByIdAsync(userId);
                if (user == null) throw new ArgumentException("Người dùng không tồn tại");

                user.SubscriptionPackageId = package.Id;
                user.UpdatedAt = DateTime.UtcNow;
                _unitOfWork.Users.Update(user);

                await _unitOfWork.CompleteAsync();
                await _unitOfWork.CommitTransactionAsync();
                return true;
            }
            catch (Exception ex)
            {
                await _unitOfWork.RollbackTransactionAsync();
                _logger.LogError(ex, "Error buying subscription package: UserId={UserId}, PackageId={PackageId}",
                    userId, packageId);
                throw;
            }
        }
    }
}
