using Application.Common.Interfaces;
using Application.Common.Models;
using Domain.Repositories;

namespace Application.SystemAdmin.Users.Queries.GetUsers;

/// <summary>
/// Handler for GetUsersQuery - delegates to IIdentityService for user data with roles.
/// </summary>
public class GetUsersQueryHandler : IQueryHandler<GetUsersQuery, PagedResult<UserListDto>>
{
    private readonly IIdentityService _identityService;
    private readonly IOphthalmologistRepository _ophthalmologistRepository;

    public GetUsersQueryHandler(IIdentityService identityService, IOphthalmologistRepository ophthalmologistRepository)
    {
        _identityService = identityService;
        _ophthalmologistRepository = ophthalmologistRepository;
    }

    public async Task<Result<PagedResult<UserListDto>>> Handle(
        GetUsersQuery request,
        CancellationToken cancellationToken)
    {
        var (users, totalCount) = await _identityService.GetUsersAsync(
            request.SearchTerm,
            request.RoleFilter,
            request.StatusFilter,
            request.PageNumber,
            request.PageSize,
            cancellationToken);

        var ophthalmologists = await _ophthalmologistRepository.GetAllAsync(cancellationToken);
        var ophthalmologistLookup = ophthalmologists.ToDictionary(o => o.UserId, o => o);

        var items = users.Select(u => {
            var ophthalmologist = ophthalmologistLookup.TryGetValue(u.Id, out var o) ? o : null;
            return new UserListDto
            {
                Id = u.Id,
                Email = u.Email,
                FullName = u.FullName,
                PhoneNumber = u.PhoneNumber,
                Roles = u.Roles,
                Status = u.Status,
                IsActive = u.IsActive,
                EmailConfirmed = u.EmailConfirmed,
                CreatedAt = u.CreatedAt,
                LastLoginAt = u.LastLoginAt,
                ConsultationFee = ophthalmologist?.ConsultationFee,
                OphthalmologistId = ophthalmologist?.Id
            };
        }).ToList();

        var pagedResult = new PagedResult<UserListDto>(
            items, totalCount, request.PageNumber, request.PageSize);

        return Result<PagedResult<UserListDto>>.Success(pagedResult);
    }
}
