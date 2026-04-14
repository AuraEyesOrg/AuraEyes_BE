using Application.Common.Interfaces;
using Application.Common.Models;
using Application.Ophthalmologists.Common;
using Domain.Repositories;

namespace Application.Ophthalmologists.LeaveRequests.Queries.GetMyLeaveRequests;

public class GetMyLeaveRequestsQueryHandler : IQueryHandler<GetMyLeaveRequestsQuery, PagedResult<OphthalmologistLeaveRequestDto>>
{
    private readonly IOphthalmologistLeaveRequestRepository _leaveRequestRepository;

    public GetMyLeaveRequestsQueryHandler(IOphthalmologistLeaveRequestRepository leaveRequestRepository)
    {
        _leaveRequestRepository = leaveRequestRepository;
    }

    public async Task<Result<PagedResult<OphthalmologistLeaveRequestDto>>> Handle(
        GetMyLeaveRequestsQuery request,
        CancellationToken cancellationToken)
    {
        var (items, totalCount) = await _leaveRequestRepository.GetByOphthalmologistPagedAsync(
            request.OphthalmologistId,
            request.Status,
            request.PageNumber,
            request.PageSize,
            cancellationToken);

        var mapped = items.Select(x => new OphthalmologistLeaveRequestDto
        {
            Id = x.Id,
            OphthalmologistId = x.OphthalmologistId,
            StartDate = x.StartDate,
            EndDate = x.EndDate,
            Reason = x.Reason,
            Status = x.Status,
            AdminNote = x.AdminNote,
            ReviewedByAdminUserId = x.ReviewedByAdminUserId,
            ReviewedAt = x.ReviewedAt,
            CreatedAt = x.CreatedAt
        }).ToList();

        var pagedResult = new PagedResult<OphthalmologistLeaveRequestDto>(
            mapped,
            totalCount,
            request.PageNumber,
            request.PageSize);

        return Result<PagedResult<OphthalmologistLeaveRequestDto>>.Success(pagedResult);
    }
}
