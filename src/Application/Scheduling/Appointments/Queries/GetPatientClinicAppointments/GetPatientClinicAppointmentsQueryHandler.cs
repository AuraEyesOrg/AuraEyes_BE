using Application.Common.Interfaces;
using Application.Common.Models;
using Application.Scheduling.Appointments.Common;
using Domain.Enums;
using Domain.Repositories;

namespace Application.Scheduling.Appointments.Queries.GetPatientClinicAppointments;

public class GetPatientClinicAppointmentsQueryHandler
    : IQueryHandler<GetPatientClinicAppointmentsQuery, IReadOnlyList<ClinicAppointmentDto>>
{
    private readonly IAppointmentRepository _appointmentRepository;
    private readonly ICurrentUserService _currentUser;

    public GetPatientClinicAppointmentsQueryHandler(
        IAppointmentRepository appointmentRepository,
        ICurrentUserService currentUser)
    {
        _appointmentRepository = appointmentRepository;
        _currentUser = currentUser;
    }

    public async Task<Result<IReadOnlyList<ClinicAppointmentDto>>> Handle(
        GetPatientClinicAppointmentsQuery request,
        CancellationToken cancellationToken)
    {
        if (_currentUser.ProfileId is null)
        {
            return Result<IReadOnlyList<ClinicAppointmentDto>>.Unauthorized("Patient profile is required.");
        }

        if (_currentUser.ProfileId.Value != request.PatientId)
        {
            return Result<IReadOnlyList<ClinicAppointmentDto>>.Forbidden(
                "You can only view your own clinic appointments.");
        }

        var appointments = await _appointmentRepository.GetByPatientAsync(
            request.PatientId,
            AppointmentType.ClinicVisit,
            null,
            cancellationToken);

        var data = appointments
            .Where(a => a.AppointmentSlot is not null)
            .Select(a => new ClinicAppointmentDto
            {
                Id = a.Id,
                PatientId = a.PatientId,
                OrganisationId = a.OrganisationId ?? Guid.Empty,
                OrganisationName = a.Organisation?.Name,
                SlotId = a.AppointmentSlotId,
                Date = a.AppointmentSlot!.Date,
                StartTime = a.AppointmentSlot.StartTime,
                EndTime = a.AppointmentSlot.EndTime,
                VisitReason = a.VisitReason,
                Status = a.Status,
                CreatedAt = a.CreatedAt
            })
            .ToList();

        return Result<IReadOnlyList<ClinicAppointmentDto>>.Success(data);
    }
}
