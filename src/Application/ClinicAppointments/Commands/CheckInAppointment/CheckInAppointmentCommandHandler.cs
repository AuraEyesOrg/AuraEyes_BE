using Application.Common.Interfaces;
using Application.Common.Models;
using Domain.Common;
using Domain.Repositories;
using Microsoft.Extensions.Logging;

namespace Application.ClinicAppointments.Commands.CheckInAppointment;

/// <summary>
/// Handler for CheckInAppointmentCommand.
/// </summary>
public class CheckInAppointmentCommandHandler : ICommandHandler<CheckInAppointmentCommand, bool>
{
    private readonly IClinicAppointmentRepository _clinicAppointmentRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<CheckInAppointmentCommandHandler> _logger;

    public CheckInAppointmentCommandHandler(
        IClinicAppointmentRepository clinicAppointmentRepository,
        IUnitOfWork unitOfWork,
        ILogger<CheckInAppointmentCommandHandler> logger)
    {
        _clinicAppointmentRepository = clinicAppointmentRepository;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<Result<bool>> Handle(CheckInAppointmentCommand request, CancellationToken cancellationToken)
    {
        var appointment = await _clinicAppointmentRepository.GetByIdAsync(request.AppointmentId, cancellationToken);

        if (appointment is null)
        {
            return Result<bool>.NotFound("Appointment not found.");
        }

        try
        {
            appointment.CheckIn();
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            _logger.LogInformation(
                "Patient checked in for appointment {AppointmentId}. Time: {CheckedInAt}",
                request.AppointmentId, appointment.CheckedInAt);

            return Result<bool>.Success(true);
        }
        catch (InvalidOperationException ex)
        {
            return Result<bool>.Failure(ex.Message);
        }
    }
}
