using Application.Common.Interfaces;
using Application.Common.Models;
using Application.Ophthalmologists.Common;

namespace Application.Ophthalmologists.Queries.GetOphthalmologists;

/// <summary>
/// Query to get paginated list of ophthalmologists with optional filters.
/// </summary>
public record GetOphthalmologistsQuery : IQuery<PagedResult<OphthalmologistListDto>>
{
    public string? SearchTerm { get; init; }
    public bool? IsVerified { get; init; }
    public int PageNumber { get; init; } = 1;
    public int PageSize { get; init; } = 10;
}
