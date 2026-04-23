using Application.Common.Interfaces;
using Application.Common.Models;
using Application.Common.Constants;
using Application.Wallets.Common;
using Domain.Common;
using Domain.Entities.Financial;
using Domain.Enums;
using Domain.Repositories;
using Microsoft.Extensions.Logging;

namespace Application.Wallets.Commands.CreateDeposit;

/// <summary>
/// Handler for CreateDepositCommand.
/// </summary>
public class CreateDepositCommandHandler : ICommandHandler<CreateDepositCommand, CreateDepositResponse>
{
    private readonly IWalletRepository _walletRepository;
    private readonly IDepositRequestRepository _depositRequestRepository;
    private readonly IPayOSService _payOSService;
    private readonly IIdentityService _identityService;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<CreateDepositCommandHandler> _logger;

    public CreateDepositCommandHandler(
        IWalletRepository walletRepository,
        IDepositRequestRepository depositRequestRepository,
        IPayOSService payOSService,
        IIdentityService identityService,
        IUnitOfWork unitOfWork,
        ILogger<CreateDepositCommandHandler> logger)
    {
        _walletRepository = walletRepository;
        _depositRequestRepository = depositRequestRepository;
        _payOSService = payOSService;
        _identityService = identityService;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<Result<CreateDepositResponse>> Handle(
        CreateDepositCommand request,
        CancellationToken cancellationToken)
    {
        try
        {
            // Get or create wallet
            var wallet = await _walletRepository.GetByUserIdAsync(request.UserId, cancellationToken);

            if (wallet is null)
            {
                var roles = await _identityService.GetUserRolesAsync(request.UserId);
                var ownerType = roles.Contains(Roles.Ophthalmologist)
                    ? "Ophthalmologist"
                    : roles.Contains(Roles.ClinicStaff)
                        ? "ClinicStaff"
                        : "Patient";

                wallet = new Wallet(request.UserId, ownerType, 0);
                await _walletRepository.AddAsync(wallet, cancellationToken);
                await _unitOfWork.SaveChangesAsync(cancellationToken);

                _logger.LogInformation("Created new wallet {WalletId} for user {UserId}", wallet.Id, request.UserId);
            }

            // Create deposit request
            var description = request.Description ?? $"Nap tien AuraEyes {request.AmountVnd:N0} VND";

            var depositRequest = new DepositRequest(
                request.UserId,
                wallet.Id,
                request.AmountVnd,
                request.PaymentMethod,
                request.ReturnUrl,
                request.CancelUrl,
                description);

            await _depositRequestRepository.AddAsync(depositRequest, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            _logger.LogInformation(
                "Created deposit request {DepositRequestId} for user {UserId}: {Amount} VND",
                depositRequest.Id, request.UserId, request.AmountVnd);

            // Create PayOS payment link
            var (paymentUrl, orderCode) = await _payOSService.CreatePaymentLinkAsync(
                depositRequest.Id,
                request.AmountVnd,
                description,
                request.ReturnUrl ?? string.Empty,
                request.CancelUrl ?? string.Empty);

            // Update deposit request with payment link info
            depositRequest.SetPaymentLink(paymentUrl, orderCode);
            // The entity is already tracked after AddAsync + SaveChangesAsync.
            // Calling Update here can mark immutable audit fields as modified and cause EF to treat it as Added.
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            _logger.LogInformation(
                "Created PayOS payment link for deposit {DepositRequestId}: OrderCode={OrderCode}",
                depositRequest.Id, orderCode);

            return Result<CreateDepositResponse>.Success(new CreateDepositResponse
            {
                DepositRequestId = depositRequest.Id,
                PaymentUrl = paymentUrl,
                OrderCode = orderCode,
                Amount = request.AmountVnd,
                Status = depositRequest.Status.ToString()
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to create deposit request for user {UserId}", request.UserId);
            return Result<CreateDepositResponse>.Failure($"Failed to create deposit: {ex.Message}");
        }
    }
}
