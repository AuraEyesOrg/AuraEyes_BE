using Application.Common.Interfaces;

namespace Application.SystemAdmin.Dashboard.Queries.GetLiveQueue;

public record GetLiveQueueQuery : IQuery<IReadOnlyList<LiveQueueItemDto>>
{
}
