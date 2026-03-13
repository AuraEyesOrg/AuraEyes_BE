using Application.Common.Interfaces;

namespace Application.Scheduling.Appointments.Commands.MarkClinicAppointmentNoShow;

public record MarkClinicAppointmentNoShowCommand(Guid AppointmentId) : ICommand;
