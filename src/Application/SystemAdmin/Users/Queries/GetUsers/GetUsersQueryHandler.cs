using Application.Common.Interfaces;
using Application.Common.Models;

namespace Application.SystemAdmin.Users.Queries.GetUsers;

/// <summary>
/// Handler for GetUsersQuery - Returns mock data
/// </summary>
public class GetUsersQueryHandler : IQueryHandler<GetUsersQuery, PagedResult<UserListDto>>
{
    public Task<Result<PagedResult<UserListDto>>> Handle(GetUsersQuery request, CancellationToken cancellationToken)
    {
        // Mock users data
        var items = new List<UserListDto>
        {
            new() { Id = Guid.NewGuid(), Email = "admin@aura.com", FullName = "System Admin", Roles = new List<string> { "SystemAdmin" }, Status = "Active", IsActive = true, EmailConfirmed = true, CreatedAt = DateTime.UtcNow.AddYears(-1), LastLoginAt = DateTime.UtcNow.AddHours(-1) },
            new() { Id = Guid.NewGuid(), Email = "doctor1@aura.com", FullName = "Dr. Nguyen Van A", PhoneNumber = "0901234567", Roles = new List<string> { "Doctor" }, Status = "Active", IsActive = true, EmailConfirmed = true, CreatedAt = DateTime.UtcNow.AddMonths(-6), LastLoginAt = DateTime.UtcNow.AddDays(-1) },
            new() { Id = Guid.NewGuid(), Email = "doctor2@aura.com", FullName = "Dr. Tran Thi B", Roles = new List<string> { "Doctor", "Ophthalmologist" }, Status = "Active", IsActive = true, EmailConfirmed = true, CreatedAt = DateTime.UtcNow.AddMonths(-4), LastLoginAt = DateTime.UtcNow },
            new() { Id = Guid.NewGuid(), Email = "staff1@aura.com", FullName = "Le Van C", Roles = new List<string> { "Staff" }, Status = "Active", IsActive = true, EmailConfirmed = true, CreatedAt = DateTime.UtcNow.AddMonths(-3) },
            new() { Id = Guid.NewGuid(), Email = "patient1@aura.com", FullName = "Pham Thi D", Roles = new List<string> { "Patient" }, Status = "Pending", IsActive = false, EmailConfirmed = false, CreatedAt = DateTime.UtcNow.AddDays(-7) }
        };

        // Apply search filter
        if (!string.IsNullOrWhiteSpace(request.SearchTerm))
        {
            items = items.Where(x =>
                x.FullName.Contains(request.SearchTerm, StringComparison.OrdinalIgnoreCase) ||
                x.Email.Contains(request.SearchTerm, StringComparison.OrdinalIgnoreCase)
            ).ToList();
        }

        // Apply role filter
        if (!string.IsNullOrWhiteSpace(request.RoleFilter))
        {
            items = items.Where(x => x.Roles.Contains(request.RoleFilter)).ToList();
        }

        // Apply status filter
        if (!string.IsNullOrWhiteSpace(request.StatusFilter))
        {
            items = items.Where(x => x.Status.Equals(request.StatusFilter, StringComparison.OrdinalIgnoreCase)).ToList();
        }

        var pagedResult = new PagedResult<UserListDto>(items, 350, request.PageNumber, request.PageSize);
        return Task.FromResult(Result<PagedResult<UserListDto>>.Success(pagedResult));
    }
}
