using Domain.Common;
using Domain.Entities.Scheduling;

namespace Domain.Repositories;

/// <summary>
/// Repository interface for AvailableSlot aggregate root.
/// Contains domain-specific query methods for availability management.
/// </summary>
public interface IAvailableSlotRepository : IRepository<AvailableSlot>
{
    /// <summary>
    /// Get available slots for a specific ophthalmologist.
    /// </summary>
    Task<IReadOnlyList<AvailableSlot>> GetByOphthalmologistIdAsync(
        Guid ophthalmologistId,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Get available slots for a specific organisation.
    /// </summary>
    Task<IReadOnlyList<AvailableSlot>> GetByOrganisationIdAsync(
        Guid organisationId,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Get available slots within a date range.
    /// </summary>
    Task<IReadOnlyList<AvailableSlot>> GetByDateRangeAsync(
        Guid? ophthalmologistId,
        Guid? organisationId,
        DateTime fromDate,
        DateTime toDate,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Check if a time slot overlaps with existing available slots.
    /// </summary>
    Task<bool> HasOverlappingSlotAsync(
        Guid? ophthalmologistId,
        Guid? organisationId,
        DateTime startTime,
        DateTime endTime,
        Guid? excludeSlotId = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Get paginated available slots with filters.
    /// </summary>
    Task<(IReadOnlyList<AvailableSlot> Items, int TotalCount)> GetPagedAsync(
        Guid? ophthalmologistId,
        Guid? organisationId,
        DateTime? fromDate = null,
        DateTime? toDate = null,
        int pageNumber = 1,
        int pageSize = 10,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Get available slot with schedules included.
    /// </summary>
    Task<AvailableSlot?> GetByIdWithSchedulesAsync(
        Guid id,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Get slots that have remaining capacity (not fully booked).
    /// </summary>
    Task<IReadOnlyList<AvailableSlot>> GetSlotsWithAvailableCapacityAsync(
        Guid? ophthalmologistId,
        Guid? organisationId,
        DateTime fromDate,
        DateTime toDate,
        CancellationToken cancellationToken = default);
}
