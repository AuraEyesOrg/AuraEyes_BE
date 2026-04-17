using Application.Common.Interfaces;
using Application.Common.Models;
using Domain.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Application.SystemAdmin.Ophthalmologists.Queries.GetEmploymentTypeChangeRequests;

public class GetEmploymentTypeChangeRequestsQueryHandler : IQueryHandler<GetEmploymentTypeChangeRequestsQuery, PagedResult<AdminOphthalmologistEmploymentTypeChangeRequestDto>>
{
    private readonly IOphthalmologistEmploymentTypeChangeRequestRepository _requestRepository;
    private readonly IOphthalmologistRepository _ophthalmologistRepository;
    private readonly IIdentityService _identityService;

    public GetEmploymentTypeChangeRequestsQueryHandler(
        IOphthalmologistEmploymentTypeChangeRequestRepository requestRepository,
        IOphthalmologistRepository ophthalmologistRepository,
        IIdentityService identityService)
    {
        _requestRepository = requestRepository;
        _ophthalmologistRepository = ophthalmologistRepository;
        _identityService = identityService;
    }

    public async Task<Result<PagedResult<AdminOphthalmologistEmploymentTypeChangeRequestDto>>> Handle(
        GetEmploymentTypeChangeRequestsQuery request,
        CancellationToken cancellationToken)
    {
        var (items, totalCount) = await _requestRepository.GetPagedAsync(
            request.Status,
            request.OphthalmologistId,
            request.PageNumber,
            request.PageSize,
            cancellationToken);

        var ophthalmologistIds = items
            .Select(x => x.OphthalmologistId)
            .Distinct()
            .ToList();

        var ophthalmologists = await _ophthalmologistRepository.Query()
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

            return new AdminOphthalmologistEmploymentTypeChangeRequestDto
            {
                Id = item.Id,
                OphthalmologistId = item.OphthalmologistId,
                DoctorUserId = doctorUserId,
                DoctorFullName = doctorIdentity.FullName,
                DoctorEmail = doctorIdentity.Email,
                CurrentEmploymentType = item.CurrentEmploymentType,
                TargetEmploymentType = item.TargetEmploymentType,
                Reason = item.Reason,
                Status = item.Status,
                AdminNote = item.AdminNote,
                ReviewedByAdminUserId = item.ReviewedByAdminUserId,
                ReviewedAt = item.ReviewedAt,
                CreatedAt = item.CreatedAt
            };
        }).ToList();

        var pagedResult = new PagedResult<AdminOphthalmologistEmploymentTypeChangeRequestDto>(
            mapped,
            totalCount,
            request.PageNumber,
            request.PageSize);

        return Result<PagedResult<AdminOphthalmologistEmploymentTypeChangeRequestDto>>.Success(pagedResult);
    }
}
