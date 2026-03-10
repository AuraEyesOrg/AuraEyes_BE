using Domain.Common;
using Domain.Entities.Contracts;
using Domain.Enums;

namespace Domain.Repositories;

public interface IContractRepository : IRepository<Contract>
{
    Task<Contract?> GetByIdWithDetailsAsync(Guid id, CancellationToken cancellationToken = default);

    Task<(IReadOnlyList<Contract> Items, int TotalCount)> GetPagedAsync(
        string? searchTerm = null,
        Guid? userId = null,
        ContractStatus? status = null,
        ContractType? contractType = null,
        int pageNumber = 1,
        int pageSize = 20,
        CancellationToken cancellationToken = default);

    Task<bool> ExistsByContractNumberAsync(
        string contractNumber,
        Guid? excludeId = null,
        CancellationToken cancellationToken = default);
}
