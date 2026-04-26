using Application.Common.Interfaces;
using Application.Common.Models;
using Application.Feedback.Common;
using Domain.Common;
using Domain.Entities.Users;
using Domain.Repositories;

namespace Application.Feedback.Queries.GetClinicRatingSummary;

public class GetClinicRatingSummaryQueryHandler : IQueryHandler<GetClinicRatingSummaryQuery, FeedbackRatingSummaryDto>
{
    private readonly IClinicFeedbackRepository _clinicFeedbackRepository;

    public GetClinicRatingSummaryQueryHandler(
        IClinicFeedbackRepository clinicFeedbackRepository)
    {
        _clinicFeedbackRepository = clinicFeedbackRepository;
    }

    public async Task<Result<FeedbackRatingSummaryDto>> Handle(GetClinicRatingSummaryQuery request, CancellationToken cancellationToken)
    {
        var (ratingAverage, ratingCount, distribution) = await _clinicFeedbackRepository.GetRatingSummaryAsync(
            cancellationToken);

        return Result<FeedbackRatingSummaryDto>.Success(new FeedbackRatingSummaryDto
        {
            EntityId = Guid.Empty, // No specific organisation ID needed anymore
            RatingAverage = ratingAverage,
            RatingCount = ratingCount,
            Distribution = distribution
        });
    }
}
