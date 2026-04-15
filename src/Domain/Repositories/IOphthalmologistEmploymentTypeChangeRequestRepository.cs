using Domain.Common;
using Domain.Entities.Users;
using Domain.Enums;

namespace Domain.Repositories;

public interface IOphthalmologistEmploymentTypeChangeRequestRepository : IRepository<OphthalmologistEmploymentTypeChangeRequest>
{
    Task<(IReadOnlyList<OphthalmologistEmploymentTypeChangeRequest> Items, int TotalCount)> GetByOphthalmologistPagedAsync(
        Guid ophthalmologistId,
        OphthalmologistEmploymentTypeChangeRequestStatus? status = null,
        int pageNumber = 1,
        int pageSize = 20,
        CancellationToken cancellationToken = default);

    Task<(IReadOnlyList<OphthalmologistEmploymentTypeChangeRequest> Items, int TotalCount)> GetPagedAsync(
        OphthalmologistEmploymentTypeChangeRequestStatus? status = null,
        Guid? ophthalmologistId = null,
        int pageNumber = 1,
        int pageSize = 20,
        CancellationToken cancellationToken = default);

    Task<bool> HasPendingRequestAsync(
        Guid ophthalmologistId,
        Guid? excludeRequestId = null,
        CancellationToken cancellationToken = default);
}
