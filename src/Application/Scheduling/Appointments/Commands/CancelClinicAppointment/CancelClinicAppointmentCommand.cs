using Application.Common.Interfaces;

namespace Application.Scheduling.Appointments.Commands.CancelClinicAppointment;

public record CancelClinicAppointmentCommand : ICommand
{
    public Guid AppointmentId { get; init; }
    public string? Reason { get; init; }
}
