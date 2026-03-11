using Application.Common.Interfaces;

namespace Application.Network.Trending.Queries.GetTrendingTopics;

/// <summary>
/// Query to get trending topics in the professional network
/// </summary>
public record GetTrendingTopicsQuery : IQuery<List<TrendingTopicDto>>
{
    public int Count { get; init; } = 5;
}
