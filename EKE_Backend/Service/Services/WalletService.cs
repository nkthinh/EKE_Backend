using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Repository.Entities;
using Repository.UnitOfWork;
using Service.DTO.Request;
using Service.DTO.Response;
using Service.Services;

public class WalletService : IWalletService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly ILogger<WalletService> _logger;

    public WalletService(IUnitOfWork unitOfWork, IMapper mapper, ILogger<WalletService> logger)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<WalletResponseDto?> GetByUserIdAsync(long userId)
    {
        var wallet = await _unitOfWork.Wallets
            .Query()
            .Include(w => w.User) // Nếu muốn trả luôn thông tin user
            .FirstOrDefaultAsync(w => w.UserId == userId);

        return wallet == null ? null : _mapper.Map<WalletResponseDto>(wallet);
    }
    

    public async Task<IEnumerable<WalletResponseDto>> GetAllAsync()
    {
        var wallets = await _unitOfWork.Wallets.GetAllAsync();
        return wallets.Select(_mapper.Map<WalletResponseDto>);
    }

    public async Task<WalletResponseDto?> GetByIdAsync(long id)
    {
        var wallet = await _unitOfWork.Wallets
            .Query()
            .Where(w => w.Id == id)
            .Include(w => w.User)
            .FirstOrDefaultAsync();

        return wallet == null ? null : _mapper.Map<WalletResponseDto>(wallet);
    }

    public async Task<WalletResponseDto> CreateAsync(WalletRequestDto dto)
    {
        var wallet = _mapper.Map<Wallet>(dto);
        await _unitOfWork.Wallets.AddAsync(wallet);
        await _unitOfWork.CompleteAsync();

        // Include User info for mapping response
        wallet.User = await _unitOfWork.Users.GetByIdAsync(wallet.UserId);

        return _mapper.Map<WalletResponseDto>(wallet);
    }

    public async Task<WalletResponseDto?> UpdateAsync(long id, WalletRequestDto dto)
    {
        var wallet = await _unitOfWork.Wallets.GetByIdAsync(id);
        if (wallet == null) return null;

        wallet.Balance = dto.Balance;
        wallet.UserId = dto.UserId;
        wallet.UpdatedAt = DateTime.UtcNow;

        _unitOfWork.Wallets.Update(wallet);
        await _unitOfWork.CompleteAsync();

        wallet.User = await _unitOfWork.Users.GetByIdAsync(wallet.UserId);

        return _mapper.Map<WalletResponseDto>(wallet);
    }

    public async Task<bool> DeleteAsync(long id)
    {
        var success = await _unitOfWork.Wallets.RemoveByIdAsync(id);
        if (!success) return false;

        await _unitOfWork.CompleteAsync();
        return true;
    }

    public async Task<decimal> GetTotalBalanceAsync()
    {
        var wallets = await _unitOfWork.Wallets.GetAllAsync();
        return wallets.Sum(w => w.Balance);
    }

    public async Task<bool> UpdateBalanceAsync(long userId, decimal amount)
    {
        var wallet = await _unitOfWork.Wallets
            .Query()
            .FirstOrDefaultAsync(w => w.UserId == userId);

        if (wallet == null) return false;

        wallet.Balance += amount;
        wallet.UpdatedAt = DateTime.UtcNow;

        _unitOfWork.Wallets.Update(wallet);
        await _unitOfWork.CompleteAsync();
        return true;
    }

}
