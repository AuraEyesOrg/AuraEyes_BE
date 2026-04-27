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
    private readonly IRepository<AiScreening> _aiScreeningRepository;
    private readonly IRepository<ClinicFeedback> _clinicFeedbackRepository;

    public GetOverviewMetricsQueryHandler(
        IRepository<Ophthalmologist> ophthalmologistRepository,
        IRepository<AiScreening> aiScreeningRepository,
        IRepository<ClinicFeedback> clinicFeedbackRepository)
    {
        _ophthalmologistRepository = ophthalmologistRepository;
        _aiScreeningRepository = aiScreeningRepository;
        _clinicFeedbackRepository = clinicFeedbackRepository;
    }

    public async Task<Result<GuestOverviewMetricsDto>> Handle(GetOverviewMetricsQuery request, CancellationToken cancellationToken)
    {
        // Run sequentially to avoid parallel operations on the same scoped DbContext instance.
        var ophthalmologistCount = await _ophthalmologistRepository.CountAsync(
            x => x.IsVerified == true,
            cancellationToken);

        var screeningCount = await _aiScreeningRepository.CountAsync(cancellationToken: cancellationToken);

        var clinicFeedbackMetrics = await _clinicFeedbackRepository.Query()
            .GroupBy(_ => 1)
            .Select(g => new
            {
                Count = g.Count(),
                RatingSum = g.Sum(x => (int?)x.Rating) ?? 0
            })
            .SingleOrDefaultAsync(cancellationToken);
        
        var totalFeedbackCount = clinicFeedbackMetrics?.Count ?? 0;
        var totalFeedbackRating = clinicFeedbackMetrics?.RatingSum ?? 0;

        double? averageRating = totalFeedbackCount > 0
            ? Math.Round((double)totalFeedbackRating / totalFeedbackCount, 1)
            : null;

        return Result<GuestOverviewMetricsDto>.Success(new GuestOverviewMetricsDto
        {
            OphthalmologistCount = ophthalmologistCount,
            OrganisationCount = 0,
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
