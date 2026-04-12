using Application.Common.Interfaces;
using Application.Wallets.Common;

namespace Application.Wallets.Queries.GetPayoutStatus;

/// <summary>
/// Query để lấy trạng thái lệnh chi từ PayOS theo WithdrawalRequestId.
/// Đọc từ DB (không gọi PayOS API trực tiếp).
/// Dùng khi ophthalmologist hoặc admin muốn xem trạng thái payout.
/// </summary>
public record GetPayoutStatusQuery : IQuery<PayoutStatusResponse>
{
    /// <summary>ID của WithdrawalRequest.</summary>
    public Guid WithdrawalRequestId { get; init; }

    /// <summary>UserId của người yêu cầu.</summary>
    public Guid RequestedByUserId { get; init; }
}
