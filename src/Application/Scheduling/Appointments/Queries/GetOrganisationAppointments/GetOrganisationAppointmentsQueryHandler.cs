using Application.Common.Interfaces;
using Application.Common.Models;
using Application.Scheduling.Appointments.Common;
using Domain.Common;
using Domain.Entities.Users;
using Domain.Repositories;

namespace Application.Scheduling.Appointments.Queries.GetOrganisationAppointments;

public class GetOrganisationAppointmentsQueryHandler
    : IQueryHandler<GetOrganisationAppointmentsQuery, IReadOnlyList<ClinicAppointmentDto>>
{
    private readonly IAppointmentRepository _appointmentRepository;
    private readonly IRepository<Organisation> _organisationRepository;
    private readonly ICurrentUserService _currentUser;

    public GetOrganisationAppointmentsQueryHandler(
        IAppointmentRepository appointmentRepository,
        IRepository<Organisation> organisationRepository,
        ICurrentUserService currentUser)
    {
        _appointmentRepository = appointmentRepository;
        _organisationRepository = organisationRepository;
        _currentUser = currentUser;
    }

    public async Task<Result<IReadOnlyList<ClinicAppointmentDto>>> Handle(
        GetOrganisationAppointmentsQuery request,
        CancellationToken cancellationToken)
    {
        if (_currentUser.UserId is null)
        {
            return Result<IReadOnlyList<ClinicAppointmentDto>>.Unauthorized("Authenticated user is required.");
        }

        var hasAccess = await _organisationRepository.ExistsAsync(
            o => o.Id == request.OrganisationId && o.OwnerId == _currentUser.UserId.Value,
            cancellationToken);

        if (!hasAccess)
        {
            return Result<IReadOnlyList<ClinicAppointmentDto>>.Forbidden(
                "You are not authorized to view appointments of this organisation.");
        }

        var appointments = request.Date.HasValue
            ? await _appointmentRepository.GetByOrganisationAndDateAsync(
                request.OrganisationId,
                request.Date.Value,
                cancellationToken)
            : await _appointmentRepository.GetByOrganisationAsync(
                request.OrganisationId,
                request.FromDate,
                request.ToDate,
                request.Status,
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
