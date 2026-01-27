using Application.Common.Interfaces;
using Application.Common.Models;

namespace Application.SystemAdmin.Dashboard.Queries.GetRecentScreenings;

/// <summary>
/// Handler for GetRecentScreeningsQuery - Returns mock data
/// </summary>
public class GetRecentScreeningsQueryHandler : IQueryHandler<GetRecentScreeningsQuery, PagedResult<RecentScreeningDto>>
{
    public Task<Result<PagedResult<RecentScreeningDto>>> Handle(GetRecentScreeningsQuery request, CancellationToken cancellationToken)
    {
        // Generate mock recent screenings
        var items = new List<RecentScreeningDto>
        {
            new() { Id = Guid.NewGuid(), ScreeningCode = "SCR-2024-001", PatientId = Guid.NewGuid(), PatientName = "Nguyen Van A", ClinicId = Guid.NewGuid(), ClinicName = "Clinic A", Status = "Completed", RiskLevel = "Low", IsCritical = false, CreatedAt = DateTime.UtcNow.AddHours(-2), CompletedAt = DateTime.UtcNow.AddHours(-1) },
            new() { Id = Guid.NewGuid(), ScreeningCode = "SCR-2024-002", PatientId = Guid.NewGuid(), PatientName = "Tran Thi B", ClinicId = Guid.NewGuid(), ClinicName = "Clinic B", Status = "InReview", RiskLevel = "High", IsCritical = true, CreatedAt = DateTime.UtcNow.AddHours(-3) },
            new() { Id = Guid.NewGuid(), ScreeningCode = "SCR-2024-003", PatientId = Guid.NewGuid(), PatientName = "Le Van C", ClinicId = Guid.NewGuid(), ClinicName = "Clinic A", Status = "Completed", RiskLevel = "Moderate", IsCritical = false, CreatedAt = DateTime.UtcNow.AddHours(-4), CompletedAt = DateTime.UtcNow.AddHours(-3) },
            new() { Id = Guid.NewGuid(), ScreeningCode = "SCR-2024-004", PatientId = Guid.NewGuid(), PatientName = "Pham Thi D", ClinicId = Guid.NewGuid(), ClinicName = "Clinic C", Status = "Submitted", RiskLevel = null, IsCritical = false, CreatedAt = DateTime.UtcNow.AddHours(-5) },
            new() { Id = Guid.NewGuid(), ScreeningCode = "SCR-2024-005", PatientId = Guid.NewGuid(), PatientName = "Hoang Van E", ClinicId = Guid.NewGuid(), ClinicName = "Clinic B", Status = "Completed", RiskLevel = "Critical", IsCritical = true, CreatedAt = DateTime.UtcNow.AddHours(-6), CompletedAt = DateTime.UtcNow.AddHours(-5) }
        };

        var pagedResult = new PagedResult<RecentScreeningDto>(items, 50, request.PageNumber, request.PageSize);
        return Task.FromResult(Result<PagedResult<RecentScreeningDto>>.Success(pagedResult));
    }
}
