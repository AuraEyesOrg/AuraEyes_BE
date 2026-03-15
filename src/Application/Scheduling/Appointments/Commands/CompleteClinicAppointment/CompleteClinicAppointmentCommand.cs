using Application.Common.Interfaces;

namespace Application.Scheduling.Appointments.Commands.CompleteClinicAppointment;

public record CompleteClinicAppointmentCommand : ICommand
{
    public Guid AppointmentId { get; init; }
    public string? Notes { get; init; }
}
