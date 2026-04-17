namespace Application.SystemAdmin.Organisations.Common;

public record OrganisationOnboardingRequestDto
{
    public Guid Id { get; init; }
    public string OrganisationName { get; init; } = string.Empty;
    public string OrgType { get; init; } = string.Empty;
    public string ContactFullName { get; init; } = string.Empty;
    public string ContactEmail { get; init; } = string.Empty;
    public string? ContactPhone { get; init; }
    public string? Address { get; init; }
    public string? LicenseNumber { get; init; }
    public string? TaxCode { get; init; }
    public string? Notes { get; init; }
    public string Status { get; init; } = string.Empty;
    public DateTime CreatedAt { get; init; }
    public DateTime? ApprovedAt { get; init; }
}

public record ApproveOrganisationOnboardingResult
{
    public Guid RequestId { get; init; }
    public Guid OrganisationId { get; init; }
    public Guid OrgAdminUserId { get; init; }
    public string OrgAdminEmail { get; init; } = string.Empty;
    public string TemporaryPassword { get; init; } = string.Empty;
}