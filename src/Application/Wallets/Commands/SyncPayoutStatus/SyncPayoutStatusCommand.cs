using Application.Common.Interfaces;
using Application.Wallets.Common;

namespace Application.Wallets.Commands.SyncPayoutStatus;

/// <summary>
/// Command để đồng bộ trạng thái lệnh chi từ PayOS về hệ thống.
/// Gọi PayOS GET /v1/payouts/{payoutId} và cập nhật WithdrawalRequest.
/// Sử dụng khi: polling trạng thái, hoặc admin muốn force-sync.
/// </summary>
public record SyncPayoutStatusCommand : ICommand<PayoutStatusResponse>
{
    /// <summary>ID của WithdrawalRequest cần đồng bộ trạng thái.</summary>
    public Guid WithdrawalRequestId { get; init; }

    /// <summary>UserId của người thực hiện (admin).</summary>
    public Guid RequestedByUserId { get; init; }
}
