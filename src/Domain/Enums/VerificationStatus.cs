namespace Domain.Enums;

/// <summary>
/// Verification status for ophthalmologist credentials.
/// </summary>
public enum VerificationStatus
{
    /// <summary>
    /// Pending verification by admin.
    /// </summary>
    PendingVerification = 0,

    /// <summary>
    /// Verified and active - can accept cases.
    /// </summary>
    Approved = 1,

    /// <summary>
    /// Rejected by admin.
    /// </summary>
    Rejected = 2,

    /// <summary>
    /// Previously approved profile uploaded new credentials and is awaiting re-verification.
    /// </summary>
    PendingUpdate = 3
}
