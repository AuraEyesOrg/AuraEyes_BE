using Application.Common.Interfaces;
using Application.Common.Models;
using Domain.Repositories;
using MediatR;

namespace Application.OphthalmologistScreenings.Queries.ListOphthalmologistScreenings;

/// <summary>
/// Handles ListOphthalmologistScreeningsQuery by delegating to the read repository.
/// </summary>
public sealed class ListOphthalmologistScreeningsQueryHandler
    : IRequestHandler<ListOphthalmologistScreeningsQuery, Result<IReadOnlyList<OphthalmologistScreeningListItemDto>>>
{
    private readonly IOphthalmologistScreeningsReadRepository _repository;

    public ListOphthalmologistScreeningsQueryHandler(IOphthalmologistScreeningsReadRepository repository)
    {
        _repository = repository;
    }

    public async Task<Result<IReadOnlyList<OphthalmologistScreeningListItemDto>>> Handle(
        ListOphthalmologistScreeningsQuery request,
        CancellationToken cancellationToken)
    {
        // Delegate to repository (optimized SQL queries + consent-based filtering)
        var readModels = await _repository.ListForOphthalmologistAsync(
            request.OphthalmologistProfileId,
            cancellationToken);

        // Map read models to DTOs
        var dtos = readModels.Select(r => new OphthalmologistScreeningListItemDto
        {
            ScreeningId = r.ScreeningId,
            PatientId = r.PatientId,
            PatientName = r.PatientName,
            CreatedAt = r.CreatedAt,
            ProcessedAt = r.ProcessedAt,
            ModelVersion = r.ModelVersion,
            ImagesCount = r.ImagesCount,
            ThumbnailUrl = r.ThumbnailUrl,
            LatestRiskLevel = r.LatestRiskLevel,
            ConfidenceScore = r.ConfidenceScore,
            AiPrimaryLabel = r.AiPrimaryLabel,
            SummarySnippet = r.SummarySnippet,
            ReviewStatus = r.ReviewStatus
        }).ToList();

        return Result<IReadOnlyList<OphthalmologistScreeningListItemDto>>.Success(dtos);
    }
}
