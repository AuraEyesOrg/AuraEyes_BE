using Application.Common.Interfaces;
using Application.Scheduling.AppointmentSlots.Common;

namespace Application.Scheduling.AppointmentSlots.Queries.GetAppointmentSlotStats;

/// <summary>
/// Query to get appointment slot statistics for an ophthalmologist or organisation.
/// </summary>
public record GetAppointmentSlotStatsQuery : IQuery<AppointmentSlotStatsDto>
{
    /// <summary>The ophthalmologist to retrieve stats for (optional).</summary>
    public Guid? OphthalId { get; init; }

    /// <summary>The organisation to retrieve stats for (optional).</summary>
    public Guid? OrgId { get; init; }
}
