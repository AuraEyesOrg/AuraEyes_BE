using Application.Common.Interfaces;
using Application.Common.Models;
using Application.Wallets.Common;
using Domain.Repositories;

namespace Application.Wallets.Queries.GetPayoutStatus;

/// <summary>
/// Handler lấy trạng thái lệnh chi từ DB (không gọi PayOS API).
/// Ophthalmologist chỉ xem được request của chính mình; Admin xem được tất cả.
/// </summary>
public class GetPayoutStatusQueryHandler
    : IQueryHandler<GetPayoutStatusQuery, PayoutStatusResponse>
{
    private readonly IWithdrawalRequestRepository _withdrawalRequestRepository;
    private readonly IIdentityService _identityService;

    public GetPayoutStatusQueryHandler(
        IWithdrawalRequestRepository withdrawalRequestRepository,
        IIdentityService identityService)
    {
        _withdrawalRequestRepository = withdrawalRequestRepository;
        _identityService = identityService;
    }

    public async Task<Result<PayoutStatusResponse>> Handle(
        GetPayoutStatusQuery request,
        CancellationToken cancellationToken)
    {
        var withdrawalRequest = await _withdrawalRequestRepository.GetByIdAsync(
            request.WithdrawalRequestId, cancellationToken);

        if (withdrawalRequest is null)
            return Result<PayoutStatusResponse>.NotFound("Withdrawal request not found.");

        // Ophthalmologist chỉ xem được request của chính mình
        var isAdmin = await _identityService.IsInRoleAsync(
            request.RequestedByUserId, Application.Common.Constants.Roles.SystemAdmin);

        if (!isAdmin && withdrawalRequest.UserId != request.RequestedByUserId)
            return Result<PayoutStatusResponse>.Forbidden(
                "You are not authorized to view this withdrawal request.");

        var response = new PayoutStatusResponse
        {
            WithdrawalRequestId = withdrawalRequest.Id,
            ExternalPayoutId = withdrawalRequest.ExternalPayoutId ?? string.Empty,
            PayOSReferenceId = withdrawalRequest.PayOSReferenceId ?? string.Empty,
            ApprovalState = withdrawalRequest.PayOSApprovalState ?? string.Empty,
            WithdrawalStatus = withdrawalRequest.Status.ToString(),
            Transactions = new List<PayOSPayoutTransactionDto>()
            // Giao dịch chi tiết chỉ có khi gọi PayOS trực tiếp (SyncPayoutStatus)
        };

        return Result<PayoutStatusResponse>.Success(response);
    }
}
