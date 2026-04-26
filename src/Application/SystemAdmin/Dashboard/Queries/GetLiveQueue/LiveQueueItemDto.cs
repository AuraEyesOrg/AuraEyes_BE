namespace Application.SystemAdmin.Dashboard.Queries.GetLiveQueue;

public class LiveQueueItemDto
{
    public Guid VisitId { get; set; }
    public string PatientName { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public string? AssignedDoctorName { get; set; }
    public int WaitingTimeMinutes { get; set; }
    public DateTime? CheckedInAt { get; set; }
}
