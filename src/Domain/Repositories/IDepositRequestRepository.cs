using Domain.Common;
using Domain.Entities;
using Domain.Enums;

namespace Domain.Repositories;

/// <summary>
/// Repository interface for DepositRequest aggregate root.
/// </summary>
public interface IDepositRequestRepository : IRepository<DepositRequest>
{
    /// <summary>
    /// Get deposit request by PayOS order code.
    /// </summary>
    Task<DepositRequest?> GetByOrderCodeAsync(string orderCode, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Get deposit requests by UserId with pagination.
    /// </summary>
    Task<(IReadOnlyList<DepositRequest> Items, int TotalCount)> GetByUserIdPagedAsync(
        Guid userId,
        int pageNumber = 1,
        int pageSize = 20,
        CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Get deposit requests by status.
    /// </summary>
    Task<IReadOnlyList<DepositRequest>> GetByStatusAsync(
        PaymentStatus status,
        CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Get pending deposit requests older than specified time (for cleanup).
    /// </summary>
    Task<IReadOnlyList<DepositRequest>> GetExpiredPendingRequestsAsync(
        TimeSpan olderThan,
        CancellationToken cancellationToken = default);
}
