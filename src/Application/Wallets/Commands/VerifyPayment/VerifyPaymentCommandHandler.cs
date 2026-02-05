using Application.Common.Interfaces;
using Application.Common.Models;
using Application.Wallets.Common;
using Domain.Common;
using Domain.Entities;
using Domain.Enums;
using Domain.Repositories;
using Microsoft.Extensions.Logging;

namespace Application.Wallets.Commands.VerifyPayment;

/// <summary>
/// Handler for VerifyPaymentCommand.
/// </summary>
public class VerifyPaymentCommandHandler : ICommandHandler<VerifyPaymentCommand, VerifyPaymentResponse>
{
    private readonly IDepositRequestRepository _depositRequestRepository;
    private readonly IWalletRepository _walletRepository;
    private readonly IPayOSService _payOSService;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<VerifyPaymentCommandHandler> _logger;

    public VerifyPaymentCommandHandler(
        IDepositRequestRepository depositRequestRepository,
        IWalletRepository walletRepository,
        IPayOSService payOSService,
        IUnitOfWork unitOfWork,
        ILogger<VerifyPaymentCommandHandler> logger)
    {
        _depositRequestRepository = depositRequestRepository;
        _walletRepository = walletRepository;
        _payOSService = payOSService;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<Result<VerifyPaymentResponse>> Handle(
        VerifyPaymentCommand request,
        CancellationToken cancellationToken)
    {
        try
        {
            // Get deposit request by order code
            var depositRequest = await _depositRequestRepository.GetByOrderCodeAsync(
                request.OrderCode, cancellationToken);

            if (depositRequest is null)
            {
                _logger.LogWarning("Deposit request not found for OrderCode: {OrderCode}", request.OrderCode);
                return Result<VerifyPaymentResponse>.NotFound($"Deposit request not found for order code: {request.OrderCode}");
            }

            // Validate user if provided
            if (request.UserId.HasValue && depositRequest.UserId != request.UserId.Value)
            {
                _logger.LogWarning(
                    "User {UserId} attempted to verify payment for deposit {DepositRequestId} belonging to user {OwnerId}",
                    request.UserId, depositRequest.Id, depositRequest.UserId);
                return Result<VerifyPaymentResponse>.Forbidden("You are not authorized to verify this payment.");
            }

            // If already completed, return current status
            if (depositRequest.Status == PaymentStatus.Completed)
            {
                var wallet = await _walletRepository.GetByIdAsync(depositRequest.WalletId, cancellationToken);
                
                return Result<VerifyPaymentResponse>.Success(new VerifyPaymentResponse
                {
                    DepositRequestId = depositRequest.Id,
                    OrderCode = request.OrderCode,
                    Status = depositRequest.Status.ToString(),
                    Amount = depositRequest.Amount,
                    IsSuccess = true,
                    Message = "Payment already completed.",
                    NewBalance = wallet?.Balance
                });
            }

            // If failed or cancelled, return status
            if (depositRequest.Status == PaymentStatus.Failed || depositRequest.Status == PaymentStatus.Cancelled)
            {
                return Result<VerifyPaymentResponse>.Success(new VerifyPaymentResponse
                {
                    DepositRequestId = depositRequest.Id,
                    OrderCode = request.OrderCode,
                    Status = depositRequest.Status.ToString(),
                    Amount = depositRequest.Amount,
                    IsSuccess = false,
                    Message = depositRequest.FailureReason ?? "Payment was not successful."
                });
            }

            // Query PayOS for payment status
            var (payosStatus, payosAmount, txnRef) = await _payOSService.GetPaymentStatusAsync(request.OrderCode);

            _logger.LogInformation(
                "PayOS status for OrderCode {OrderCode}: Status={Status}, Amount={Amount}",
                request.OrderCode, payosStatus, payosAmount);

            // Handle PayOS status
            if (payosStatus.Equals("PAID", StringComparison.OrdinalIgnoreCase))
            {
                // Credit wallet
                var wallet = await _walletRepository.GetByIdAsync(depositRequest.WalletId, cancellationToken);
                
                if (wallet is null)
                {
                    _logger.LogError("Wallet {WalletId} not found for deposit {DepositRequestId}", 
                        depositRequest.WalletId, depositRequest.Id);
                    return Result<VerifyPaymentResponse>.Failure("Wallet not found.");
                }

                // Update wallet balance
                wallet.Deposit(depositRequest.Amount, $"Deposit via PayOS - Order: {request.OrderCode}");
                
                // Add transaction record
                var transaction = new WalletTransaction(
                    wallet.Id,
                    depositRequest.Amount,
                    TransactionType.Deposit,
                    $"Deposit via PayOS - Order: {request.OrderCode}");
                
                wallet.AddTransaction(transaction);

                // Update deposit request status
                depositRequest.Complete(txnRef, $"PayOS Status: {payosStatus}");

                await _walletRepository.UpdateAsync(wallet, cancellationToken);
                await _depositRequestRepository.UpdateAsync(depositRequest, cancellationToken);
                await _unitOfWork.SaveChangesAsync(cancellationToken);

                _logger.LogInformation(
                    "Deposit {DepositRequestId} completed. Wallet {WalletId} credited {Amount} VND. New balance: {Balance}",
                    depositRequest.Id, wallet.Id, depositRequest.Amount, wallet.Balance);

                return Result<VerifyPaymentResponse>.Success(new VerifyPaymentResponse
                {
                    DepositRequestId = depositRequest.Id,
                    OrderCode = request.OrderCode,
                    Status = "Completed",
                    Amount = depositRequest.Amount,
                    IsSuccess = true,
                    Message = "Payment verified and wallet credited successfully.",
                    NewBalance = wallet.Balance
                });
            }
            else if (payosStatus.Equals("CANCELLED", StringComparison.OrdinalIgnoreCase))
            {
                depositRequest.Cancel("Payment cancelled by user or provider.");
                await _depositRequestRepository.UpdateAsync(depositRequest, cancellationToken);
                await _unitOfWork.SaveChangesAsync(cancellationToken);

                return Result<VerifyPaymentResponse>.Success(new VerifyPaymentResponse
                {
                    DepositRequestId = depositRequest.Id,
                    OrderCode = request.OrderCode,
                    Status = "Cancelled",
                    Amount = depositRequest.Amount,
                    IsSuccess = false,
                    Message = "Payment was cancelled."
                });
            }
            else if (payosStatus.Equals("EXPIRED", StringComparison.OrdinalIgnoreCase))
            {
                depositRequest.Fail("Payment link expired.");
                await _depositRequestRepository.UpdateAsync(depositRequest, cancellationToken);
                await _unitOfWork.SaveChangesAsync(cancellationToken);

                return Result<VerifyPaymentResponse>.Success(new VerifyPaymentResponse
                {
                    DepositRequestId = depositRequest.Id,
                    OrderCode = request.OrderCode,
                    Status = "Expired",
                    Amount = depositRequest.Amount,
                    IsSuccess = false,
                    Message = "Payment link has expired."
                });
            }
            else
            {
                // Still pending
                return Result<VerifyPaymentResponse>.Success(new VerifyPaymentResponse
                {
                    DepositRequestId = depositRequest.Id,
                    OrderCode = request.OrderCode,
                    Status = payosStatus,
                    Amount = depositRequest.Amount,
                    IsSuccess = false,
                    Message = "Payment is still pending."
                });
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to verify payment for OrderCode: {OrderCode}", request.OrderCode);
            return Result<VerifyPaymentResponse>.Failure($"Failed to verify payment: {ex.Message}");
        }
    }
}
