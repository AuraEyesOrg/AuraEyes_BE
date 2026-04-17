using Application.Common.Interfaces;
using Application.Common.Models;
using Domain.Enums;

namespace Application.SystemAdmin.Ophthalmologists.Queries.GetEmploymentTypeChangeRequests;

public record GetEmploymentTypeChangeRequestsQuery : IQuery<PagedResult<AdminOphthalmologistEmploymentTypeChangeRequestDto>>
{
    public OphthalmologistEmploymentTypeChangeRequestStatus? Status { get; init; }
    public Guid? OphthalmologistId { get; init; }
    public int PageNumber { get; init; } = 1;
    public int PageSize { get; init; } = 20;
}
