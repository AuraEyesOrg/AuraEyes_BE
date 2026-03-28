using Application.Common.Interfaces;
using Application.Wallets.Common;

namespace Application.Wallets.Commands.CreateWithdrawalRequest;

/// <summary>
/// Command for ophthalmologist to create a withdrawal request.
/// </summary>
public record CreateWithdrawalRequestCommand : ICommand<WithdrawalRequestDto>
{
    public Guid UserId { get; init; }
    public decimal AmountVnd { get; init; }
    public string BankName { get; init; } = string.Empty;
    public string BankAccountNumber { get; init; } = string.Empty;
    public string AccountHolderName { get; init; } = string.Empty;
    public string? ContractNumber { get; init; }
    public string? Note { get; init; }
}
