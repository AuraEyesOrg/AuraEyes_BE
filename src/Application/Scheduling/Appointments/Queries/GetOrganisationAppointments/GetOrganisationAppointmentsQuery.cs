using Application.Common.Interfaces;
using Application.Scheduling.Appointments.Common;
using Domain.Enums;

namespace Application.Scheduling.Appointments.Queries.GetOrganisationAppointments;

public record GetOrganisationAppointmentsQuery : IQuery<IReadOnlyList<ClinicAppointmentDto>>
{
    public Guid OrganisationId { get; init; }
    public DateOnly? Date { get; init; }
    public DateOnly? FromDate { get; init; }
    public DateOnly? ToDate { get; init; }
    public AppointmentStatus? Status { get; init; }
}
