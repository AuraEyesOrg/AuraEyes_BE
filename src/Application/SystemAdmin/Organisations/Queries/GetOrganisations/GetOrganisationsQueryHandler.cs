using Application.Common.Interfaces;
using Application.Common.Models;

namespace Application.SystemAdmin.Organisations.Queries.GetOrganisations;

/// <summary>
/// Handler for GetOrganisationsQuery - Returns mock data
/// </summary>
public class GetOrganisationsQueryHandler : IQueryHandler<GetOrganisationsQuery, PagedResult<OrganisationListDto>>
{
    public Task<Result<PagedResult<OrganisationListDto>>> Handle(GetOrganisationsQuery request, CancellationToken cancellationToken)
    {
        // Mock organisations (clinics/hospitals)
        var items = new List<OrganisationListDto>
        {
            new() { Id = Guid.NewGuid(), Name = "Central Hospital", Address = "123 Le Loi, District 1, HCMC", LicenseNumber = "MED-001", OrgType = "Hospital", DeviceCount = 25, IsActive = true, CreatedAt = DateTime.UtcNow.AddYears(-2) },
            new() { Id = Guid.NewGuid(), Name = "Eye Care Clinic", Address = "456 Nguyen Hue, District 1, HCMC", LicenseNumber = "CLI-002", OrgType = "Clinic", DeviceCount = 8, IsActive = true, CreatedAt = DateTime.UtcNow.AddYears(-1) },
            new() { Id = Guid.NewGuid(), Name = "Private Practice Dr. Nguyen", Address = "789 Tran Hung Dao, District 5, HCMC", LicenseNumber = "PRI-003", OrgType = "PrivatePractice", DeviceCount = 3, IsActive = true, CreatedAt = DateTime.UtcNow.AddMonths(-6) },
            new() { Id = Guid.NewGuid(), Name = "Diagnostic Center", Address = "321 Cach Mang Thang 8, District 3, HCMC", LicenseNumber = "DIA-004", OrgType = "DiagnosticCenter", DeviceCount = 15, IsActive = true, CreatedAt = DateTime.UtcNow.AddMonths(-8) },
            new() { Id = Guid.NewGuid(), Name = "Research Institute", Address = "555 Vo Van Tan, District 3, HCMC", LicenseNumber = "RES-005", OrgType = "ResearchCenter", DeviceCount = 12, IsActive = false, CreatedAt = DateTime.UtcNow.AddYears(-3) }
        };

        // Apply search filter
        if (!string.IsNullOrWhiteSpace(request.SearchTerm))
        {
            items = items.Where(x => 
                x.Name.Contains(request.SearchTerm, StringComparison.OrdinalIgnoreCase) ||
                (x.Address?.Contains(request.SearchTerm, StringComparison.OrdinalIgnoreCase) ?? false)
            ).ToList();
        }

        // Apply org type filter
        if (!string.IsNullOrWhiteSpace(request.OrgType))
        {
            items = items.Where(x => x.OrgType.Equals(request.OrgType, StringComparison.OrdinalIgnoreCase)).ToList();
        }

        var pagedResult = new PagedResult<OrganisationListDto>(items, 45, request.PageNumber, request.PageSize);
        return Task.FromResult(Result<PagedResult<OrganisationListDto>>.Success(pagedResult));
    }
}
