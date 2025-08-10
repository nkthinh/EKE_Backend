using Application.DTOs;
using Application.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Service.DTO.Request;
using Service.DTO.Response;
using Service.Services;
using Service.Services.Users;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace WebAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class WalletController : ControllerBase
    {
        private readonly IWalletService _walletService;
        private readonly IUserService _userService;

        public WalletController(IWalletService walletService , IUserService userService)
        {
            _walletService = walletService;
            _userService = userService;
        }

        /// <summary>
        /// Get all wallets
        /// </summary>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<WalletResponseDto>>> GetAllWallets()
        {
            try
            {
                var wallets = await _walletService.GetAllAsync();
                return Ok(wallets);
            }
            catch (Exception ex)
            {
                return BadRequest($"Error retrieving wallets: {ex.Message}");
            }
        }

        /// <summary>
        /// Get wallet by userId
        /// </summary>
        [HttpGet("{userId}")]
        public async Task<ActionResult<WalletResponseDto>> GetWallet(long userId)
        {
            try
            {
                var wallet = await _walletService.GetByUserIdAsync(userId);
                if (wallet == null)
                    return NotFound("Wallet not found");

                return Ok(wallet);
            }
            catch (Exception ex)
            {
                return BadRequest($"Error retrieving wallet: {ex.Message}");
            }
        }


        /// <summary>
        /// Create a new wallet
        /// </summary>
        [HttpPost]
        public async Task<ActionResult<WalletResponseDto>> CreateWallet([FromBody] WalletRequestDto requestDto)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                var wallet = await _walletService.CreateAsync(requestDto);
                return CreatedAtAction(nameof(GetWallet), new { userId = wallet.UserId }, wallet);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                return BadRequest($"Error creating wallet: {ex.Message}");
            }
        }

        /// <summary>
        /// Update a wallet
        /// </summary>
        [HttpPut("{userId}")]
        public async Task<ActionResult<WalletResponseDto>> UpdateWallet(long userId, [FromBody] WalletRequestDto requestDto)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                var wallet = await _walletService.UpdateAsync(userId, requestDto);
                return Ok(wallet);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                return BadRequest($"Error updating wallet: {ex.Message}");
            }
        }

        /// <summary>
        /// Delete a wallet
        /// </summary>
        [HttpDelete("{userId}")]
        public async Task<ActionResult> DeleteWallet(long userId)
        {
            try
            {
                var result = await _walletService.DeleteAsync(userId);
                if (!result)
                    return NotFound($"Wallet for userId {userId} not found");

                return NoContent();
            }
            catch (Exception ex)
            {
                return BadRequest($"Error deleting wallet: {ex.Message}");
            }
        }

        /// <summary>
        /// Admin update wallet balance manually
        /// </summary>
        [HttpPatch("{userId}/balance")]
        public async Task<ActionResult<WalletResponseDto>> UpdateBalance(long userId, [FromBody] UpdateWalletBalanceRequest request)
        {
            try
            {
                var wallet = await _walletService.UpdateBalanceAsync(userId, request.NewBalance);
                return Ok(wallet);
            }
            catch (Exception ex)
            {
                return BadRequest($"Error updating balance: {ex.Message}");
            }
        }

        /// <summary>
        /// Get total balance in system
        /// </summary>
        [HttpGet("total-balance")]
        public async Task<ActionResult<decimal>> GetTotalBalance()
        {
            try
            {
                var total = await _walletService.GetTotalBalanceAsync();
                return Ok(new { totalBalance = total });
            }
            catch (Exception ex)
            {
                return BadRequest($"Error retrieving total balance: {ex.Message}");
            }
        }
    }

    /// <summary>
    /// Payload for updating wallet balance
    /// </summary>
    public class UpdateWalletBalanceRequest
    {
        public decimal NewBalance { get; set; }
    }
}
