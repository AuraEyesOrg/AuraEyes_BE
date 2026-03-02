using Application.Common.Interfaces;
using Application.Common.Models;
using Application.Ophthalmologists.Schedules.Common;
using Domain.Enums;

namespace Application.Ophthalmologists.Schedules.Queries.GetSchedules;

/// <summary>
/// Query to get paginated list of schedules for an ophthalmologist.
/// </summary>
public record GetSchedulesQuery : IQuery<PagedResult<ScheduleListDto>>
{
    /// <summary>
    /// Ophthalmologist ID to get schedules for.
    /// </summary>
    public Guid OphthalmologistId { get; init; }

    /// <summary>
    /// Filter by schedule status.
    /// </summary>
    public ScheduleStatus? Status { get; init; }

    /// <summary>
    /// Filter by slot type.
    /// </summary>
    public SlotType? SlotType { get; init; }

    /// <summary>
    /// Filter schedules from this date.
    /// </summary>
    public DateOnly? FromDate { get; init; }

    /// <summary>
    /// Filter schedules up to this date.
    /// </summary>
    public DateOnly? ToDate { get; init; }

    /// <summary>
    /// Page number (default: 1).
    /// </summary>
    public int PageNumber { get; init; } = 1;

    /// <summary>
    /// Page size (default: 10).
    /// </summary>
    public int PageSize { get; init; } = 10;
}
