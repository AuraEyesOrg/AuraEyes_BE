using Application.Common.Interfaces;
using Application.Common.Models;
using Domain.Common;
using Domain.Repositories;
using Microsoft.Extensions.Logging;

namespace Application.ClinicAppointments.Commands.MarkNoShow;

/// <summary>
/// Handler for MarkNoShowCommand.
/// </summary>
public class MarkNoShowCommandHandler : ICommandHandler<MarkNoShowCommand, bool>
{
    private readonly IClinicAppointmentRepository _clinicAppointmentRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<MarkNoShowCommandHandler> _logger;

    public MarkNoShowCommandHandler(
        IClinicAppointmentRepository clinicAppointmentRepository,
        IUnitOfWork unitOfWork,
        ILogger<MarkNoShowCommandHandler> logger)
    {
        _clinicAppointmentRepository = clinicAppointmentRepository;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<Result<bool>> Handle(MarkNoShowCommand request, CancellationToken cancellationToken)
    {
        var appointment = await _clinicAppointmentRepository.GetByIdAsync(request.AppointmentId, cancellationToken);

        if (appointment is null)
        {
            return Result<bool>.NotFound("Appointment not found.");
        }

        try
        {
            appointment.MarkNoShow();
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            _logger.LogInformation(
                "Marked appointment {AppointmentId} as no-show. Patient: {PatientId}",
                request.AppointmentId, appointment.PatientId);

            return Result<bool>.Success(true);
        }
        catch (InvalidOperationException ex)
        {
            return Result<bool>.Failure(ex.Message);
        }
    }
}
