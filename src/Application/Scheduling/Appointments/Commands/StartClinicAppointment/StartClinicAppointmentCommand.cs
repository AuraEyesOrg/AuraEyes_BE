using Application.Common.Interfaces;

namespace Application.Scheduling.Appointments.Commands.StartClinicAppointment;

public record StartClinicAppointmentCommand(Guid AppointmentId) : ICommand;
