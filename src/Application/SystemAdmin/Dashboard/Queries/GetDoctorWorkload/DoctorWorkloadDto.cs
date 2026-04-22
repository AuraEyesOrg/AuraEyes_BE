namespace Application.SystemAdmin.Dashboard.Queries.GetDoctorWorkload;

public class DoctorWorkloadDto
{
    public Guid DoctorId { get; init; }
    public string PeriodType { get; init; } = string.Empty;
    public DateTime PeriodStart { get; init; }
    public DateTime PeriodEnd { get; init; }
    public decimal RequiredHours { get; init; }
    public decimal ActualHours { get; init; }
    public decimal CompletionRate { get; init; }
    public string Status { get; init; } = string.Empty;
}
