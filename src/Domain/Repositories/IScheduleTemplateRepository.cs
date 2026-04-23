using Domain.Common;
using Domain.Entities.Scheduling;

namespace Domain.Repositories;

/// <summary>
/// Repository contract for ScheduleTemplate aggregate root.
/// Clinic-centric model - no doctor/organisation ownership filters.
/// </summary>
public interface IScheduleTemplateRepository : IRepository<ScheduleTemplate>
{
    Task<IReadOnlyList<ScheduleTemplate>> GetByDayOfWeekAsync(
        DayOfWeek dayOfWeek,
        CancellationToken cancellationToken = default);

    Task<bool> HasOverlappingTemplateAsync(
        DayOfWeek dayOfWeek,
        TimeOnly startTime,
        TimeOnly endTime,
        Guid? excludeTemplateId = null,
        CancellationToken cancellationToken = default);

    Task<(IReadOnlyList<ScheduleTemplate> Items, int TotalCount)> GetPagedAsync(
        DayOfWeek? dayOfWeek = null,
        int pageNumber = 1,
        int pageSize = 10,
        CancellationToken cancellationToken = default);

    Task<ScheduleTemplate?> GetByIdWithSlotsAsync(
        Guid id,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<ScheduleTemplate>> GetActiveTemplatesAsync(
        CancellationToken cancellationToken = default);
}
