using Application.ClinicAppointments.Common;
using Application.Common.Interfaces;
using Application.Common.Models;
using Domain.Repositories;

namespace Application.ClinicAppointments.Queries.GetClinicAppointmentDetail;

/// <summary>
/// Handler for GetClinicAppointmentDetailQuery.
/// </summary>
public class GetClinicAppointmentDetailQueryHandler : IQueryHandler<GetClinicAppointmentDetailQuery, ClinicAppointmentDetailDto>
{
    private readonly IClinicAppointmentRepository _clinicAppointmentRepository;

    public GetClinicAppointmentDetailQueryHandler(IClinicAppointmentRepository clinicAppointmentRepository)
    {
        _clinicAppointmentRepository = clinicAppointmentRepository;
    }

    public async Task<Result<ClinicAppointmentDetailDto>> Handle(
        GetClinicAppointmentDetailQuery request, 
        CancellationToken cancellationToken)
    {
        var appointment = await _clinicAppointmentRepository.GetByIdWithDetailsAsync(
            request.AppointmentId, cancellationToken);

        if (appointment is null)
        {
            return Result<ClinicAppointmentDetailDto>.NotFound("Appointment not found.");
        }

        var dto = new ClinicAppointmentDetailDto
        {
            Id = appointment.Id,
            PatientId = appointment.PatientId,
            PatientName = "Patient",  // Name is in ApplicationUser
            PatientEmail = "",
            PatientPhone = null,
            OrganisationId = appointment.OrganisationId,
            OrganisationName = appointment.Organisation?.Name ?? "Unknown",
            OrganisationAddress = appointment.Organisation?.Address,
            Date = appointment.AppointmentSlot?.Date ?? default,
            StartTime = appointment.AppointmentSlot?.StartTime ?? default,
            EndTime = appointment.AppointmentSlot?.EndTime ?? default,
            Status = appointment.Status.ToString(),
            VisitReason = appointment.VisitReason,
            Notes = appointment.Notes,
            AssignedDoctorId = appointment.AssignedDoctorId,
            AssignedDoctorName = null,  // Doctor name is in ApplicationUser
            CheckedInAt = appointment.CheckedInAt,
            CompletedAt = appointment.CompletedAt,
            CancelledBy = appointment.CancelledBy,
            CancellationReason = appointment.CancellationReason,
            CreatedAt = appointment.CreatedAt,
            UpdatedAt = appointment.UpdatedAt
        };

        return Result<ClinicAppointmentDetailDto>.Success(dto);
    }
}
