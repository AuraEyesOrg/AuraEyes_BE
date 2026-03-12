using Application.ClinicAppointments.Common;
using Application.Common.Interfaces;

namespace Application.ClinicAppointments.Queries.GetOrganisationAvailableSlots;

/// <summary>
/// Query to get available slots for an organisation (for patient booking).
/// Returns only slots with remaining capacity.
/// </summary>
public record GetOrganisationAvailableSlotsQuery : IQuery<IReadOnlyList<OrganisationAvailableSlotDto>>
{
    /// <summary>Organisation to get slots for.</summary>
    public Guid OrganisationId { get; init; }

    /// <summary>Filter from date (optional).</summary>
    public DateOnly? FromDate { get; init; }

    /// <summary>Filter to date (optional).</summary>
    public DateOnly? ToDate { get; init; }
}
