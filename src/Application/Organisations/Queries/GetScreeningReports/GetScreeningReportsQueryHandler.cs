using Application.Common.Interfaces;
using Application.Common.Models;
using Application.OrganisationScreenings;

namespace Application.Organisations.Queries.GetScreeningReports;

public sealed class GetScreeningReportsQueryHandler
    : IQueryHandler<GetScreeningReportsQuery, OrgScreeningReportDto>
{
    private readonly Domain.Repositories.IOrganisationPatientsRepository _orgPatientsRepo;

    public GetScreeningReportsQueryHandler(
        Domain.Repositories.IOrganisationPatientsRepository orgPatientsRepo)
    {
        _orgPatientsRepo = orgPatientsRepo;
    }

    public async Task<Result<OrgScreeningReportDto>> Handle(
        GetScreeningReportsQuery request,
        CancellationToken cancellationToken)
    {
        var allPatients = await _orgPatientsRepo.GetRecentPatientsForOrganisationAdminAsync(
            request.OrgAdminUserId, 1000, cancellationToken);

        var highRisk = allPatients.Count(p => p.Priority == "high");
        var mediumRisk = allPatients.Count(p => p.Priority == "medium");
        var lowRisk = allPatients.Count(p => p.Priority == "low");
        var avgConfidence = allPatients.Count > 0
            ? allPatients.Average(p => p.Confidence)
            : 0m;

        // Monthly breakdown for last 6 months
        var monthly = allPatients
            .GroupBy(p => p.LastScreening.ToString("yyyy-MM"))
            .OrderByDescending(g => g.Key)
            .Take(6)
            .Select(g => new OrgMonthlyScreeningCount
            {
                Month = g.Key,
                Count = g.Count(),
                HighRisk = g.Count(p => p.Priority == "high"),
                ModerateRisk = g.Count(p => p.Priority == "medium"),
                LowRisk = g.Count(p => p.Priority == "low")
            })
            .OrderBy(m => m.Month)
            .ToList();

        return Result<OrgScreeningReportDto>.Success(new OrgScreeningReportDto
        {
            TotalScreenings = allPatients.Count,
            HighRiskCount = highRisk,
            ModerateRiskCount = mediumRisk,
            LowRiskCount = lowRisk,
            AverageConfidence = Math.Round(avgConfidence, 1),
            MonthlyBreakdown = monthly
        });
    }
}
