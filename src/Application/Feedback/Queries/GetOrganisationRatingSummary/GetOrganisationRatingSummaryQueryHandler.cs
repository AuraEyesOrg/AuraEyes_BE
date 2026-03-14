using Application.Common.Interfaces;
using Application.Common.Models;
using Application.Feedback.Common;
using Domain.Common;
using Domain.Entities.Users;
using Domain.Repositories;

namespace Application.Feedback.Queries.GetOrganisationRatingSummary;

public class GetOrganisationRatingSummaryQueryHandler : IQueryHandler<GetOrganisationRatingSummaryQuery, FeedbackRatingSummaryDto>
{
    private readonly IRepository<Organisation> _organisationRepository;
    private readonly IOrganisationFeedbackRepository _organisationFeedbackRepository;

    public GetOrganisationRatingSummaryQueryHandler(
        IRepository<Organisation> organisationRepository,
        IOrganisationFeedbackRepository organisationFeedbackRepository)
    {
        _organisationRepository = organisationRepository;
        _organisationFeedbackRepository = organisationFeedbackRepository;
    }

    public async Task<Result<FeedbackRatingSummaryDto>> Handle(GetOrganisationRatingSummaryQuery request, CancellationToken cancellationToken)
    {
        var exists = await _organisationRepository.ExistsAsync(x => x.Id == request.OrganisationId, cancellationToken);
        if (!exists)
            return Result<FeedbackRatingSummaryDto>.NotFound($"Organisation '{request.OrganisationId}' not found.");

        var (ratingAverage, ratingCount, distribution) = await _organisationFeedbackRepository.GetRatingSummaryAsync(
            request.OrganisationId,
            cancellationToken);

        return Result<FeedbackRatingSummaryDto>.Success(new FeedbackRatingSummaryDto
        {
            EntityId = request.OrganisationId,
            RatingAverage = ratingAverage,
            RatingCount = ratingCount,
            Distribution = distribution
        });
    }
}
