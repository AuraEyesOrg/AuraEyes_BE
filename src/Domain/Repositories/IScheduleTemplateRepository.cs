using Domain.Common;
using Domain.Entities.Scheduling;

namespace Domain.Repositories;

/// <summary>
/// Repository interface for ScheduleTemplate aggregate root.
/// Contains domain-specific query methods for schedule templates.
/// </summary>
public interface IScheduleTemplateRepository : IRepository<ScheduleTemplate>
{
    /// <summary>
    /// Get schedule templates for a specific ophthalmologist.
    /// </summary>
    Task<IReadOnlyList<ScheduleTemplate>> GetByOphthalmologistIdAsync(
        Guid ophthalId,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Get schedule templates for a specific organisation.
    /// </summary>
    Task<IReadOnlyList<ScheduleTemplate>> GetByOrganisationIdAsync(
        Guid orgId,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Get schedule templates for a specific day of week.
    /// </summary>
    Task<IReadOnlyList<ScheduleTemplate>> GetByDayOfWeekAsync(
        Guid? ophthalId,
        Guid? orgId,
        DayOfWeek dayOfWeek,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Check if a template overlaps with existing templates.
    /// </summary>
    Task<bool> HasOverlappingTemplateAsync(
        Guid? ophthalId,
        Guid? orgId,
        DayOfWeek dayOfWeek,
        TimeOnly startTime,
        TimeOnly endTime,
        Guid? excludeTemplateId = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Get paginated schedule templates with filters.
    /// </summary>
    Task<(IReadOnlyList<ScheduleTemplate> Items, int TotalCount)> GetPagedAsync(
        Guid? ophthalId,
        Guid? orgId,
        DayOfWeek? dayOfWeek = null,
        int pageNumber = 1,
        int pageSize = 10,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Get schedule template with appointment slots included.
    /// </summary>
    Task<ScheduleTemplate?> GetByIdWithSlotsAsync(
        Guid id,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Get active system-generated templates for a specific ophthalmologist.
    /// </summary>
    Task<IReadOnlyList<ScheduleTemplate>> GetActiveSystemGeneratedByOphthalmologistIdAsync(
        Guid ophthalId,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Check if an active system-generated template exists for ophthalmologist/day.
    /// </summary>
    Task<bool> ExistsActiveSystemGeneratedTemplateAsync(
        Guid ophthalId,
        DayOfWeek dayOfWeek,
        CancellationToken cancellationToken = default);
}
