using Application.Common.Interfaces;
using Application.Common.Models;
using Application.Ophthalmologists.Common;
using Domain.Repositories;

namespace Application.Ophthalmologists.EmploymentTypeChangeRequests.Queries.GetMyEmploymentTypeChangeRequests;

public class GetMyEmploymentTypeChangeRequestsQueryHandler : IQueryHandler<GetMyEmploymentTypeChangeRequestsQuery, PagedResult<OphthalmologistEmploymentTypeChangeRequestDto>>
{
    private readonly IOphthalmologistEmploymentTypeChangeRequestRepository _requestRepository;

    public GetMyEmploymentTypeChangeRequestsQueryHandler(IOphthalmologistEmploymentTypeChangeRequestRepository requestRepository)
    {
        _requestRepository = requestRepository;
    }

    public async Task<Result<PagedResult<OphthalmologistEmploymentTypeChangeRequestDto>>> Handle(
        GetMyEmploymentTypeChangeRequestsQuery request,
        CancellationToken cancellationToken)
    {
        var (items, totalCount) = await _requestRepository.GetByOphthalmologistPagedAsync(
            request.OphthalmologistId,
            request.Status,
            request.PageNumber,
            request.PageSize,
            cancellationToken);

        var mapped = items.Select(x => new OphthalmologistEmploymentTypeChangeRequestDto
        {
            Id = x.Id,
            OphthalmologistId = x.OphthalmologistId,
            CurrentEmploymentType = x.CurrentEmploymentType,
            TargetEmploymentType = x.TargetEmploymentType,
            Reason = x.Reason,
            Status = x.Status,
            AdminNote = x.AdminNote,
            ReviewedByAdminUserId = x.ReviewedByAdminUserId,
            ReviewedAt = x.ReviewedAt,
            CreatedAt = x.CreatedAt
        }).ToList();

        var pagedResult = new PagedResult<OphthalmologistEmploymentTypeChangeRequestDto>(
            mapped,
            totalCount,
            request.PageNumber,
            request.PageSize);

        return Result<PagedResult<OphthalmologistEmploymentTypeChangeRequestDto>>.Success(pagedResult);
    }
}
