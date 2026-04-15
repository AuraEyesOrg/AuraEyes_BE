using Domain.Common;
using Domain.Enums;

namespace Domain.Entities.Users;

/// <summary>
/// Request submitted by an ophthalmologist to change employment type and reviewed by system admin.
/// </summary>
public class OphthalmologistEmploymentTypeChangeRequest : BaseEntity, IAggregateRoot
{
    public Guid OphthalmologistId { get; private set; }
    public OphthalmologistEmploymentType CurrentEmploymentType { get; private set; }
    public OphthalmologistEmploymentType TargetEmploymentType { get; private set; }
    public string Reason { get; private set; } = string.Empty;
    public OphthalmologistEmploymentTypeChangeRequestStatus Status { get; private set; }
    public string? AdminNote { get; private set; }
    public Guid? ReviewedByAdminUserId { get; private set; }
    public DateTime? ReviewedAt { get; private set; }

    public Ophthalmologist? Ophthalmologist { get; private set; }

    private OphthalmologistEmploymentTypeChangeRequest()
    {
    }

    public static OphthalmologistEmploymentTypeChangeRequest Create(
        Guid ophthalmologistId,
        OphthalmologistEmploymentType currentEmploymentType,
        OphthalmologistEmploymentType targetEmploymentType,
        string reason)
    {
        if (ophthalmologistId == Guid.Empty)
            throw new ArgumentException("Ophthalmologist ID is required.", nameof(ophthalmologistId));

        if (currentEmploymentType == targetEmploymentType)
            throw new ArgumentException("Target employment type must be different from current employment type.", nameof(targetEmploymentType));

        if (string.IsNullOrWhiteSpace(reason))
            throw new ArgumentException("Reason is required.", nameof(reason));

        return new OphthalmologistEmploymentTypeChangeRequest
        {
            OphthalmologistId = ophthalmologistId,
            CurrentEmploymentType = currentEmploymentType,
            TargetEmploymentType = targetEmploymentType,
            Reason = reason.Trim(),
            Status = OphthalmologistEmploymentTypeChangeRequestStatus.Pending
        };
    }

    public void Approve(Guid reviewedByAdminUserId, string? adminNote = null)
    {
        EnsurePendingStatus();

        if (reviewedByAdminUserId == Guid.Empty)
            throw new ArgumentException("Admin user ID is required.", nameof(reviewedByAdminUserId));

        Status = OphthalmologistEmploymentTypeChangeRequestStatus.Approved;
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

        Status = OphthalmologistEmploymentTypeChangeRequestStatus.Rejected;
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
            throw new InvalidOperationException("Only the owner can cancel this employment type change request.");

        Status = OphthalmologistEmploymentTypeChangeRequestStatus.Cancelled;
        UpdatedAt = DateTime.UtcNow;
    }

    private void EnsurePendingStatus()
    {
        if (Status != OphthalmologistEmploymentTypeChangeRequestStatus.Pending)
            throw new InvalidOperationException("Only pending employment type change requests can be updated.");
    }

    private static string? NormalizeAdminNote(string? adminNote)
    {
        return string.IsNullOrWhiteSpace(adminNote) ? null : adminNote.Trim();
    }
}
