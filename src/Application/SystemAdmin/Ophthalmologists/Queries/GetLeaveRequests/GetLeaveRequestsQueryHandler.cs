using Application.Common.Interfaces;
using Application.Common.Models;
using Domain.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Application.SystemAdmin.Ophthalmologists.Queries.GetLeaveRequests;

public class GetLeaveRequestsQueryHandler : IQueryHandler<GetLeaveRequestsQuery, PagedResult<AdminOphthalmologistLeaveRequestDto>>
{
    private readonly IOphthalmologistLeaveRequestRepository _leaveRequestRepository;
    private readonly IOphthalmologistRepository _ophthalmologistRepository;
    private readonly IIdentityService _identityService;

    public GetLeaveRequestsQueryHandler(
        IOphthalmologistLeaveRequestRepository leaveRequestRepository,
        IOphthalmologistRepository ophthalmologistRepository,
        IIdentityService identityService)
    {
        _leaveRequestRepository = leaveRequestRepository;
        _ophthalmologistRepository = ophthalmologistRepository;
        _identityService = identityService;
    }

    public async Task<Result<PagedResult<AdminOphthalmologistLeaveRequestDto>>> Handle(
        GetLeaveRequestsQuery request,
        CancellationToken cancellationToken)
    {
        var (items, totalCount) = await _leaveRequestRepository.GetPagedAsync(
            request.Status,
            request.OphthalmologistId,
            request.PageNumber,
            request.PageSize,
            cancellationToken);

        var ophthalmologistIds = items
            .Select(x => x.OphthalmologistId)
            .Distinct()
            .ToList();

        var ophthalmologists = await _ophthalmologistRepository.Query().AsNoTracking()
            .Where(x => ophthalmologistIds.Contains(x.Id))
            .Select(x => new { x.Id, x.UserId })
            .ToListAsync(cancellationToken);

        var doctorMap = ophthalmologists.ToDictionary(x => x.Id, x => x.UserId);

        var doctorNames = new Dictionary<Guid, (string FullName, string Email)>();
        foreach (var userId in doctorMap.Values.Distinct())
        {
            var user = await _identityService.GetUserByIdAsync(userId, cancellationToken);
            doctorNames[userId] = (
                user?.FullName ?? "Unknown",
                user?.Email ?? string.Empty);
        }

        var mapped = items.Select(item =>
        {
            doctorMap.TryGetValue(item.OphthalmologistId, out var doctorUserId);
            doctorNames.TryGetValue(doctorUserId, out var doctorIdentity);

            return new AdminOphthalmologistLeaveRequestDto
            {
                Id = item.Id,
                OphthalmologistId = item.OphthalmologistId,
                DoctorUserId = doctorUserId,
                DoctorFullName = doctorIdentity.FullName,
                DoctorEmail = doctorIdentity.Email,
                StartDate = item.StartDate,
                EndDate = item.EndDate,
                Reason = item.Reason,
                Status = item.Status,
                AdminNote = item.AdminNote,
                ReviewedByAdminUserId = item.ReviewedByAdminUserId,
                ReviewedAt = item.ReviewedAt,
                CreatedAt = item.CreatedAt
            };
        }).ToList();

        var pagedResult = new PagedResult<AdminOphthalmologistLeaveRequestDto>(
            mapped,
            totalCount,
            request.PageNumber,
            request.PageSize);

        return Result<PagedResult<AdminOphthalmologistLeaveRequestDto>>.Success(pagedResult);
    }
}

