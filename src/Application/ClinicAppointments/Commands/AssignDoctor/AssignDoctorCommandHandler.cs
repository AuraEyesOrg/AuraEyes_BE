using Application.Common.Interfaces;
using Application.Common.Models;
using Domain.Common;
using Domain.Repositories;
using Microsoft.Extensions.Logging;

namespace Application.ClinicAppointments.Commands.AssignDoctor;

/// <summary>
/// Handler for AssignDoctorCommand.
/// </summary>
public class AssignDoctorCommandHandler : ICommandHandler<AssignDoctorCommand, bool>
{
    private readonly IClinicAppointmentRepository _clinicAppointmentRepository;
    private readonly IOphthalmologistRepository _ophthalmologistRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<AssignDoctorCommandHandler> _logger;

    public AssignDoctorCommandHandler(
        IClinicAppointmentRepository clinicAppointmentRepository,
        IOphthalmologistRepository ophthalmologistRepository,
        IUnitOfWork unitOfWork,
        ILogger<AssignDoctorCommandHandler> logger)
    {
        _clinicAppointmentRepository = clinicAppointmentRepository;
        _ophthalmologistRepository = ophthalmologistRepository;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<Result<bool>> Handle(AssignDoctorCommand request, CancellationToken cancellationToken)
    {
        var appointment = await _clinicAppointmentRepository.GetByIdAsync(request.AppointmentId, cancellationToken);

        if (appointment is null)
        {
            return Result<bool>.NotFound("Appointment not found.");
        }

        // Verify doctor exists
        var doctor = await _ophthalmologistRepository.GetByIdAsync(request.DoctorId, cancellationToken);
        if (doctor is null)
        {
            return Result<bool>.NotFound("Doctor not found.");
        }

        try
        {
            appointment.AssignDoctor(request.DoctorId);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            _logger.LogInformation(
                "Assigned doctor {DoctorId} to appointment {AppointmentId}",
                request.DoctorId, request.AppointmentId);

            return Result<bool>.Success(true);
        }
        catch (InvalidOperationException ex)
        {
            return Result<bool>.Failure(ex.Message);
        }
    }
}
