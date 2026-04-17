namespace Domain.Enums;

/// <summary>
/// Approval lifecycle for ophthalmologist employment type change requests.
/// </summary>
public enum OphthalmologistEmploymentTypeChangeRequestStatus
{
    Pending = 1,
    Approved = 2,
    Rejected = 3,
    Cancelled = 4
}
