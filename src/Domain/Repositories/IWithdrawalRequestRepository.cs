using Domain.Common;
using Domain.Entities.Financial;
using Domain.Enums;

namespace Domain.Repositories;

/// <summary>
/// Repository interface for WithdrawalRequest aggregate root.
/// </summary>
public interface IWithdrawalRequestRepository : IRepository<WithdrawalRequest>
{
    Task<(IReadOnlyList<WithdrawalRequest> Items, int TotalCount)> GetByUserIdPagedAsync(
        Guid userId,
        int pageNumber = 1,
        int pageSize = 20,
        CancellationToken cancellationToken = default);

    Task<(IReadOnlyList<WithdrawalRequest> Items, int TotalCount)> GetPagedAsync(
        PaymentStatus? status = null,
        int pageNumber = 1,
        int pageSize = 20,
        CancellationToken cancellationToken = default);

    Task<bool> HasPendingRequestAsync(Guid userId, CancellationToken cancellationToken = default);
}
