using Application.Common.Interfaces;
using Application.Common.Models;
using Domain.Enums.Network;
using Domain.Repositories;

namespace Application.Network.Trending.Queries.GetTrendingTopics;

/// <summary>
/// Handler for GetTrendingTopicsQuery
/// </summary>
public class GetTrendingTopicsQueryHandler : IQueryHandler<GetTrendingTopicsQuery, List<TrendingTopicDto>>
{
    private readonly IPostRepository _postRepository;

    public GetTrendingTopicsQueryHandler(IPostRepository postRepository)
    {
        _postRepository = postRepository;
    }

    public async Task<Result<List<TrendingTopicDto>>> Handle(
        GetTrendingTopicsQuery request,
        CancellationToken cancellationToken)
    {
        var since = DateTime.UtcNow.AddDays(-7);
        var categoryCounts = await _postRepository.GetPostCountsByCategoryAsync(
            since, cancellationToken);

        var categoryNames = new Dictionary<PostCategory, string>
        {
            { PostCategory.CasePresentation, "Case Presentations" },
            { PostCategory.PeerDiscussion, "Peer Discussions" },
            { PostCategory.KnowledgeShare, "Knowledge Sharing" },
            { PostCategory.Announcement, "Announcements" }
        };

        var topics = categoryCounts
            .OrderByDescending(c => c.Value)
            .Take(request.Count)
            .Select(c => new TrendingTopicDto
            {
                Category = c.Key,
                TopicName = categoryNames.GetValueOrDefault(c.Key, c.Key.ToString()),
                PostCount = c.Value,
                TimeFrame = "This week"
            })
            .ToList();

        return Result<List<TrendingTopicDto>>.Success(topics);
    }
}
