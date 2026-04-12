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
    private readonly IIdentityService _identityService;

    public GetOrganisationAppointmentsQueryHandler(
        IAppointmentRepository appointmentRepository,
        IRepository<Organisation> organisationRepository,
        ICurrentUserService currentUser,
        IIdentityService identityService)
    {
        _appointmentRepository = appointmentRepository;
        _organisationRepository = organisationRepository;
        _currentUser = currentUser;
        _identityService = identityService;
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

        // Build identity lookup for registered patients only (those with UserId)
        var registeredPatientUserIds = appointments
            .Where(a => a.Patient is not null && a.Patient.UserId.HasValue)
            .Select(a => a.Patient!.UserId!.Value)
            .Distinct()
            .ToList();

        // Sequential fetch — must NOT use Task.WhenAll here because all tasks share
        // the same scoped DbContext and concurrent operations throw:
        // "A second operation was started on this context instance before a previous operation completed."
        var patientLookup = new Dictionary<Guid, UserDto>();
        foreach (var userId in registeredPatientUserIds)
        {
            var user = await _identityService.GetUserByIdAsync(userId, cancellationToken);
            if (user is not null)
            {
                patientLookup[userId] = user;
            }
        }

        var data = appointments
            .Where(a => a.AppointmentSlot is not null)
            .Select(a =>
            {
                // Resolve patient display name: walk-in → entity fields, registered → Identity
                string patientName;
                string? patientAvatarUrl = null;

                if (a.Patient is not null && a.Patient.IsWalkIn)
                {
                    patientName = !string.IsNullOrWhiteSpace(a.Patient.FullName)
                        ? a.Patient.FullName
                        : $"Patient {a.PatientId.ToString()[..8]}";
                }
                else if (a.Patient is not null
                         && a.Patient.UserId.HasValue
                         && patientLookup.TryGetValue(a.Patient.UserId.Value, out var user))
                {
                    patientName = !string.IsNullOrWhiteSpace(user.FullName)
                        ? user.FullName
                        : !string.IsNullOrWhiteSpace(user.Email)
                            ? user.Email
                            : $"Patient {a.PatientId.ToString()[..8]}";
                    patientAvatarUrl = user.AvatarUrl;
                }
                else
                {
                    patientName = $"Patient {a.PatientId.ToString()[..8]}";
                }

                return new ClinicAppointmentDto
                {
                    Id = a.Id,
                    PatientId = a.PatientId,
                    PatientName = patientName,
                    PatientAvatarUrl = patientAvatarUrl,
                    OrganisationId = a.OrganisationId ?? Guid.Empty,
                    OrganisationName = a.Organisation?.Name,
                    SlotId = a.AppointmentSlotId,
                    Date = a.AppointmentSlot!.Date,
                    StartTime = a.AppointmentSlot.StartTime,
                    EndTime = a.AppointmentSlot.EndTime,
                    VisitReason = a.VisitReason,
                    Status = a.Status,
                    CreatedAt = a.CreatedAt
                };
            })
            .ToList();

        return Result<IReadOnlyList<ClinicAppointmentDto>>.Success(data);
    }
}
