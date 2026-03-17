using Domain.Common;
using Domain.Enums;

namespace Domain.Entities.Users;

public class OrganisationOnboardingRequest : BaseEntity, IAggregateRoot
{
    public string OrganisationName { get; private set; } = string.Empty;
    public OrgType OrgType { get; private set; }
    public string ContactFullName { get; private set; } = string.Empty;
    public string ContactEmail { get; private set; } = string.Empty;
    public string? ContactPhone { get; private set; }
    public string? Address { get; private set; }
    public string? LicenseNumber { get; private set; }
    public string? Notes { get; private set; }
    public OrganisationOnboardingStatus Status { get; private set; }
    public DateTime? ApprovedAt { get; private set; }
    public Guid? ApprovedByUserId { get; private set; }
    public Guid? OrganisationId { get; private set; }
    public Guid? OrgAdminUserId { get; private set; }

    private OrganisationOnboardingRequest() { }

    public OrganisationOnboardingRequest(
        string organisationName,
        OrgType orgType,
        string contactFullName,
        string contactEmail,
        string? contactPhone = null,
        string? address = null,
        string? licenseNumber = null,
        string? notes = null)
    {
        if (string.IsNullOrWhiteSpace(organisationName))
            throw new ArgumentException("Organisation name cannot be empty.", nameof(organisationName));
        if (string.IsNullOrWhiteSpace(contactFullName))
            throw new ArgumentException("Contact full name cannot be empty.", nameof(contactFullName));
        if (string.IsNullOrWhiteSpace(contactEmail))
            throw new ArgumentException("Contact email cannot be empty.", nameof(contactEmail));

        OrganisationName = organisationName.Trim();
        OrgType = orgType;
        ContactFullName = contactFullName.Trim();
        ContactEmail = contactEmail.Trim();
        ContactPhone = string.IsNullOrWhiteSpace(contactPhone) ? null : contactPhone.Trim();
        Address = string.IsNullOrWhiteSpace(address) ? null : address.Trim();
        LicenseNumber = string.IsNullOrWhiteSpace(licenseNumber) ? null : licenseNumber.Trim();
        Notes = string.IsNullOrWhiteSpace(notes) ? null : notes.Trim();
        Status = OrganisationOnboardingStatus.Pending;
    }

    public void Approve(Guid approvedByUserId, Guid organisationId, Guid orgAdminUserId)
    {
        if (Status != OrganisationOnboardingStatus.Pending)
            throw new InvalidOperationException("Only pending onboarding requests can be approved.");

        Status = OrganisationOnboardingStatus.Approved;
        ApprovedByUserId = approvedByUserId;
        ApprovedAt = DateTime.UtcNow;
        OrganisationId = organisationId;
        OrgAdminUserId = orgAdminUserId;
        UpdatedAt = DateTime.UtcNow;
    }
}