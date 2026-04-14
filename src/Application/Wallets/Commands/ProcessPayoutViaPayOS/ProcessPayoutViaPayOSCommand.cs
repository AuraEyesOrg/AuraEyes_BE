using Application.Common.Interfaces;
using Application.Wallets.Common;

namespace Application.Wallets.Commands.ProcessPayoutViaPayOS;

/// <summary>
/// Command để xử lý yêu cầu rút tiền qua PayOS Payout API.
/// Ophthalmologist gửi request này sau khi tạo WithdrawalRequest,
/// hoặc Admin trigger để tự động chi tiền qua PayOS.
/// </summary>
public record ProcessPayoutViaPayOSCommand : ICommand<PayoutViaPayOSResponse>
{
    /// <summary>ID của WithdrawalRequest cần xử lý.</summary>
    public Guid WithdrawalRequestId { get; init; }

    /// <summary>UserId của người thực hiện (admin hoặc ophthalmologist).</summary>
    public Guid RequestedByUserId { get; init; }

    /// <summary>
    /// Danh mục thanh toán PayOS (mặc định: ["salary"]).
    /// Ví dụ: salary, bonus, commission.
    /// </summary>
    public List<string> Categories { get; init; } = new() { "salary" };
}
