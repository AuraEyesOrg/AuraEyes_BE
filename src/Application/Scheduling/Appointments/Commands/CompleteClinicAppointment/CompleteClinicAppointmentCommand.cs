using Application.Common.Interfaces;

namespace Application.Scheduling.Appointments.Commands.CompleteClinicAppointment;

public record CompleteClinicAppointmentCommand(Guid AppointmentId, string? Notes) : ICommand;
