using Domain.Common;
using Domain.Entities.Platform;

namespace Domain.Repositories;

public interface ILeavePolicyRepository : IRepository<LeavePolicy>
{
    Task<(IReadOnlyList<LeavePolicy> Items, int TotalCount)> GetPagedAsync(
        int pageNumber = 1,
        int pageSize = 20,
        CancellationToken cancellationToken = default);
}
