using Application.Common.Interfaces;
using Application.Wallets.Common;

namespace Application.Wallets.Commands.CreateWithdrawalRequest;

/// <summary>
/// Command for ophthalmologist to create a withdrawal request.
/// BankBin là mã BIN ngân hàng PayOS (ví dụ: "970415") - cần thiết để chi tự động qua PayOS Payout API.
/// </summary>
public record CreateWithdrawalRequestCommand : ICommand<WithdrawalRequestDto>
{
    public Guid UserId { get; init; }
    public decimal AmountVnd { get; init; }
    public string BankName { get; init; } = string.Empty;
    public string BankAccountNumber { get; init; } = string.Empty;
    public string AccountHolderName { get; init; } = string.Empty;

    /// <summary>Mã BIN ngân hàng PayOS (ví dụ: "970415" = Vietinbank).</summary>
    public string BankBin { get; init; } = string.Empty;

    public string? ContractNumber { get; init; }
    public string? Note { get; init; }
}
