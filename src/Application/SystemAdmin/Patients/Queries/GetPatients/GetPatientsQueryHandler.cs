using Application.Common.Interfaces;
using Application.Common.Models;

namespace Application.SystemAdmin.Patients.Queries.GetPatients;

/// <summary>
/// Handler for GetPatientsQuery - delegates to IAdminQueryService
/// for cross-layer joins between Domain entities and Identity data.
/// </summary>
public class GetPatientsQueryHandler
    : IQueryHandler<GetPatientsQuery, PagedResult<PatientListDto>>
{
    private readonly IAdminQueryService _queryService;

    public GetPatientsQueryHandler(IAdminQueryService queryService)
    {
        _queryService = queryService;
    }

    public async Task<Result<PagedResult<PatientListDto>>> Handle(
        GetPatientsQuery request,
        CancellationToken cancellationToken)
    {
        var result = await _queryService.GetPatientsAsync(
            request.SearchTerm,
            request.Status,
            request.PageNumber,
            request.PageSize,
            cancellationToken);

        return Result<PagedResult<PatientListDto>>.Success(result);
    }
}
