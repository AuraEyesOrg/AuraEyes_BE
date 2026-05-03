using Domain.Enums;

namespace Application.Scheduling.Appointments.Common;

public class ClinicAppointmentDto
{
    public Guid Id { get; init; }
    public Guid PatientId { get; init; }
    public string? PatientName { get; init; }
    public string? PatientAvatarUrl { get; init; }
    public Guid SlotId { get; init; }
    public DateOnly Date { get; init; }
    public TimeOnly StartTime { get; init; }
    public TimeOnly EndTime { get; init; }
    public string? VisitReason { get; init; }
    /// <summary>Booking lifecycle status (AppointmentStatus).</summary>
    public string Status { get; init; } = string.Empty;

    /// <summary>Physical visit status when a visit exists (PatientVisitStatus).</summary>
    public string? VisitStatus { get; init; }

    /// <summary>Unified clinic pipeline step (matches queue flowState).</summary>
    public string? FlowState { get; init; }

    public DateTime CreatedAt { get; init; }
    public Guid? OrganisationId { get; init; }
    public string? OrganisationName { get; init; }
    public Guid? OphthalId { get; init; }
    public string? OphthalFullName { get; init; }
    public string? OphthalAvatarUrl { get; init; }
    public Guid? StaffId { get; init; }
    public string? StaffName { get; init; }

    /// <summary>
    /// True when the current patient has already submitted feedback for this
    /// </summary>
    public bool HasFeedback { get; init; }

    // Billing info for Clinic Staff
    public Guid? OrderId { get; init; }
    public decimal? TotalAmount { get; init; }
    public decimal? DepositAmount { get; init; }
    public bool IsPaidDeposit { get; init; }
    public decimal? RemainingAmount { get; init; }
    public decimal? PaidAmount { get; init; }
    public string? OrderStatus { get; init; }

    // Refund bank info
    public string? RefundBankNumber { get; init; }
    public string? RefundAccountName { get; init; }
    public string? RefundBankName { get; init; }
    public string? CancellationReason { get; init; }
}

public class ClinicAvailableSlotDto
{
    public Guid SlotId { get; init; }
    public DateOnly Date { get; init; }
    public TimeOnly StartTime { get; init; }
    public TimeOnly EndTime { get; init; }
    public int MaxCapacity { get; init; }
    public int BookedCount { get; init; }
    public int Remaining { get; init; }
    public decimal? Cost { get; init; }
}

public class CreateClinicAppointmentResult
{
    public Guid AppointmentId { get; init; }
    public Guid? VisitId { get; init; }
    public string Status { get; init; } = string.Empty;

    /// <summary>
    /// PayOS checkout URL to redirect the patient for deposit payment (30% of slot price).
    /// </summary>
    public string? PaymentUrl { get; init; }

    /// <summary>
    /// The created Order ID associated with this booking deposit.
    /// </summary>
    public Guid? OrderId { get; init; }

    /// <summary>
    /// Deposit amount (30% of full slot price) the patient needs to pay.
    /// </summary>
    public decimal? DepositAmount { get; init; }
}

public record AvailableDoctorDto(
    Guid Id,
    string Name,
    string? Avatar,
    string? DegreeLevel
);
