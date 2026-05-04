using Application.Common.Interfaces;
using Application.Common.Models;
using Domain.Common;
using Domain.Entities.Screening;
using Domain.Entities.Users;
using Domain.Enums;
using Domain.Repositories;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;

namespace Application.ClinicScreenings.Commands.CreateClinicScreeningSession;

public class CreateClinicScreeningSessionCommandHandler
    : ICommandHandler<CreateClinicScreeningSessionCommand, CreateClinicScreeningSessionResponse>
{
    private const string ClinicQueueCacheKey = "clinic_queue_all";

    private readonly IRepository<AiScreening> _screeningRepository;
    private readonly IRepository<Patient> _patientRepository;
    private readonly IPatientVisitRepository _patientVisitRepository;
    private readonly ICurrentUserService _currentUserService;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMemoryCache _memoryCache;
    private readonly ILogger<CreateClinicScreeningSessionCommandHandler> _logger;

    public CreateClinicScreeningSessionCommandHandler(
        IRepository<AiScreening> screeningRepository,
        IRepository<Patient> patientRepository,
        IPatientVisitRepository patientVisitRepository,
        ICurrentUserService currentUserService,
        IUnitOfWork unitOfWork,
        IMemoryCache memoryCache,
        ILogger<CreateClinicScreeningSessionCommandHandler> logger)
    {
        _screeningRepository = screeningRepository;
        _patientRepository = patientRepository;
        _patientVisitRepository = patientVisitRepository;
        _currentUserService = currentUserService;
        _unitOfWork = unitOfWork;
        _memoryCache = memoryCache;
        _logger = logger;
    }

    public async Task<Result<CreateClinicScreeningSessionResponse>> Handle(
        CreateClinicScreeningSessionCommand request,
        CancellationToken cancellationToken)
    {
        var userId = _currentUserService.UserId;
        if (userId is null)
            return Result<CreateClinicScreeningSessionResponse>.Unauthorized("User not authenticated");

        if (request.PatientVisitId == Guid.Empty)
            return Result<CreateClinicScreeningSessionResponse>.Failure("Patient visit is required.");

        var patient = await _patientRepository.GetByIdAsync(request.PatientId, cancellationToken);
        if (patient is null)
            return Result<CreateClinicScreeningSessionResponse>.NotFound("Patient not found");

        var visit = await _patientVisitRepository.GetByIdAsync(request.PatientVisitId, cancellationToken);
        if (visit is null)
            return Result<CreateClinicScreeningSessionResponse>.NotFound("Patient visit not found");

        if (visit.PatientId != patient.Id)
            return Result<CreateClinicScreeningSessionResponse>.Failure("Visit does not belong to this patient.");

        if (visit.Status == PatientVisitStatus.Completed)
            return Result<CreateClinicScreeningSessionResponse>.Failure("Cannot create screening for a completed visit.");

        var screening = new AiScreening(patient.Id, request.ModelVersion);
        screening.AttachToPatientVisit(visit.Id);

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

        _memoryCache.Remove(ClinicQueueCacheKey);

        _logger.LogInformation(
            "Clinic staff {UserId} created screening {ScreeningId} for patient {PatientId}, visit {VisitId}, images {Count}",
            userId.Value,
            screening.Id,
            patient.Id,
            visit.Id,
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
