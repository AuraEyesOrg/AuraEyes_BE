using Application.Common.Interfaces;

namespace Application.SystemAdmin.Ophthalmologists.Commands.ConfirmWithdrawalRequest;

public record ConfirmWithdrawalRequestCommand : ICommand
{
    public Guid WithdrawalRequestId { get; init; }
    public Guid AdminUserId { get; init; }
    public string? TransferReference { get; init; }
    public string? Note { get; init; }
}
