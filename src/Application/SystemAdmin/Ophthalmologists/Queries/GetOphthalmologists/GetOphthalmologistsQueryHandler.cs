using Application.Common.Interfaces;
using Application.Common.Models;
using Application.SystemAdmin.Interfaces;

namespace Application.SystemAdmin.Ophthalmologists.Queries.GetOphthalmologists;

/// <summary>
/// Handler for GetOphthalmologistsQuery - delegates to IAdminQueryService
/// for cross-layer joins between Domain entities and Identity data.
/// </summary>
public class GetOphthalmologistsQueryHandler
    : IQueryHandler<GetOphthalmologistsQuery, PagedResult<OphthalmologistListDto>>
{
    private readonly IAdminQueryService _queryService;

    public GetOphthalmologistsQueryHandler(IAdminQueryService queryService)
    {
        _queryService = queryService;
    }

    public async Task<Result<PagedResult<OphthalmologistListDto>>> Handle(
        GetOphthalmologistsQuery request,
        CancellationToken cancellationToken)
    {
        var result = await _queryService.GetOphthalmologistsAsync(
            request.SearchTerm,
            request.PageNumber,
            request.PageSize,
            cancellationToken);

        return Result<PagedResult<OphthalmologistListDto>>.Success(result);
    }
}
