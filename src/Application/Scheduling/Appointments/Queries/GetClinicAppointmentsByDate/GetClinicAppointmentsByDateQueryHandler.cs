using Application.Common.Interfaces;
using Application.Common.Models;
using Application.Scheduling.Appointments.Common;
using Domain.Repositories;

namespace Application.Scheduling.Appointments.Queries.GetClinicAppointmentsByDate;

public class GetClinicAppointmentsByDateQueryHandler
    : IQueryHandler<GetClinicAppointmentsByDateQuery, IReadOnlyList<ClinicAppointmentDto>>
{
    private readonly IAppointmentRepository _appointmentRepository;

    public GetClinicAppointmentsByDateQueryHandler(IAppointmentRepository appointmentRepository)
    {
        _appointmentRepository = appointmentRepository;
    }

    public async Task<Result<IReadOnlyList<ClinicAppointmentDto>>> Handle(
        GetClinicAppointmentsByDateQuery request,
        CancellationToken cancellationToken)
    {
        var (appointments, _) = await _appointmentRepository.GetPagedAsync(
            fromDate: request.Date,
            toDate: request.Date,
            pageNumber: 1,
            pageSize: 500,
            cancellationToken: cancellationToken);

        var items = appointments
            .Where(a => a.AppointmentSlot is not null)
            .Select(a => new ClinicAppointmentDto
            {
                Id = a.Id,
                PatientId = a.PatientId,
                PatientName = a.Patient?.FullName,
                PatientAvatarUrl = null,
                SlotId = a.AppointmentSlotId,
                Date = a.AppointmentSlot!.Date,
                StartTime = a.AppointmentSlot.StartTime,
                EndTime = a.AppointmentSlot.EndTime,
                VisitReason = a.VisitReason,
                Status = a.Status,
                CreatedAt = a.CreatedAt,
                HasFeedback = false
            })
            .OrderBy(x => x.Date)
            .ThenBy(x => x.StartTime)
            .ToList();

        return Result<IReadOnlyList<ClinicAppointmentDto>>.Success(items);
    }
}
