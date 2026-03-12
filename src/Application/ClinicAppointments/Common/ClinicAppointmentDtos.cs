namespace Application.ClinicAppointments.Common;

/// <summary>
/// DTO for available slot in organisation booking flow.
/// </summary>
public record OrganisationAvailableSlotDto
{
    public Guid SlotId { get; init; }
    public DateOnly Date { get; init; }
    public TimeOnly StartTime { get; init; }
    public TimeOnly EndTime { get; init; }
    public int MaxCapacity { get; init; }
    public int BookedCount { get; init; }
    public int RemainingCapacity { get; init; }
    public decimal? Cost { get; init; }
}

/// <summary>
/// DTO for clinic appointment list items.
/// </summary>
public record ClinicAppointmentListDto
{
    public Guid Id { get; init; }
    public Guid PatientId { get; init; }
    public string PatientName { get; init; } = string.Empty;
    public string PatientEmail { get; init; } = string.Empty;
    public Guid OrganisationId { get; init; }
    public string OrganisationName { get; init; } = string.Empty;
    public DateOnly Date { get; init; }
    public TimeOnly StartTime { get; init; }
    public TimeOnly EndTime { get; init; }
    public string Status { get; init; } = string.Empty;
    public string? VisitReason { get; init; }
    public Guid? AssignedDoctorId { get; init; }
    public string? AssignedDoctorName { get; init; }
    public DateTime CreatedAt { get; init; }
}

/// <summary>
/// DTO for clinic appointment details.
/// </summary>
public record ClinicAppointmentDetailDto
{
    public Guid Id { get; init; }
    public Guid PatientId { get; init; }
    public string PatientName { get; init; } = string.Empty;
    public string PatientEmail { get; init; } = string.Empty;
    public string? PatientPhone { get; init; }
    public Guid OrganisationId { get; init; }
    public string OrganisationName { get; init; } = string.Empty;
    public string? OrganisationAddress { get; init; }
    public DateOnly Date { get; init; }
    public TimeOnly StartTime { get; init; }
    public TimeOnly EndTime { get; init; }
    public string Status { get; init; } = string.Empty;
    public string? VisitReason { get; init; }
    public string? Notes { get; init; }
    public Guid? AssignedDoctorId { get; init; }
    public string? AssignedDoctorName { get; init; }
    public DateTime? CheckedInAt { get; init; }
    public DateTime? CompletedAt { get; init; }
    public Guid? CancelledBy { get; init; }
    public string? CancellationReason { get; init; }
    public DateTime CreatedAt { get; init; }
    public DateTime? UpdatedAt { get; init; }
}

/// <summary>
/// DTO for organisation appointment summary by slot.
/// </summary>
public record SlotAppointmentSummaryDto
{
    public Guid SlotId { get; init; }
    public DateOnly Date { get; init; }
    public TimeOnly StartTime { get; init; }
    public TimeOnly EndTime { get; init; }
    public int MaxCapacity { get; init; }
    public int BookedCount { get; init; }
    public int CheckedInCount { get; init; }
    public int CompletedCount { get; init; }
    public List<ClinicAppointmentListDto> Appointments { get; init; } = new();
}

/// <summary>
/// DTO for appointment status counts.
/// </summary>
public record AppointmentStatusCountsDto
{
    public int PendingCount { get; init; }
    public int ConfirmedCount { get; init; }
    public int CheckedInCount { get; init; }
    public int InProgressCount { get; init; }
    public int CompletedCount { get; init; }
    public int CancelledCount { get; init; }
    public int NoShowCount { get; init; }
    public int TotalCount { get; init; }
}
