using Domain.Common;
using Domain.Entities;
using Domain.Enums;

namespace Domain.Repositories;

/// <summary>
/// Repository interface for Schedule aggregate root.
/// Contains domain-specific query methods for doctor's time slots.
/// </summary>
public interface IScheduleRepository : IRepository<Schedule>
{
    /// <summary>
    /// Get schedules for a specific ophthalmologist.
    /// </summary>
    Task<IReadOnlyList<Schedule>> GetByOphthalmologistIdAsync(
        Guid ophthalmologistId, 
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Get available schedules for a specific ophthalmologist within a date range.
    /// </summary>
    Task<IReadOnlyList<Schedule>> GetAvailableByOphthalmologistAsync(
        Guid ophthalmologistId,
        DateOnly? fromDate = null,
        DateOnly? toDate = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Get schedules for a specific date.
    /// </summary>
    Task<IReadOnlyList<Schedule>> GetByDateAsync(
        Guid ophthalmologistId,
        DateOnly date,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Check if a time slot overlaps with existing schedules.
    /// </summary>
    Task<bool> HasOverlappingScheduleAsync(
        Guid ophthalmologistId,
        DateOnly date,
        TimeOnly startTime,
        TimeOnly endTime,
        Guid? excludeScheduleId = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Get paginated schedules with filters.
    /// </summary>
    Task<(IReadOnlyList<Schedule> Items, int TotalCount)> GetPagedAsync(
        Guid ophthalmologistId,
        ScheduleStatus? status = null,
        SlotType? slotType = null,
        DateOnly? fromDate = null,
        DateOnly? toDate = null,
        int pageNumber = 1,
        int pageSize = 10,
        CancellationToken cancellationToken = default);
}
