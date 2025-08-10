using Service.DTO.Request;
using Service.DTO.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.Services
{
    public interface IWalletService
    {
        Task<IEnumerable<WalletResponseDto>> GetAllAsync();
        Task<WalletResponseDto?> GetByIdAsync(long id);
        Task<WalletResponseDto> CreateAsync(WalletRequestDto dto);
        Task<WalletResponseDto?> UpdateAsync(long id, WalletRequestDto dto);
        Task<bool> DeleteAsync(long id);

        Task<decimal> GetTotalBalanceAsync();
        Task<bool> UpdateBalanceAsync(long userId, decimal amount);

        Task<WalletResponseDto?> GetByUserIdAsync(long userId);

    }
}
