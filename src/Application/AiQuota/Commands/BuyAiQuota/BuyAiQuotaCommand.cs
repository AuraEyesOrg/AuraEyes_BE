using Application.AiQuota.Common;
using Application.Common.Interfaces;

namespace Application.AiQuota.Commands.BuyAiQuota;

public record BuyAiQuotaCommand : ICommand<BuyAiQuotaResponse>
{
    public int NumberOfBundles { get; init; } = 1;
}

public record BuyAiQuotaResponse
{
    public int TotalAiQuota { get; init; }
    public int UsedAiQuota { get; init; }
    public int RemainingQuota { get; init; }
    public decimal WalletBalance { get; init; }
    public decimal AmountDeducted { get; init; }
}
