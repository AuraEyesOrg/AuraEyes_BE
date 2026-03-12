using Application.Common.Interfaces;
using Application.Common.Models;
using Domain.Common;
using Domain.Repositories;
using Microsoft.Extensions.Logging;

namespace Application.ClinicAppointments.Commands.StartAppointment;

/// <summary>
/// Handler for StartAppointmentCommand.
/// </summary>
public class StartAppointmentCommandHandler : ICommandHandler<StartAppointmentCommand, bool>
{
    private readonly IClinicAppointmentRepository _clinicAppointmentRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<StartAppointmentCommandHandler> _logger;

    public StartAppointmentCommandHandler(
        IClinicAppointmentRepository clinicAppointmentRepository,
        IUnitOfWork unitOfWork,
        ILogger<StartAppointmentCommandHandler> logger)
    {
        _clinicAppointmentRepository = clinicAppointmentRepository;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<Result<bool>> Handle(StartAppointmentCommand request, CancellationToken cancellationToken)
    {
        var appointment = await _clinicAppointmentRepository.GetByIdAsync(request.AppointmentId, cancellationToken);

        if (appointment is null)
        {
            return Result<bool>.NotFound("Appointment not found.");
        }

        try
        {
            appointment.Start();
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            _logger.LogInformation(
                "Started appointment {AppointmentId}. Doctor: {DoctorId}",
                request.AppointmentId, appointment.AssignedDoctorId);

            return Result<bool>.Success(true);
        }
        catch (InvalidOperationException ex)
        {
            return Result<bool>.Failure(ex.Message);
        }
    }
}
