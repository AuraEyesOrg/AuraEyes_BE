using Application.Common.Interfaces;

namespace Application.SystemAdmin.Ophthalmologists.Commands.RejectWithdrawalRequest;

public record RejectWithdrawalRequestCommand : ICommand
{
    public Guid WithdrawalRequestId { get; init; }
    public Guid AdminUserId { get; init; }
    public string? Reason { get; init; }
}
