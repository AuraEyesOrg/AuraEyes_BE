using Application.Common.Interfaces;
using Application.Common.Models;
using Domain.Common;
using Domain.Entities.Screening;
using Domain.Entities.Users;
using Microsoft.Extensions.Logging;

namespace Application.Screenings.Commands.CreateAiScreeningSession;

/// <summary>
/// Handler for CreateAiScreeningSessionCommand
/// Creates a new AI screening session for the current patient.
/// </summary>
public class CreateAiScreeningSessionCommandHandler : ICommandHandler<CreateAiScreeningSessionCommand, CreateAiScreeningSessionResponse>
{
    private readonly IRepository<AiScreening> _screeningRepository;
    private readonly IRepository<Patient> _patientRepository;
    private readonly ICurrentUserService _currentUserService;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<CreateAiScreeningSessionCommandHandler> _logger;

    public CreateAiScreeningSessionCommandHandler(
        IRepository<AiScreening> screeningRepository,
        IRepository<Patient> patientRepository,
        ICurrentUserService currentUserService,
        IUnitOfWork unitOfWork,
        ILogger<CreateAiScreeningSessionCommandHandler> logger)
    {
        _screeningRepository = screeningRepository;
        _patientRepository = patientRepository;
        _currentUserService = currentUserService;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<Result<CreateAiScreeningSessionResponse>> Handle(
        CreateAiScreeningSessionCommand request,
        CancellationToken cancellationToken)
    {
        var userId = _currentUserService.UserId;
        if (userId is null)
        {
            _logger.LogWarning("User is not authenticated");
            return Result<CreateAiScreeningSessionResponse>.Unauthorized("User not authenticated");
        }

        // Get patient profile by user ID
        var patients = await _patientRepository.FindAsync(
            p => p.UserId == userId.Value,
            cancellationToken);

        var patient = patients.FirstOrDefault();
        if (patient is null)
        {
            _logger.LogWarning("Patient profile not found for user {UserId}", userId.Value);
            return Result<CreateAiScreeningSessionResponse>.NotFound("Patient profile not found");
        }

        // Create new screening session
        var screening = new AiScreening(patient.Id, request.ModelVersion);

        // Add retinal images if provided
        var savedImages = new List<RetinalImageResponse>();
        if (request.RetinalImages.Count > 0)
        {
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

                // Link to screening
                retinalImage.AssignToScreening(screening.Id);
                screening.AddRetinalImage(retinalImage);

                savedImages.Add(new RetinalImageResponse
                {
                    Id = retinalImage.Id,
                    ImageUrl = imageData.ImageUrl,
                    EyeSide = imageData.EyeSide.ToString()
                });
            }
        }

        try
        {
            await _screeningRepository.AddAsync(screening, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            _logger.LogInformation(
                "Created new AI screening session {ScreeningId} for patient {PatientId}, model {ModelVersion}, images {ImageCount}",
                screening.Id, patient.Id, request.ModelVersion, savedImages.Count);

            return Result<CreateAiScreeningSessionResponse>.Success(new CreateAiScreeningSessionResponse
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
            _logger.LogError(ex, "Error creating AI screening session for patient {PatientId}", patient.Id);
            return Result<CreateAiScreeningSessionResponse>.Failure($"Failed to create screening session: {ex.Message}");
        }
    }
}
