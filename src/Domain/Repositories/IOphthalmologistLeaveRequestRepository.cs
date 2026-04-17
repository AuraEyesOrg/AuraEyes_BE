using Domain.Common;
using Domain.Entities.Scheduling;
using Domain.Enums;

namespace Domain.Repositories;

public interface IOphthalmologistLeaveRequestRepository : IRepository<OphthalmologistLeaveRequest>
{
    Task<(IReadOnlyList<OphthalmologistLeaveRequest> Items, int TotalCount)> GetByOphthalmologistPagedAsync(
        Guid ophthalmologistId,
        OphthalmologistLeaveRequestStatus? status = null,
        int pageNumber = 1,
        int pageSize = 20,
        CancellationToken cancellationToken = default);

    Task<(IReadOnlyList<OphthalmologistLeaveRequest> Items, int TotalCount)> GetPagedAsync(
        OphthalmologistLeaveRequestStatus? status = null,
        Guid? ophthalmologistId = null,
        int pageNumber = 1,
        int pageSize = 20,
        CancellationToken cancellationToken = default);

    Task<bool> HasOverlappingActiveRequestAsync(
        Guid ophthalmologistId,
        DateOnly startDate,
        DateOnly endDate,
        Guid? excludeLeaveRequestId = null,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<OphthalmologistLeaveRequest>> GetApprovedOverlappingAsync(
        Guid ophthalmologistId,
        DateOnly fromDate,
        DateOnly toDate,
        CancellationToken cancellationToken = default);
}