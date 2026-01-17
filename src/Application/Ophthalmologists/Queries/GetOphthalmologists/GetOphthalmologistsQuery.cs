using Application.Common.Interfaces;
using Application.Common.Models;

namespace Application.Ophthalmologists.Queries.GetOphthalmologists;

public record GetOphthalmologistsQuery : IQuery<PagedResult<OphthalmologistListDto>>
{
    public int PageNumber { get; init; } = 1;
    public int PageSize { get; init; } = 10;
    public bool? IsVerified { get; init; }
    public int? MinYearsOfExperience { get; init; }
    public int? MaxYearsOfExperience { get; init; }
}
