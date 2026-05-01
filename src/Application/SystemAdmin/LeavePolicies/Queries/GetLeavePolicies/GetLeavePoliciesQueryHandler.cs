using Application.Common.Interfaces;
using Application.Common.Models;
using Application.SystemAdmin.LeavePolicies.Common;
using Domain.Repositories;

namespace Application.SystemAdmin.LeavePolicies.Queries.GetLeavePolicies;

public class GetLeavePoliciesQueryHandler : IQueryHandler<GetLeavePoliciesQuery, GetLeavePoliciesResult>
{
    private readonly ILeavePolicyRepository _leavePolicyRepository;

    public GetLeavePoliciesQueryHandler(ILeavePolicyRepository leavePolicyRepository)
    {
        _leavePolicyRepository = leavePolicyRepository;
    }

    public async Task<Result<GetLeavePoliciesResult>> Handle(
        GetLeavePoliciesQuery request,
        CancellationToken cancellationToken)
    {
        var (items, totalCount) = await _leavePolicyRepository.GetPagedAsync(
            request.PageNumber,
            request.PageSize,
            cancellationToken);

        var dtos = items.Select(p => new LeavePolicyDto
        {
            Id = p.Id,
            Name = p.Name,
            AdditionalDays = p.AdditionalDays,
            Description = p.Description,
            CreatedAt = p.CreatedAt,
            UpdatedAt = p.UpdatedAt
        }).ToList();

        return Result<GetLeavePoliciesResult>.Success(new GetLeavePoliciesResult
        {
            Items = dtos,
            TotalCount = totalCount,
            PageNumber = request.PageNumber,
            PageSize = request.PageSize
        });
    }
}
