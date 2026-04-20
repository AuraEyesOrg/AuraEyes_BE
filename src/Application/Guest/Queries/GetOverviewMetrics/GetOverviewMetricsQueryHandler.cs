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

        var websiteFeedbackCount = await _websiteFeedbackRepository.CountAsync(cancellationToken: cancellationToken);
        var websiteFeedbackRatingSum = await _websiteFeedbackRepository.Query()
            .Select(x => (int?)x.Rating)
            .SumAsync(cancellationToken);

        var organisationFeedbackCount = await _organisationFeedbackRepository.CountAsync(cancellationToken: cancellationToken);
        var organisationFeedbackRatingSum = await _organisationFeedbackRepository.Query()
            .Select(x => (int?)x.Rating)
            .SumAsync(cancellationToken);

        var ophthalmologistFeedbackCount = await _ophthalmologistFeedbackRepository.CountAsync(cancellationToken: cancellationToken);
        var ophthalmologistFeedbackRatingSum = await _ophthalmologistFeedbackRepository.Query()
            .Select(x => (int?)x.Rating)
            .SumAsync(cancellationToken);

        var totalFeedbackCount = websiteFeedbackCount
            + organisationFeedbackCount
            + ophthalmologistFeedbackCount;

        var totalFeedbackRating = (websiteFeedbackRatingSum ?? 0)
            + (organisationFeedbackRatingSum ?? 0)
            + (ophthalmologistFeedbackRatingSum ?? 0);

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
