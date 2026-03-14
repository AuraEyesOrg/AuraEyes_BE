using Application.Common.Interfaces;
using Application.Scheduling.Appointments.Common;

namespace Application.Scheduling.Appointments.Queries.GetOrganisationAvailableSlots;

public record GetOrganisationAvailableSlotsQuery : IQuery<IReadOnlyList<OrganisationAvailableSlotDto>>
{
    public Guid OrganisationId { get; init; }
    public DateOnly? Date { get; init; }
    public DateOnly? FromDate { get; init; }
    public DateOnly? ToDate { get; init; }
}
