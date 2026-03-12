using Application.ClinicAppointments.Common;
using Application.Common.Interfaces;
using Application.Common.Models;
using Domain.Entities.Scheduling;
using Domain.Repositories;

namespace Application.ClinicAppointments.Queries.GetPatientClinicAppointments;

/// <summary>
/// Handler for GetPatientClinicAppointmentsQuery.
/// </summary>
public class GetPatientClinicAppointmentsQueryHandler : IQueryHandler<GetPatientClinicAppointmentsQuery, IReadOnlyList<ClinicAppointmentListDto>>
{
    private readonly IClinicAppointmentRepository _clinicAppointmentRepository;

    public GetPatientClinicAppointmentsQueryHandler(IClinicAppointmentRepository clinicAppointmentRepository)
    {
        _clinicAppointmentRepository = clinicAppointmentRepository;
    }

    public async Task<Result<IReadOnlyList<ClinicAppointmentListDto>>> Handle(
        GetPatientClinicAppointmentsQuery request, 
        CancellationToken cancellationToken)
    {
        IReadOnlyList<ClinicAppointment> appointments;

        if (request.UpcomingOnly)
        {
            appointments = await _clinicAppointmentRepository.GetUpcomingByPatientAsync(
                request.PatientId, cancellationToken);
        }
        else
        {
            appointments = await _clinicAppointmentRepository.GetByPatientAsync(
                request.PatientId, request.Status, cancellationToken);
        }

        var dtos = appointments.Select(a => new ClinicAppointmentListDto
        {
            Id = a.Id,
            PatientId = a.PatientId,
            PatientName = "Patient",  // Name is in ApplicationUser, not directly accessible here
            PatientEmail = "",
            OrganisationId = a.OrganisationId,
            OrganisationName = a.Organisation?.Name ?? "Unknown",
            Date = a.AppointmentSlot?.Date ?? default,
            StartTime = a.AppointmentSlot?.StartTime ?? default,
            EndTime = a.AppointmentSlot?.EndTime ?? default,
            Status = a.Status.ToString(),
            VisitReason = a.VisitReason,
            AssignedDoctorId = a.AssignedDoctorId,
            AssignedDoctorName = null,  // Doctor name is in ApplicationUser
            CreatedAt = a.CreatedAt
        }).ToList();

        return Result<IReadOnlyList<ClinicAppointmentListDto>>.Success(dtos);
    }
}
