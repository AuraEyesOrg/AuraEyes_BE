using Domain.Common;
using Domain.Entities.Scheduling;
using Domain.Enums;

namespace Domain.Repositories;

/// <summary>
/// Repository contract for AppointmentSlot aggregate root.
/// Clinic-centric model - no ophthalmologist/organisation ownership filters.
/// </summary>
public interface IAppointmentSlotRepository : IRepository<AppointmentSlot>
{
    Task<IReadOnlyList<AppointmentSlot>> GetByTemplateIdAsync(
        Guid scheduleTemplateId,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<AppointmentSlot>> GetByDateRangeAsync(
        Guid scheduleTemplateId,
        DateOnly fromDate,
        DateOnly toDate,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<AppointmentSlot>> GetAvailableSlotsAsync(
        Guid? scheduleTemplateId,
        DateOnly? fromDate = null,
        DateOnly? toDate = null,
        CancellationToken cancellationToken = default);

    Task<AppointmentSlot?> GetByIdWithTemplateAsync(
        Guid id,
        CancellationToken cancellationToken = default);

    Task<(IReadOnlyList<AppointmentSlot> Items, int TotalCount)> GetPagedAsync(
        Guid? scheduleTemplateId,
        ScheduleStatus? status = null,
        DateOnly? fromDate = null,
        DateOnly? toDate = null,
        bool excludePastSlots = false,
        int pageNumber = 1,
        int pageSize = 10,
        CancellationToken cancellationToken = default);

    Task<bool> HasOverlappingSlotAsync(
        DateOnly date,
        TimeOnly startTime,
        TimeOnly endTime,
        Guid? excludeSlotId = null,
        CancellationToken cancellationToken = default);

    Task<AppointmentSlot?> GetByIdWithLockAsync(
        Guid id,
        CancellationToken cancellationToken = default);

    Task<Dictionary<ScheduleStatus, int>> GetStatusCountsAsync(
        CancellationToken cancellationToken = default);
}
