using Domain.Common;
using Domain.Entities.Scheduling;
using Domain.Enums;

namespace Domain.Repositories;

/// <summary>
/// Repository interface for AppointmentSlot aggregate root.
/// Contains domain-specific query methods for appointment slots.
/// </summary>
public interface IAppointmentSlotRepository : IRepository<AppointmentSlot>
{
    /// <summary>
    /// Get appointment slots for a specific schedule template.
    /// </summary>
    Task<IReadOnlyList<AppointmentSlot>> GetByTemplateIdAsync(
        Guid scheduleTemplateId,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Get appointment slots for a specific date range.
    /// </summary>
    Task<IReadOnlyList<AppointmentSlot>> GetByDateRangeAsync(
        Guid scheduleTemplateId,
        DateOnly fromDate,
        DateOnly toDate,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Get available appointment slots (status = Available and has capacity).
    /// </summary>
    Task<IReadOnlyList<AppointmentSlot>> GetAvailableSlotsAsync(
        Guid? scheduleTemplateId,
        DateOnly? fromDate = null,
        DateOnly? toDate = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Get appointment slot with template included.
    /// </summary>
    Task<AppointmentSlot?> GetByIdWithTemplateAsync(
        Guid id,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Get paginated appointment slots with filters.
    /// </summary>
    Task<(IReadOnlyList<AppointmentSlot> Items, int TotalCount)> GetPagedAsync(
        Guid? scheduleTemplateId,
        Guid? ophthalId = null,
        Guid? orgId = null,
        ScheduleStatus? status = null,
        DateOnly? fromDate = null,
        DateOnly? toDate = null,
        bool excludePastSlots = false,
        int pageNumber = 1,
        int pageSize = 10,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Get appointment slots by ophthalmologist (via template).
    /// </summary>
    Task<IReadOnlyList<AppointmentSlot>> GetByOphthalmologistAsync(
        Guid ophthalId,
        DateOnly? fromDate = null,
        DateOnly? toDate = null,
        ScheduleStatus? status = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Get min/max slot price for an ophthalmologist with optional filters.
    /// </summary>
    Task<(decimal? MinPrice, decimal? MaxPrice)> GetPriceRangeByOphthalmologistAsync(
        Guid ophthalId,
        DateOnly? fromDate = null,
        DateOnly? toDate = null,
        ScheduleStatus? status = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Get min/max slot prices for multiple ophthalmologists with optional filters.
    /// </summary>
    Task<IReadOnlyDictionary<Guid, (decimal? MinPrice, decimal? MaxPrice)>> GetPriceRangesByOphthalmologistAsync(
        IReadOnlyCollection<Guid> ophthalIds,
        DateOnly? fromDate = null,
        DateOnly? toDate = null,
        ScheduleStatus? status = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Get appointment slots by organisation (via template).
    /// </summary>
    Task<IReadOnlyList<AppointmentSlot>> GetByOrganisationAsync(
        Guid orgId,
        DateOnly? fromDate = null,
        DateOnly? toDate = null,
        ScheduleStatus? status = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Get status counts for reporting.
    /// </summary>
    Task<Dictionary<ScheduleStatus, int>> GetStatusCountsAsync(
        Guid? ophthalId,
        Guid? orgId,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Check if a time slot overlaps with existing appointment slots.
    /// </summary>
    Task<bool> HasOverlappingSlotAsync(
        DateOnly date,
        TimeOnly startTime,
        TimeOnly endTime,
        Guid? excludeSlotId = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Get all expired reservations (Reserved status and ReservationExpireAt &lt; now).
    /// </summary>
    Task<IReadOnlyList<AppointmentSlot>> GetExpiredReservationsAsync(
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Get slot with pessimistic lock for thread-safe booking operations.
    /// </summary>
    Task<AppointmentSlot?> GetByIdWithLockAsync(
        Guid id,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Get available slots for organisation with capacity (booked_count < max_capacity).
    /// For patient booking flow.
    /// </summary>
    Task<IReadOnlyList<AppointmentSlot>> GetAvailableByOrganisationWithCapacityAsync(
        Guid organisationId,
        DateOnly? fromDate = null,
        DateOnly? toDate = null,
        CancellationToken cancellationToken = default);
}
