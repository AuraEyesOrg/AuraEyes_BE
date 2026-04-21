using Application.Common.Interfaces;
using Application.Common.Models;
using Domain.Common;
using Domain.Entities.Consultation;
using Domain.Entities.Screening;
using Domain.Entities.Users;
using Microsoft.EntityFrameworkCore;

namespace Application.Guest.Queries.GetOverviewMetrics;

public class GetOverviewMetricsQueryHandler : IQueryHandler<GetOverviewMetricsQuery, GuestOverviewMetricsDto>
{
    private readonly IRepository<Ophthalmologist> _ophthalmologistRepository;
    private readonly IRepository<Organisation> _organisationRepository;
    private readonly IRepository<AiScreening> _aiScreeningRepository;
    private readonly IRepository<WebsiteFeedback> _websiteFeedbackRepository;
    private readonly IRepository<OrganisationFeedback> _organisationFeedbackRepository;
    private readonly IRepository<OphthalmologistFeedback> _ophthalmologistFeedbackRepository;

    public GetOverviewMetricsQueryHandler(
        IRepository<Ophthalmologist> ophthalmologistRepository,
        IRepository<Organisation> organisationRepository,
        IRepository<AiScreening> aiScreeningRepository,
        IRepository<WebsiteFeedback> websiteFeedbackRepository,
        IRepository<OrganisationFeedback> organisationFeedbackRepository,
        IRepository<OphthalmologistFeedback> ophthalmologistFeedbackRepository)
    {
        _ophthalmologistRepository = ophthalmologistRepository;
        _organisationRepository = organisationRepository;
        _aiScreeningRepository = aiScreeningRepository;
        _websiteFeedbackRepository = websiteFeedbackRepository;
        _organisationFeedbackRepository = organisationFeedbackRepository;
        _ophthalmologistFeedbackRepository = ophthalmologistFeedbackRepository;
    }

    public async Task<Result<GuestOverviewMetricsDto>> Handle(GetOverviewMetricsQuery request, CancellationToken cancellationToken)
    {
        // Run sequentially to avoid parallel operations on the same scoped DbContext instance.
        var ophthalmologistCount = await _ophthalmologistRepository.CountAsync(
            x => x.IsVerified == true,
            cancellationToken);

        var organisationCount = await _organisationRepository.CountAsync(cancellationToken: cancellationToken);
        var screeningCount = await _aiScreeningRepository.CountAsync(cancellationToken: cancellationToken);

        var websiteFeedbackMetrics = await _websiteFeedbackRepository.Query()
            .GroupBy(_ => 1)
            .Select(g => new
            {
                Count = g.Count(),
                RatingSum = g.Sum(x => (int?)x.Rating) ?? 0
            })
            .SingleOrDefaultAsync(cancellationToken);
        var organisationFeedbackMetrics = await _organisationFeedbackRepository.Query()
            .GroupBy(_ => 1)
            .Select(g => new
            {
                Count = g.Count(),
                RatingSum = g.Sum(x => (int?)x.Rating) ?? 0
            })
            .SingleOrDefaultAsync(cancellationToken);
        var ophthalmologistFeedbackMetrics = await _ophthalmologistFeedbackRepository.Query()
            .GroupBy(_ => 1)
            .Select(g => new
            {
                Count = g.Count(),
                RatingSum = g.Sum(x => (int?)x.Rating) ?? 0
            })
            .SingleOrDefaultAsync(cancellationToken);
        var totalFeedbackCount = (websiteFeedbackMetrics?.Count ?? 0)
            + (organisationFeedbackMetrics?.Count ?? 0)
            + (ophthalmologistFeedbackMetrics?.Count ?? 0);
        var totalFeedbackRating = (websiteFeedbackMetrics?.RatingSum ?? 0)
            + (organisationFeedbackMetrics?.RatingSum ?? 0)
            + (ophthalmologistFeedbackMetrics?.RatingSum ?? 0);

        double? averageRating = totalFeedbackCount > 0
            ? Math.Round((double)totalFeedbackRating / totalFeedbackCount, 1)
            : null;

        return Result<GuestOverviewMetricsDto>.Success(new GuestOverviewMetricsDto
        {
            OphthalmologistCount = ophthalmologistCount,
            OrganisationCount = organisationCount,
            ScreeningCount = screeningCount,
            AverageRating = averageRating
        });
    }
}

public class GuestOverviewMetricsDto
{
    public int OphthalmologistCount { get; init; }
    public int OrganisationCount { get; init; }
    public int ScreeningCount { get; init; }
    public double? AverageRating { get; init; }
}
