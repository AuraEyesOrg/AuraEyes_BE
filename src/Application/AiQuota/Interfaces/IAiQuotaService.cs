using Application.AiQuota.Common;

namespace Application.AiQuota.Interfaces;

/// <summary>
/// AI Quota service interface — Stored State architecture.
/// Reads PurchasedAiQuota / UsedAiQuota directly from Patient or Organisation columns.
/// </summary>
public interface IAiQuotaService
{
    /// <summary>
    /// Get quota information for a user based on their role.
    /// </summary>
    Task<AiQuotaDto> GetQuotaAsync(Guid userId, string role, CancellationToken cancellationToken = default);

    /// <summary>
    /// Check if the user still has available AI screening credits.
    /// </summary>
    Task<bool> HasAvailableQuotaAsync(Guid userId, string role, CancellationToken cancellationToken = default);

    /// <summary>
    /// Increment UsedAiQuota by 1 on the Patient or Organisation entity and save.
    /// </summary>
    Task DeductQuotaAsync(Guid userId, string role, CancellationToken cancellationToken = default);

    /// <summary>
    /// Add purchased quota credits to the Patient or Organisation entity and save.
    /// </summary>
    Task AddPurchasedQuotaAsync(Guid userId, string role, int amount, CancellationToken cancellationToken = default);
}
