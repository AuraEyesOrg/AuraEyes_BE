using Domain.Common;
using Domain.Entities.Scheduling;
using Domain.Enums;

namespace Domain.Repositories;

/// <summary>
/// Repository contract for SlotAssignment aggregate root.
/// </summary>
public interface ISlotAssignmentRepository : IRepository<SlotAssignment>
{
    Task<IReadOnlyList<SlotAssignment>> GetBySlotIdAsync(
        Guid appointmentSlotId,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<SlotAssignment>> GetByStaffIdAsync(
        Guid staffId,
        DateOnly? fromDate = null,
        DateOnly? toDate = null,
        CancellationToken cancellationToken = default);

    Task<bool> HasAssignmentAsync(
        Guid appointmentSlotId,
        Guid staffId,
        SlotAssignmentRole role,
        CancellationToken cancellationToken = default);
}
