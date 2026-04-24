using Application.Common.Interfaces;
using Application.Common.Models;
using Domain.Common;
using Domain.Entities.Screening;
using Domain.Entities.Users;
using Domain.Repositories;
using Microsoft.Extensions.Logging;

namespace Application.ClinicScreenings.Commands.CreateClinicScreeningSession;

public class CreateClinicScreeningSessionCommandHandler
    : ICommandHandler<CreateClinicScreeningSessionCommand, CreateClinicScreeningSessionResponse>
{
    private const string ClinicScreeningConsentContent =
        "Clinic-initiated AI screening was authorized and recorded on behalf of the patient.";

    private readonly IRepository<AiScreening> _screeningRepository;
    private readonly IRepository<Patient> _patientRepository;
    private readonly IOrganisationPatientsRepository _organisationPatientsRepository;
    private readonly ICurrentUserService _currentUserService;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<CreateClinicScreeningSessionCommandHandler> _logger;

    public CreateClinicScreeningSessionCommandHandler(
        IRepository<AiScreening> screeningRepository,
        IRepository<Patient> patientRepository,
        IOrganisationPatientsRepository organisationPatientsRepository,
        ICurrentUserService currentUserService,
        IUnitOfWork unitOfWork,
        ILogger<CreateClinicScreeningSessionCommandHandler> logger)
    {
        _screeningRepository = screeningRepository;
        _patientRepository = patientRepository;
        _organisationPatientsRepository = organisationPatientsRepository;
        _currentUserService = currentUserService;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<Result<CreateClinicScreeningSessionResponse>> Handle(
        CreateClinicScreeningSessionCommand request,
        CancellationToken cancellationToken)
    {
        var userId = _currentUserService.UserId;
        if (userId is null)
            return Result<CreateClinicScreeningSessionResponse>.Unauthorized("User not authenticated");

        var patient = await _patientRepository.GetByIdAsync(request.PatientId, cancellationToken);
        if (patient is null)
            return Result<CreateClinicScreeningSessionResponse>.NotFound("Patient not found");

        var hasPatientAccess = await _organisationPatientsRepository.IsPatientManagedByOrganisationAdminAsync(
            userId.Value,
            patient.Id,
            cancellationToken);
        if (!hasPatientAccess)
            return Result<CreateClinicScreeningSessionResponse>.NotFound("Patient not found");

        // Clinic-owned AI: session does not depend on organisation quota.
        var screening = new AiScreening(patient.Id, request.ModelVersion, null);
        screening.RecordConsent(patient.Id, ClinicScreeningConsentContent);

        var savedImages = new List<RetinalImageResponse>();
        foreach (var imageData in request.RetinalImages)
        {
            if (string.IsNullOrWhiteSpace(imageData.ImageUrl))
                continue;

            var retinalImage = new RetinalImage(
                patientId: patient.Id,
                imageUrl: imageData.ImageUrl,
                eyeSide: imageData.EyeSide,
                capturedAt: DateTime.UtcNow,
                deviceName: imageData.DeviceName);

            retinalImage.AssignToScreening(screening.Id);
            screening.AddRetinalImage(retinalImage);

            savedImages.Add(new RetinalImageResponse
            {
                Id = retinalImage.Id,
                ImageUrl = imageData.ImageUrl,
                EyeSide = imageData.EyeSide.ToString()
            });
        }

        await _screeningRepository.AddAsync(screening, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        _logger.LogInformation(
            "Clinic staff {UserId} created screening {ScreeningId} for patient {PatientId}, images {Count}",
            userId.Value,
            screening.Id,
            patient.Id,
            savedImages.Count);

        return Result<CreateClinicScreeningSessionResponse>.Success(new CreateClinicScreeningSessionResponse
        {
            ScreeningId = screening.Id,
            PatientId = patient.Id,
            ModelVersion = screening.ModelVersion,
            Images = savedImages,
            CreatedAt = screening.CreatedAt
        });
    }
}
