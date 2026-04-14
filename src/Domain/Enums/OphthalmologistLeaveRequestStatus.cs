namespace Domain.Enums;

/// <summary>
/// Approval lifecycle for ophthalmologist leave requests.
/// </summary>
public enum OphthalmologistLeaveRequestStatus
{
    Pending = 1,
    Approved = 2,
    Rejected = 3,
    Cancelled = 4
}