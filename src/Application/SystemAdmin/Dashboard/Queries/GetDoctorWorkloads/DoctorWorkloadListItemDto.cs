namespace Application.SystemAdmin.Dashboard.Queries.GetDoctorWorkloads;

public class DoctorWorkloadListItemDto
{
    public Guid DoctorId { get; init; }
    public string DoctorName { get; init; } = string.Empty;
    public string? Email { get; init; }
    public string EmploymentType { get; init; } = string.Empty;
    public string PeriodType { get; init; } = string.Empty;
    public DateTime PeriodStart { get; init; }
    public DateTime PeriodEnd { get; init; }
    public decimal RequiredHours { get; init; }
    public decimal ActualHours { get; init; }
    public decimal CompletionRate { get; init; }
    public string Status { get; init; } = string.Empty;
    public bool WarningFlag { get; init; }
}
