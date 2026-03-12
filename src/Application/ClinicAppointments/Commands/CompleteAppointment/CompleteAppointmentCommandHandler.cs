using Application.Common.Interfaces;
using Application.Common.Models;
using Domain.Common;
using Domain.Repositories;
using Microsoft.Extensions.Logging;

namespace Application.ClinicAppointments.Commands.CompleteAppointment;

/// <summary>
/// Handler for CompleteAppointmentCommand.
/// </summary>
public class CompleteAppointmentCommandHandler : ICommandHandler<CompleteAppointmentCommand, bool>
{
    private readonly IClinicAppointmentRepository _clinicAppointmentRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<CompleteAppointmentCommandHandler> _logger;

    public CompleteAppointmentCommandHandler(
        IClinicAppointmentRepository clinicAppointmentRepository,
        IUnitOfWork unitOfWork,
        ILogger<CompleteAppointmentCommandHandler> logger)
    {
        _clinicAppointmentRepository = clinicAppointmentRepository;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<Result<bool>> Handle(CompleteAppointmentCommand request, CancellationToken cancellationToken)
    {
        var appointment = await _clinicAppointmentRepository.GetByIdAsync(request.AppointmentId, cancellationToken);

        if (appointment is null)
        {
            return Result<bool>.NotFound("Appointment not found.");
        }

        try
        {
            appointment.Complete(request.Notes);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            _logger.LogInformation(
                "Completed appointment {AppointmentId}. CompletedAt: {CompletedAt}",
                request.AppointmentId, appointment.CompletedAt);

            return Result<bool>.Success(true);
        }
        catch (InvalidOperationException ex)
        {
            return Result<bool>.Failure(ex.Message);
        }
    }
}
