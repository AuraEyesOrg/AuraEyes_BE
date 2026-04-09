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
        var report = await _orgPatientsRepo.GetScreeningReportForOrganisationAdminAsync(
            request.OrgAdminUserId,
            cancellationToken);

        return Result<OrgScreeningReportDto>.Success(new OrgScreeningReportDto
        {
            TotalScreenings = report.TotalScreenings,
            HighRiskCount = report.HighRiskCount,
            ModerateRiskCount = report.ModerateRiskCount,
            LowRiskCount = report.LowRiskCount,
            AverageConfidence = Math.Round(report.AverageConfidence, 1),
            MonthlyBreakdown = report.MonthlyBreakdown
                .Select(m => new OrgMonthlyScreeningCount
                {
                    Month = m.Month,
                    Count = m.Count,
                    HighRisk = m.HighRisk,
                    ModerateRisk = m.ModerateRisk,
                    LowRisk = m.LowRisk
                })
                .ToList()
        });
    }
}
