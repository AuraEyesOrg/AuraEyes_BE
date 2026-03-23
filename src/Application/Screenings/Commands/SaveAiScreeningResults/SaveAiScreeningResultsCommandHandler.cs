using Application.Common.Interfaces;
using Application.Common.Models;
using Domain.Common;
using Domain.Entities.Screening;
using Domain.Entities.Users;
using Domain.Enums;
using Microsoft.Extensions.Logging;

namespace Application.Screenings.Commands.SaveAiScreeningResults;

/// <summary>
/// Handler for SaveAiScreeningResultsCommand
/// Saves AI analysis results, screening details, and retinal images to database.
/// Called after AI processing completes and results are ready for review.
/// </summary>
public class SaveAiScreeningResultsCommandHandler : ICommandHandler<SaveAiScreeningResultsCommand, SaveAiScreeningResultsResponse>
{
    private readonly IRepository<AiScreening> _screeningRepository;
    private readonly IRepository<Patient> _patientRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IAiScreeningQuery _screeningQuery;
    private readonly ILogger<SaveAiScreeningResultsCommandHandler> _logger;

    public SaveAiScreeningResultsCommandHandler(
        IRepository<AiScreening> screeningRepository,
        IRepository<Patient> patientRepository,
        IUnitOfWork unitOfWork,
        IAiScreeningQuery screeningQuery,
        ILogger<SaveAiScreeningResultsCommandHandler> logger)
    {
        _screeningRepository = screeningRepository;
        _patientRepository = patientRepository;
        _unitOfWork = unitOfWork;
        _screeningQuery = screeningQuery;
        _logger = logger;
    }

    public async Task<Result<SaveAiScreeningResultsResponse>> Handle(
        SaveAiScreeningResultsCommand request,
        CancellationToken cancellationToken)
    {
        // Validate confidence score
        if (request.ConfidenceScore < 0 || request.ConfidenceScore > 100)
        {
            _logger.LogWarning("Invalid confidence score for screening {ScreeningId}: {Score}",
                request.ScreeningId, request.ConfidenceScore);
            return Result<SaveAiScreeningResultsResponse>.Failure("Confidence score must be between 0 and 100");
        }

        // Get the screening session
        var screening = await _screeningRepository.GetByIdAsync(request.ScreeningId, cancellationToken);
        if (screening is null)
        {
            _logger.LogWarning("AI Screening {ScreeningId} not found", request.ScreeningId);
            return Result<SaveAiScreeningResultsResponse>.NotFound($"AI Screening {request.ScreeningId} not found");
        }

        // Validate patient exists
        var patient = await _patientRepository.GetByIdAsync(screening.PatientId, cancellationToken);
        if (patient is null)
        {
            _logger.LogWarning("Patient {PatientId} not found for screening {ScreeningId}",
                screening.PatientId, request.ScreeningId);
            return Result<SaveAiScreeningResultsResponse>.NotFound("Patient not found");
        }

        // Store raw JSON output in the screening
        screening.Process(request.RawJsonOutput);

        // Count images from DB — aggregate loaded via GetByIdAsync does not populate RetinalImages.
        var imagesCount = await _screeningQuery.CountRetinalImagesForScreeningAsync(
            request.ScreeningId,
            cancellationToken);

        // Create screening result entity
        var screeningResult = new ScreeningResult(
            aiScreeningId: request.ScreeningId,
            riskLevel: request.RiskLevel,
            confidenceScore: request.ConfidenceScore,
            summary: request.Summary,
            findings: request.Findings);

        // Add screening result to the screening aggregate
        screening.AddScreeningResult(screeningResult);

        // Save to database in transaction
        try
        {
            await _unitOfWork.BeginTransactionAsync(cancellationToken);

            // Save screening (aggregate root) which will cascade to children
            await _screeningRepository.UpdateAsync(screening, cancellationToken);

            await _unitOfWork.SaveChangesAsync(cancellationToken);
            await _unitOfWork.CommitTransactionAsync(cancellationToken);

            _logger.LogInformation(
                "AI Screening results saved for screening {ScreeningId}, " +
                "patient {PatientId}, risk level {RiskLevel}, confidence {ConfidenceScore}%",
                request.ScreeningId,
                screening.PatientId,
                request.RiskLevel,
                request.ConfidenceScore);

            return Result<SaveAiScreeningResultsResponse>.Success(new SaveAiScreeningResultsResponse
            {
                ScreeningId = request.ScreeningId,
                ScreeningResultId = screeningResult.Id,
                ImagesCount = imagesCount,
                SavedAt = DateTime.UtcNow,
                RiskLevel = request.RiskLevel.ToString()
            });
        }
        catch (Exception ex)
        {
            await _unitOfWork.RollbackTransactionAsync(cancellationToken);
            _logger.LogError(ex, "Error saving AI screening results for screening {ScreeningId}",
                request.ScreeningId);
            return Result<SaveAiScreeningResultsResponse>.Failure($"Failed to save screening results: {ex.Message}");
        }
    }
}
