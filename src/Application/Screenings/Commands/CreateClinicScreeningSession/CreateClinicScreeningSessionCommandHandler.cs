using Application.ClinicQueue.Common;
using Application.Common.Interfaces;
using Application.Common.Models;
using Domain.Common;
using Domain.Entities.Screening;
using Domain.Entities.Users;
using Domain.Enums;
using Domain.Repositories;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;

namespace Application.Screenings.Commands.CreateClinicScreeningSession;

public class CreateClinicScreeningSessionCommandHandler
    : IRequestHandler<CreateClinicScreeningSessionCommand, Result<Guid>>
{
    private const string ClinicQueueCacheKey = "clinic_queue_all";

    private readonly IRepository<AiScreening> _screeningRepository;
    private readonly IRepository<Patient> _patientRepository;
    private readonly IPatientVisitRepository _patientVisitRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMemoryCache _memoryCache;
    private readonly ILogger<CreateClinicScreeningSessionCommandHandler> _logger;

    public CreateClinicScreeningSessionCommandHandler(
        IRepository<AiScreening> screeningRepository,
        IRepository<Patient> patientRepository,
        IPatientVisitRepository patientVisitRepository,
        IUnitOfWork unitOfWork,
        IMemoryCache memoryCache,
        ILogger<CreateClinicScreeningSessionCommandHandler> logger)
    {
        _screeningRepository = screeningRepository;
        _patientRepository = patientRepository;
        _patientVisitRepository = patientVisitRepository;
        _unitOfWork = unitOfWork;
        _memoryCache = memoryCache;
        _logger = logger;
    }

    public async Task<Result<Guid>> Handle(
        CreateClinicScreeningSessionCommand request,
        CancellationToken cancellationToken)
    {
        if (request.PatientVisitId == Guid.Empty)
            return Result<Guid>.Failure("Patient visit is required.");

        var patient = await _patientRepository.GetByIdAsync(request.PatientId, cancellationToken);
        if (patient == null)
            return Result<Guid>.NotFound("Patient not found");

        var visit = await _patientVisitRepository
            .Query()
            .Include(v => v.MedicalRecord)
            .FirstOrDefaultAsync(v => v.Id == request.PatientVisitId, cancellationToken);
        if (visit == null)
            return Result<Guid>.NotFound("Patient visit not found");

        if (visit.PatientId != patient.Id)
            return Result<Guid>.Failure("Visit does not belong to this patient.");

        if (visit.Status == PatientVisitStatus.Completed)
            return Result<Guid>.Failure("Cannot create screening for a completed visit.");

        if (!ClinicAdministrativeErmGate.IsSatisfied(visit.MedicalRecord))
            return Result<Guid>.Failure("Administrative ERM must be saved before creating a screening session.");

        var screening = new AiScreening(patient.Id, request.ModelVersion);
        screening.AttachToPatientVisit(visit.Id);

        if (request.RetinalImages is { Count: > 0 })
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

            _memoryCache.Remove(ClinicQueueCacheKey);

            _logger.LogInformation(
                "Clinic staff created screening session {ScreeningId} for patient {PatientId}, visit {VisitId}",
                screening.Id,
                patient.Id,
                visit.Id);

            return Result<Guid>.Success(screening.Id);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating clinic screening session for patient {PatientId}", patient.Id);
            return Result<Guid>.Failure($"Failed to create screening session: {ex.Message}");
        }
    }
}
