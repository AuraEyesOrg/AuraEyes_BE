using Application.Common.Interfaces;
using Application.Common.Models;
using Application.Screenings.Commands.CreateAiScreeningSession;
using Domain.Common;
using Domain.Entities.Screening;
using Domain.Entities.Users;
using Domain.Repositories;
using Microsoft.Extensions.Logging;

namespace Application.OrganisationScreenings.Commands.CreateOrgScreeningSession;

public class CreateOrgScreeningSessionCommandHandler
    : ICommandHandler<CreateOrgScreeningSessionCommand, CreateOrgScreeningSessionResponse>
{
    private const string OrgScreeningConsentContent =
        "Organisation-initiated AI screening was authorized and recorded on behalf of the patient.";

    private readonly IRepository<AiScreening> _screeningRepository;
    private readonly IRepository<Patient> _patientRepository;
    private readonly IOrganisationPatientsRepository _organisationPatientsRepository;
    private readonly ICurrentUserService _currentUserService;
    private readonly IIdentityService _identityService;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<CreateOrgScreeningSessionCommandHandler> _logger;

    public CreateOrgScreeningSessionCommandHandler(
        IRepository<AiScreening> screeningRepository,
        IRepository<Patient> patientRepository,
        IOrganisationPatientsRepository organisationPatientsRepository,
        ICurrentUserService currentUserService,
        IIdentityService identityService,
        IUnitOfWork unitOfWork,
        ILogger<CreateOrgScreeningSessionCommandHandler> logger)
    {
        _screeningRepository = screeningRepository;
        _patientRepository = patientRepository;
        _organisationPatientsRepository = organisationPatientsRepository;
        _currentUserService = currentUserService;
        _identityService = identityService;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<Result<CreateOrgScreeningSessionResponse>> Handle(
        CreateOrgScreeningSessionCommand request,
        CancellationToken cancellationToken)
    {
        var userId = _currentUserService.UserId;
        if (userId is null)
            return Result<CreateOrgScreeningSessionResponse>.Unauthorized("User not authenticated");

        var orgAdminUser = await _identityService.GetUserByIdAsync(userId.Value, cancellationToken);
        if (orgAdminUser?.OrganizationId is null)
            return Result<CreateOrgScreeningSessionResponse>.Forbidden("Organisation is not assigned for this account");

        // Verify patient exists
        var patient = await _patientRepository.GetByIdAsync(request.PatientId, cancellationToken);
        if (patient is null)
        {
            _logger.LogWarning("Patient {PatientId} not found for org screening", request.PatientId);
            return Result<CreateOrgScreeningSessionResponse>.NotFound("Patient not found");
        }

        var hasPatientAccess = await _organisationPatientsRepository.IsPatientManagedByOrganisationAdminAsync(
            userId.Value,
            patient.Id,
            cancellationToken);
        if (!hasPatientAccess)
        {
            _logger.LogWarning(
                "OrgAdmin {UserId} attempted to create screening for unauthorized patient {PatientId}",
                userId.Value,
                patient.Id);
            return Result<CreateOrgScreeningSessionResponse>.NotFound("Patient not found");
        }

        // Create new screening session for the patient
        var screening = new AiScreening(patient.Id, request.ModelVersion, orgAdminUser.OrganizationId.Value);

        // Organisation flow is performed by staff on behalf of the patient.
        // Record consent at session creation so the AI result persistence step remains valid.
        screening.RecordConsent(patient.Id, OrgScreeningConsentContent);

        // Add retinal images
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

        try
        {
            await _screeningRepository.AddAsync(screening, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            _logger.LogInformation(
                "OrgAdmin {UserId} created screening {ScreeningId} for patient {PatientId}, images {Count}",
                userId.Value, screening.Id, patient.Id, savedImages.Count);

            return Result<CreateOrgScreeningSessionResponse>.Success(new CreateOrgScreeningSessionResponse
            {
                ScreeningId = screening.Id,
                PatientId = patient.Id,
                ModelVersion = screening.ModelVersion,
                Images = savedImages,
                CreatedAt = screening.CreatedAt
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating org screening for patient {PatientId}", patient.Id);
            return Result<CreateOrgScreeningSessionResponse>.Failure($"Failed to create screening: {ex.Message}");
        }
    }
}
