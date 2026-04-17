using Application.Common.Interfaces;
using Application.Common.Models;
using Application.Ophthalmologists.Common;
using Domain.Enums;

namespace Application.Ophthalmologists.EmploymentTypeChangeRequests.Queries.GetMyEmploymentTypeChangeRequests;

public record GetMyEmploymentTypeChangeRequestsQuery : IQuery<PagedResult<OphthalmologistEmploymentTypeChangeRequestDto>>
{
    public Guid OphthalmologistId { get; init; }
    public OphthalmologistEmploymentTypeChangeRequestStatus? Status { get; init; }
    public int PageNumber { get; init; } = 1;
    public int PageSize { get; init; } = 20;
}
