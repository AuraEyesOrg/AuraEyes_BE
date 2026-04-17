using Application.Common.Interfaces;
using Application.Common.Models;
using Application.Ophthalmologists.Common;
using Domain.Enums;

namespace Application.Ophthalmologists.LeaveRequests.Queries.GetMyLeaveRequests;

public record GetMyLeaveRequestsQuery : IQuery<PagedResult<OphthalmologistLeaveRequestDto>>
{
    public Guid OphthalmologistId { get; init; }
    public OphthalmologistLeaveRequestStatus? Status { get; init; }
    public int PageNumber { get; init; } = 1;
    public int PageSize { get; init; } = 20;
}
