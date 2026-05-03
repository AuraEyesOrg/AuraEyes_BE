using Application.Common.Interfaces;
using Application.Common.Models;
using Application.Scheduling.Appointments.Common;

namespace Application.Scheduling.Appointments.Queries.GetPendingCancellations;

public record GetPendingCancellationsQuery(int PageNumber = 1, int PageSize = 10) : IQuery<PagedResult<ClinicAppointmentDto>>;
