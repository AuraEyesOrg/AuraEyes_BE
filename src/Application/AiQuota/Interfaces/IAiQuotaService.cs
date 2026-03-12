using Application.AiQuota.Common;

namespace Application.AiQuota.Interfaces;

/// <summary>
/// AI Quota service interface — implemented in Infrastructure layer.
/// Handles cross-table queries (SystemSettings, AiScreenings, Contracts, WalletTransactions).
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
}
