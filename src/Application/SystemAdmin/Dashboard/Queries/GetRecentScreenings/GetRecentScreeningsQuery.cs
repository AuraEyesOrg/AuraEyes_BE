using Application.Common.Interfaces;
using Application.Common.Models;

namespace Application.SystemAdmin.Dashboard.Queries.GetRecentScreenings;

/// <summary>
/// Query to get recent screenings list
/// Screen: 3.3.8 View Recent Screenings List
/// </summary>
public record GetRecentScreeningsQuery : IQuery<PagedResult<RecentScreeningDto>>
{
    public int PageNumber { get; init; } = 1;
    public int PageSize { get; init; } = 10;
}
