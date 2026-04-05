using Application.Common.Interfaces;
using Application.Common.Models;
using Domain.Repositories;

namespace Application.OrganisationScreenings.Queries.GetOrgScreeningHistory;

public sealed class GetOrgScreeningHistoryQueryHandler
    : IQueryHandler<GetOrgScreeningHistoryQuery, IReadOnlyList<OrgScreeningHistoryItemDto>>
{
    private readonly IOrganisationPatientsRepository _orgPatientsRepo;

    public GetOrgScreeningHistoryQueryHandler(
        IOrganisationPatientsRepository orgPatientsRepo)
    {
        _orgPatientsRepo = orgPatientsRepo;
    }

    public async Task<Result<IReadOnlyList<OrgScreeningHistoryItemDto>>> Handle(
        GetOrgScreeningHistoryQuery request,
        CancellationToken cancellationToken)
    {
        var history = await _orgPatientsRepo.GetScreeningHistoryForOrganisationAdminAsync(
            request.OrgAdminUserId,
            request.Take,
            cancellationToken);

        var items = history.Select(item => new OrgScreeningHistoryItemDto
        {
            ScreeningId = item.ScreeningId,
            PatientId = item.PatientId,
            PatientName = item.PatientName,
            CreatedAt = item.CreatedAt,
            ProcessedAt = item.ProcessedAt,
            ImagesCount = item.ImagesCount,
            LatestRiskLevel = item.LatestRiskLevel,
            ConfidenceScore = item.ConfidenceScore,
            AiPrimaryLabel = item.AiPrimaryLabel,
            Status = item.Status
        }).ToList();

        return Result<IReadOnlyList<OrgScreeningHistoryItemDto>>.Success(items);
    }
}
