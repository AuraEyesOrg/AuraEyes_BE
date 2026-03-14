using Domain.Enums.Network;

namespace Application.Network.Trending.Queries.GetTrendingTopics;

/// <summary>
/// Trending topic DTO
/// </summary>
public class TrendingTopicDto
{
    public PostCategory Category { get; set; }
    public string TopicName { get; set; } = string.Empty;
    public int PostCount { get; set; }
    public string TimeFrame { get; set; } = string.Empty;
}
