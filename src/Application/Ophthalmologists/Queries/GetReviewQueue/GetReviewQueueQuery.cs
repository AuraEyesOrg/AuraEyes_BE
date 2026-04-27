using Application.Common.Interfaces;

namespace Application.Ophthalmologists.Queries.GetReviewQueue;

public record GetReviewQueueQuery(Guid UserId) : IQuery<List<ReviewQueueItemDto>>;
