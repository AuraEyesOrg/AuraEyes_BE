using Application.Common.Interfaces;
using Application.Common.Models;
using Domain.Common;
using Domain.Repositories;

namespace Application.Scheduling.Appointments.Commands.MarkClinicAppointmentNoShow;

public class MarkClinicAppointmentNoShowCommandHandler : ICommandHandler<MarkClinicAppointmentNoShowCommand>
{
    private readonly IAppointmentRepository _appointmentRepository;
    private readonly ICurrentUserService _currentUser;
    private readonly IUnitOfWork _unitOfWork;

    public MarkClinicAppointmentNoShowCommandHandler(
        IAppointmentRepository appointmentRepository,
        ICurrentUserService currentUser,
        IUnitOfWork unitOfWork)
    {
        _appointmentRepository = appointmentRepository;
        _currentUser = currentUser;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> Handle(MarkClinicAppointmentNoShowCommand request, CancellationToken cancellationToken)
    {
        var appointment = await _appointmentRepository.GetByIdWithDetailsAsync(request.AppointmentId, cancellationToken);
        if (appointment is null)
        {
            return Result.NotFound($"Clinic appointment '{request.AppointmentId}' not found.");
        }

        // Check if user is staff/admin
        if (!_currentUser.IsInRole("SystemAdmin") && !_currentUser.IsInRole("Staff"))
        {
            return Result.Forbidden("Only clinic staff can mark an appointment as no-show.");
        }

        try
        {
            appointment.MarkNoShow();
            await _appointmentRepository.UpdateAsync(appointment, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return Result.Success();
        }
        catch (InvalidOperationException ex)
        {
            return Result.Failure(ex.Message);
        }
    }
}
