using Application.Common.Interfaces;
using Application.Scheduling.Appointments.Common;

namespace Application.Scheduling.Appointments.Queries.GetClinicAppointmentsByDate;

public record GetClinicAppointmentsByDateQuery(DateOnly? Date) : IQuery<IReadOnlyList<ClinicAppointmentDto>>;
