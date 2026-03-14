using Application.AiQuota.Interfaces;
using Application.Common.Interfaces;
using Application.Common.Models;

namespace Application.AiQuota.Commands.DeductQuota;

/// <summary>
/// Handler for DeductQuotaCommand — checks remaining quota
/// and returns 402 Payment Required if exhausted.
/// </summary>
public class DeductQuotaCommandHandler : ICommandHandler<DeductQuotaCommand, DeductQuotaResponse>
{
    private readonly IAiQuotaService _quotaService;
    private readonly ICurrentUserService _currentUser;

    public DeductQuotaCommandHandler(IAiQuotaService quotaService, ICurrentUserService currentUser)
    {
        _quotaService = quotaService;
        _currentUser = currentUser;
    }

    public async Task<Result<DeductQuotaResponse>> Handle(DeductQuotaCommand request, CancellationToken cancellationToken)
    {
        if (_currentUser.UserId is null)
            return Result<DeductQuotaResponse>.Failure("User is not authenticated.");

        var userId = _currentUser.UserId.Value;
        var role = _currentUser.Roles.FirstOrDefault() ?? "Patient";

        var hasQuota = await _quotaService.HasAvailableQuotaAsync(userId, role, cancellationToken);

        if (!hasQuota)
            return Result<DeductQuotaResponse>.PaymentRequired(
                "AI screening quota exhausted. Please purchase additional credits.");

        // Increment UsedAiQuota on the Patient or Organisation entity
        await _quotaService.DeductQuotaAsync(userId, role, cancellationToken);

        // Re-fetch to get accurate numbers for response
        var quota = await _quotaService.GetQuotaAsync(userId, role, cancellationToken);

        return Result<DeductQuotaResponse>.Success(new DeductQuotaResponse
        {
            RemainingQuota = quota.RemainingQuota,
            TotalQuota = quota.TotalQuota,
            UsedQuota = quota.UsedQuota
        });
    }
}
