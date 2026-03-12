using Application.Common.Interfaces;

namespace Application.AiQuota.Commands.DeductQuota;

/// <summary>
/// Command to deduct 1 AI screening credit from the current user's quota.
/// Returns 402 Payment Required if quota is exhausted.
/// </summary>
public record DeductQuotaCommand : ICommand<DeductQuotaResponse>;

/// <summary>
/// Response after successful quota deduction.
/// </summary>
public record DeductQuotaResponse
{
    public int RemainingQuota { get; init; }
    public int TotalQuota { get; init; }
    public int UsedQuota { get; init; }
}
