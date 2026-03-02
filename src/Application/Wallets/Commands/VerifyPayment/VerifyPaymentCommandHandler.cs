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

                // Update wallet balance (entity is already tracked by EF change tracker)
                wallet.Deposit(depositRequest.Amount, $"Deposit via PayOS - Order: {request.OrderCode}");

                // Create transaction record and explicitly add to context
                // NOTE: Do NOT use wallet.AddTransaction() + _walletRepository.UpdateAsync()
                // because DbSet.Update() marks ALL reachable entities as Modified,
                // including new child entities that should be Added → causes
                // UPDATE instead of INSERT → DbUpdateConcurrencyException.
                var transaction = new WalletTransaction(
                    wallet.Id,
                    depositRequest.Amount,
                    TransactionType.Deposit,
                    $"Deposit via PayOS - Order: {request.OrderCode}");

                await _walletRepository.AddTransactionAsync(transaction, cancellationToken);

                // Update deposit request status (entity already tracked, no need for UpdateAsync)
                depositRequest.Complete(txnRef, $"PayOS Status: {payosStatus}");

                // EF change tracker detects all modifications automatically
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
                // Entity already tracked — change tracker detects the modification
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
                // Entity already tracked — change tracker detects the modification
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
        catch (ConcurrencyException)
        {
            // Race condition: two concurrent requests both passed the Pending check
            // before either had finished writing. The first one succeeded; we need to
            // reload and return its result instead of failing the second request.
            _logger.LogWarning(
                "Concurrency conflict while verifying OrderCode {OrderCode}. Reloading to check final state.",
                request.OrderCode);

            var refreshed = await _depositRequestRepository.GetByOrderCodeAsync(
                request.OrderCode, cancellationToken);

            if (refreshed?.Status == PaymentStatus.Completed)
            {
                var wallet = await _walletRepository.GetByIdAsync(refreshed.WalletId, cancellationToken);
                return Result<VerifyPaymentResponse>.Success(new VerifyPaymentResponse
                {
                    DepositRequestId = refreshed.Id,
                    OrderCode = request.OrderCode,
                    Status = "Completed",
                    Amount = refreshed.Amount,
                    IsSuccess = true,
                    Message = "Payment verified and wallet credited successfully.",
                    NewBalance = wallet?.Balance
                });
            }

            // Concurrency conflict but payment wasn't completed – treat as failure
            return Result<VerifyPaymentResponse>.Failure(
                "Payment verification encountered a conflict. Please try again.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to verify payment for OrderCode: {OrderCode}", request.OrderCode);
            return Result<VerifyPaymentResponse>.Failure($"Failed to verify payment: {ex.Message}");
        }
    }
}
