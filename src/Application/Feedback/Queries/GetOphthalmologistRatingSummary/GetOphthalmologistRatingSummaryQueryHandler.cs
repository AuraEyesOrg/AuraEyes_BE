using Application.Common.Interfaces;
using Application.Common.Models;
using Application.Feedback.Common;
using Domain.Common;
using Domain.Entities.Users;
using Domain.Repositories;

namespace Application.Feedback.Queries.GetOphthalmologistRatingSummary;

public class GetOphthalmologistRatingSummaryQueryHandler : IQueryHandler<GetOphthalmologistRatingSummaryQuery, FeedbackRatingSummaryDto>
{
    private readonly IRepository<Ophthalmologist> _ophthalmologistRepository;
    private readonly IOphthalmologistFeedbackRepository _ophthalmologistFeedbackRepository;

    public GetOphthalmologistRatingSummaryQueryHandler(
        IRepository<Ophthalmologist> ophthalmologistRepository,
        IOphthalmologistFeedbackRepository ophthalmologistFeedbackRepository)
    {
        _ophthalmologistRepository = ophthalmologistRepository;
        _ophthalmologistFeedbackRepository = ophthalmologistFeedbackRepository;
    }

    public async Task<Result<FeedbackRatingSummaryDto>> Handle(GetOphthalmologistRatingSummaryQuery request, CancellationToken cancellationToken)
    {
        var exists = await _ophthalmologistRepository.ExistsAsync(x => x.Id == request.OphthalmologistId, cancellationToken);
        if (!exists)
            return Result<FeedbackRatingSummaryDto>.NotFound($"Ophthalmologist '{request.OphthalmologistId}' not found.");

        var (ratingAverage, ratingCount, distribution) = await _ophthalmologistFeedbackRepository.GetRatingSummaryAsync(
            request.OphthalmologistId,
            cancellationToken);

        return Result<FeedbackRatingSummaryDto>.Success(new FeedbackRatingSummaryDto
        {
            EntityId = request.OphthalmologistId,
            RatingAverage = ratingAverage,
            RatingCount = ratingCount,
            Distribution = distribution
        });
    }
}
