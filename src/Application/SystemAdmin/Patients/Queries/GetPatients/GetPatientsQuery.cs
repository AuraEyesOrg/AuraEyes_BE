using Application.Common.Interfaces;
using Application.Common.Models;

namespace Application.SystemAdmin.Patients.Queries.GetPatients;

/// <summary>
/// Query to get a paginated list of patients for admin management.
/// </summary>
public record GetPatientsQuery : IQuery<PagedResult<PatientListDto>>
{
    public string? SearchTerm { get; init; }
    public string? Status { get; init; }
    public int PageNumber { get; init; } = 1;
    public int PageSize { get; init; } = 10;
}
