using Domain.Common;
using Domain.Entities.Users;
using Domain.Enums;

namespace Domain.Entities.Scheduling;

/// <summary>
/// Leave request submitted by a full-time ophthalmologist and reviewed by system admin.
/// </summary>
public class OphthalmologistLeaveRequest : BaseEntity, IAggregateRoot
{
    public Guid OphthalmologistId { get; private set; }
    public DateOnly StartDate { get; private set; }
    public DateOnly EndDate { get; private set; }
    public string Reason { get; private set; } = string.Empty;
    public OphthalmologistLeaveRequestStatus Status { get; private set; }
    public string? AdminNote { get; private set; }
    public Guid? ReviewedByAdminUserId { get; private set; }
    public DateTime? ReviewedAt { get; private set; }

    public Ophthalmologist? Ophthalmologist { get; private set; }

    private OphthalmologistLeaveRequest()
    {
    }

    public static OphthalmologistLeaveRequest Create(
        Guid ophthalmologistId,
        DateOnly startDate,
        DateOnly endDate,
        string reason)
    {
        if (ophthalmologistId == Guid.Empty)
            throw new ArgumentException("Ophthalmologist ID is required.", nameof(ophthalmologistId));

        if (endDate < startDate)
            throw new ArgumentException("EndDate must be greater than or equal to StartDate.", nameof(endDate));

        if (string.IsNullOrWhiteSpace(reason))
            throw new ArgumentException("Reason is required.", nameof(reason));

        return new OphthalmologistLeaveRequest
        {
            OphthalmologistId = ophthalmologistId,
            StartDate = startDate,
            EndDate = endDate,
            Reason = reason.Trim(),
            Status = OphthalmologistLeaveRequestStatus.Pending
        };
    }

    public void Approve(Guid reviewedByAdminUserId, string? adminNote = null)
    {
        EnsurePendingStatus();

        if (reviewedByAdminUserId == Guid.Empty)
            throw new ArgumentException("Admin user ID is required.", nameof(reviewedByAdminUserId));

        Status = OphthalmologistLeaveRequestStatus.Approved;
        ReviewedByAdminUserId = reviewedByAdminUserId;
        ReviewedAt = DateTime.UtcNow;
        AdminNote = NormalizeAdminNote(adminNote);
        UpdatedAt = DateTime.UtcNow;
    }

    public void Reject(Guid reviewedByAdminUserId, string? adminNote = null)
    {
        EnsurePendingStatus();

        if (reviewedByAdminUserId == Guid.Empty)
            throw new ArgumentException("Admin user ID is required.", nameof(reviewedByAdminUserId));

        Status = OphthalmologistLeaveRequestStatus.Rejected;
        ReviewedByAdminUserId = reviewedByAdminUserId;
        ReviewedAt = DateTime.UtcNow;
        AdminNote = NormalizeAdminNote(adminNote);
        UpdatedAt = DateTime.UtcNow;
    }

    public void Cancel(Guid ophthalmologistId)
    {
        EnsurePendingStatus();

        if (ophthalmologistId == Guid.Empty)
            throw new ArgumentException("Ophthalmologist ID is required.", nameof(ophthalmologistId));

        if (ophthalmologistId != OphthalmologistId)
            throw new InvalidOperationException("Only the owner can cancel this leave request.");

        Status = OphthalmologistLeaveRequestStatus.Cancelled;
        UpdatedAt = DateTime.UtcNow;
    }

    public bool Overlaps(DateOnly fromDate, DateOnly toDate)
    {
        return StartDate <= toDate && EndDate >= fromDate;
    }

    private void EnsurePendingStatus()
    {
        if (Status != OphthalmologistLeaveRequestStatus.Pending)
            throw new InvalidOperationException("Only pending leave requests can be updated.");
    }

    private static string? NormalizeAdminNote(string? adminNote)
    {
        return string.IsNullOrWhiteSpace(adminNote) ? null : adminNote.Trim();
    }
}