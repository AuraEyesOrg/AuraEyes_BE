using Application.Common.Interfaces;
using Application.Common.Models;
using Domain.Common;
using Domain.Entities.Screening;
using Domain.Entities.Users;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Application.Screenings.Commands.CreateClinicScreeningSession;

public class CreateClinicScreeningSessionCommandHandler 
    : IRequestHandler<CreateClinicScreeningSessionCommand, Result<Guid>>
{
    private readonly IRepository<AiScreening> _screeningRepository;
    private readonly IRepository<Patient> _patientRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<CreateClinicScreeningSessionCommandHandler> _logger;

    public CreateClinicScreeningSessionCommandHandler(
        IRepository<AiScreening> screeningRepository,
        IRepository<Patient> patientRepository,
        IUnitOfWork unitOfWork,
        ILogger<CreateClinicScreeningSessionCommandHandler> logger)
    {
        _screeningRepository = screeningRepository;
        _patientRepository = patientRepository;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<Result<Guid>> Handle(
        CreateClinicScreeningSessionCommand request,
        CancellationToken cancellationToken)
    {
        var patient = await _patientRepository.GetByIdAsync(request.PatientId, cancellationToken);
        if (patient == null)
        {
            return Result<Guid>.NotFound("Patient not found");
        }

        var screening = new AiScreening(patient.Id, request.ModelVersion);

        if (request.RetinalImages != null && request.RetinalImages.Any())
        {
            foreach (var imgData in request.RetinalImages)
            {
                var retinalImage = new RetinalImage(
                    patientId: patient.Id,
                    imageUrl: imgData.ImageUrl,
                    eyeSide: imgData.EyeSide,
                    capturedAt: DateTime.UtcNow,
                    deviceName: imgData.DeviceName);
                
                retinalImage.AssignToScreening(screening.Id);
                screening.AddRetinalImage(retinalImage);
            }
        }

        try
        {
            await _screeningRepository.AddAsync(screening, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("Clinic staff created screening session {ScreeningId} for patient {PatientId}", 
                screening.Id, patient.Id);

            return Result<Guid>.Success(screening.Id);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating clinic screening session for patient {PatientId}", patient.Id);
            return Result<Guid>.Failure($"Failed to create screening session: {ex.Message}");
        }
    }
}
