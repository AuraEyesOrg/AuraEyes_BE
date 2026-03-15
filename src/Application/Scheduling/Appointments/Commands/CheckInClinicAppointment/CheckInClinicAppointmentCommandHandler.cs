using Application.Common.Interfaces;
using Application.Common.Models;
using Domain.Common;
using Domain.Entities.Users;
using Domain.Enums;
using Domain.Repositories;

namespace Application.Scheduling.Appointments.Commands.CheckInClinicAppointment;

public class CheckInClinicAppointmentCommandHandler : ICommandHandler<CheckInClinicAppointmentCommand>
{
    private readonly IAppointmentRepository _appointmentRepository;
    private readonly IRepository<Organisation> _organisationRepository;
    private readonly ICurrentUserService _currentUser;
    private readonly IUnitOfWork _unitOfWork;

    public CheckInClinicAppointmentCommandHandler(
        IAppointmentRepository appointmentRepository,
        IRepository<Organisation> organisationRepository,
        ICurrentUserService currentUser,
        IUnitOfWork unitOfWork)
    {
        _appointmentRepository = appointmentRepository;
        _organisationRepository = organisationRepository;
        _currentUser = currentUser;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> Handle(CheckInClinicAppointmentCommand request, CancellationToken cancellationToken)
    {
        var appointment = await _appointmentRepository.GetByIdWithDetailsAsync(request.AppointmentId, cancellationToken);
        if (appointment is null)
        {
            return Result.NotFound($"Clinic appointment '{request.AppointmentId}' not found.");
        }

        if (appointment.Type != AppointmentType.ClinicVisit)
        {
            return Result.Failure("Only clinic visit appointments support check-in.");
        }

        var access = await HasOrganisationAccessAsync(appointment.OrganisationId, cancellationToken);
        if (!access.IsSuccess)
        {
            return access;
        }

        try
        {
            appointment.CheckIn();
            await _appointmentRepository.UpdateAsync(appointment, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return Result.Success();
        }
        catch (InvalidOperationException ex)
        {
            return Result.Failure(ex.Message);
        }
    }

    private async Task<Result> HasOrganisationAccessAsync(Guid? organisationId, CancellationToken cancellationToken)
    {
        if (_currentUser.UserId is null)
        {
            return Result.Unauthorized("Authenticated user is required.");
        }

        if (!organisationId.HasValue)
        {
            return Result.Failure("Appointment does not belong to an organisation.");
        }

        var hasAccess = await _organisationRepository.ExistsAsync(
            o => o.Id == organisationId.Value && o.OwnerId == _currentUser.UserId.Value,
            cancellationToken);

        return hasAccess
            ? Result.Success()
            : Result.Forbidden("You are not authorized to operate appointments of this organisation.");
    }
}
