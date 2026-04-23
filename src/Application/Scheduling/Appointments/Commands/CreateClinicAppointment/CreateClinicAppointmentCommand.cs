using Application.Common.Interfaces;
using Application.Scheduling.Appointments.Common;

namespace Application.Scheduling.Appointments.Commands.CreateClinicAppointment;

public record CreateClinicAppointmentCommand : ICommand<CreateClinicAppointmentResult>
{
    public Guid SlotId { get; init; }
    public string? VisitReason { get; init; }
}
