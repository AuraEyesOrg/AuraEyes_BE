using Application.Common.Interfaces;
using Application.Common.Models;
using Domain.Repositories;
using MediatR;

namespace Application.OphthalmologistScreenings.Queries.GetOphthalmologistScreeningDetail;

/// <summary>
/// Handles GetOphthalmologistScreeningDetailQuery by delegating to the read repository.
/// </summary>
public sealed class GetOphthalmologistScreeningDetailQueryHandler
    : IRequestHandler<GetOphthalmologistScreeningDetailQuery, Result<OphthalmologistScreeningDetailDto?>>
{
    private readonly IOphthalmologistScreeningsReadRepository _repository;

    public GetOphthalmologistScreeningDetailQueryHandler(IOphthalmologistScreeningsReadRepository repository)
    {
        _repository = repository;
    }

    public async Task<Result<OphthalmologistScreeningDetailDto?>> Handle(
        GetOphthalmologistScreeningDetailQuery request,
        CancellationToken cancellationToken)
    {
        // Delegate to repository (access control + internal doctor visibility rules)
        var readModel = await _repository.GetDetailForOphthalmologistAsync(
            request.OphthalmologistProfileId,
            request.ScreeningId,
            cancellationToken);

        // Access denied: no consultation session linking ophthalmologist to screening
        if (readModel is null)
            return Result<OphthalmologistScreeningDetailDto?>.NotFound(
                "Screening not found or you do not have access.");

        // Map read model to DTO
        var dto = new OphthalmologistScreeningDetailDto
        {
            ScreeningId = readModel.ScreeningId,
            PatientId = readModel.PatientId,
            PatientFullName = readModel.PatientFullName,
            ModelVersion = readModel.ModelVersion,
            CreatedAt = readModel.CreatedAt,
            ProcessedAt = readModel.ProcessedAt,
            RawJsonOutput = readModel.RawJsonOutput, // Null if not shared
            ReviewStatus = readModel.ReviewStatus,
            Images = readModel.Images.Select(i => new OphthalmologistRetinalImageDto
            {
                Id = i.Id,
                ImageUrl = i.ImageUrl,
                EyeSide = i.EyeSide,
                DeviceName = i.DeviceName,
                QualityScore = i.QualityScore,
                CapturedAt = i.CapturedAt
            }).ToList(),
            LatestResult = readModel.LatestResult is not null
                ? new OphthalmologistScreeningResultDto
                {
                    ScreeningResultId = readModel.LatestResult.ScreeningResultId,
                    RiskLevel = readModel.LatestResult.RiskLevel,
                    ConfidenceScore = readModel.LatestResult.ConfidenceScore,
                    Summary = readModel.LatestResult.Summary,
                    Findings = readModel.LatestResult.Findings,
                    AssessedAt = readModel.LatestResult.AssessedAt
                }
                : null,
            MedicalRecordId = readModel.MedicalRecordId
        };

        return Result<OphthalmologistScreeningDetailDto?>.Success(dto);
    }
}
