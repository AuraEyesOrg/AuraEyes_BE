using Application.Common.Interfaces;
using Application.Common.Models;
using Domain.Enums;

namespace Application.SystemAdmin.Ophthalmologists.Queries.GetLeaveRequests;

public record GetLeaveRequestsQuery : IQuery<PagedResult<AdminOphthalmologistLeaveRequestDto>>
{
    public OphthalmologistLeaveRequestStatus? Status { get; init; }
    public Guid? OphthalmologistId { get; init; }
    public int PageNumber { get; init; } = 1;
    public int PageSize { get; init; } = 20;
}
