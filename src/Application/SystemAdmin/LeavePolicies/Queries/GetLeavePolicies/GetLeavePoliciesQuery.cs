using Application.Common.Interfaces;
using Application.SystemAdmin.LeavePolicies.Common;

namespace Application.SystemAdmin.LeavePolicies.Queries.GetLeavePolicies;

public record GetLeavePoliciesQuery : IQuery<GetLeavePoliciesResult>
{
    public int PageNumber { get; init; } = 1;
    public int PageSize { get; init; } = 20;
}

public record GetLeavePoliciesResult
{
    public IReadOnlyList<LeavePolicyDto> Items { get; init; } = [];
    public int TotalCount { get; init; }
    public int PageNumber { get; init; }
    public int PageSize { get; init; }
    public int TotalPages => PageSize > 0 ? (int)Math.Ceiling((double)TotalCount / PageSize) : 0;
    public bool HasPrevious => PageNumber > 1;
    public bool HasNext => PageNumber < TotalPages;
}
