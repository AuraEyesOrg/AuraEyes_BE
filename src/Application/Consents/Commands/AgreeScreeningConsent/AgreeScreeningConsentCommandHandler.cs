using Application.Common.Interfaces;
using Application.Common.Models;
using Domain.Common;
using Domain.Entities.Screening;
using Domain.Entities.Users;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Application.Consents.Commands.AgreeScreeningConsent;

public class AgreeScreeningConsentCommandHandler
    : ICommandHandler<AgreeScreeningConsentCommand, AgreeScreeningConsentResponse>
{
    private readonly IRepository<AiScreening> _screeningRepository;
    private readonly IRepository<Patient> _patientRepository;
    private readonly ICurrentUserService _currentUserService;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<AgreeScreeningConsentCommandHandler> _logger;

    public AgreeScreeningConsentCommandHandler(
        IRepository<AiScreening> screeningRepository,
        IRepository<Patient> patientRepository,
        ICurrentUserService currentUserService,
        IUnitOfWork unitOfWork,
        ILogger<AgreeScreeningConsentCommandHandler> logger)
    {
        _screeningRepository = screeningRepository;
        _patientRepository = patientRepository;
        _currentUserService = currentUserService;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<Result<AgreeScreeningConsentResponse>> Handle(
        AgreeScreeningConsentCommand request,
        CancellationToken cancellationToken)
    {
        var userId = _currentUserService.UserId;
        if (userId is null)
        {
            return Result<AgreeScreeningConsentResponse>.Unauthorized("User not authenticated");
        }

        var patient = (await _patientRepository.FindAsync(
            p => p.UserId == userId.Value,
            cancellationToken)).FirstOrDefault();

        if (patient is null)
        {
            return Result<AgreeScreeningConsentResponse>.NotFound("Patient profile not found");
        }

        var screening = await _screeningRepository
            .Query()
            .Include(s => s.Consent)
            .FirstOrDefaultAsync(s => s.Id == request.ScreeningId, cancellationToken);

        if (screening is null)
        {
            return Result<AgreeScreeningConsentResponse>.NotFound(
                $"AI Screening {request.ScreeningId} not found");
        }

        if (screening.PatientId != patient.Id)
        {
            return Result<AgreeScreeningConsentResponse>.Forbidden(
                "You can only agree consent for your own screening.");
        }

        try
        {
            screening.RecordConsent(patient.Id, request.Content);

            await _screeningRepository.UpdateAsync(screening, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            var consent = screening.Consent!;
            _logger.LogInformation(
                "Consent agreed for screening {ScreeningId}, consent {ConsentId}, patient {PatientId}",
                screening.Id,
                consent.Id,
                patient.Id);

            return Result<AgreeScreeningConsentResponse>.Success(new AgreeScreeningConsentResponse
            {
                ConsentId = consent.Id,
                AiScreeningId = consent.AiScreeningId,
                PatientId = consent.PatientId,
                Content = consent.Content,
                IsAgreed = consent.IsAgreed,
                SignedAt = consent.SignedAt
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "Failed to agree consent for screening {ScreeningId}, patient {PatientId}",
                request.ScreeningId,
                patient.Id);
            return Result<AgreeScreeningConsentResponse>.Failure(
                $"Failed to save consent: {ex.Message}");
        }
    }
}
