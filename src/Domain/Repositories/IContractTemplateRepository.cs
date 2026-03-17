using Domain.Common;
using Domain.Entities.Contracts;
using Domain.Enums;

namespace Domain.Repositories;

public interface IContractTemplateRepository : IRepository<ContractTemplate>
{
    Task<(IReadOnlyList<ContractTemplate> Items, int TotalCount)> GetPagedAsync(
        string? searchTerm = null,
        ContractType? type = null,
        bool? isActive = null,
        int pageNumber = 1,
        int pageSize = 20,
        CancellationToken cancellationToken = default);

    Task<bool> ExistsByTypeAndVersionAsync(
        ContractType type,
        string version,
        Guid? excludeId = null,
        CancellationToken cancellationToken = default);
}
