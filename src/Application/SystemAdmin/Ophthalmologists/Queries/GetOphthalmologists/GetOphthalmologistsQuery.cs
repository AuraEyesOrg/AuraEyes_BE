using Application.Common.Interfaces;
using Application.Common.Models;

namespace Application.SystemAdmin.Ophthalmologists.Queries.GetOphthalmologists;

/// <summary>
/// Query to get a paginated list of ophthalmologists for admin management.
/// </summary>
public record GetOphthalmologistsQuery : IQuery<PagedResult<OphthalmologistListDto>>
{
    public string? SearchTerm { get; init; }
    public string? VerificationStatus { get; init; }
    public int PageNumber { get; init; } = 1;
    public int PageSize { get; init; } = 10;
}
