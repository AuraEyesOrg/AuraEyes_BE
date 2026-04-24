namespace Application.Common.Interfaces;

/// <summary>
/// Resolves the Organisation that the currently authenticated user belongs to.
/// Works for both OrgAdmin (Organisation.OwnerId) and ClinicStaff (ApplicationUser.OrganizationId).
/// </summary>
public interface ICurrentUserOrganisationService
{
    /// <summary>
    /// Returns the OrganisationId for the given user, regardless of whether they are
    /// an OrgAdmin or a ClinicStaff member.
    /// Returns null if the user does not belong to any organisation.
    /// </summary>
    Task<Guid?> GetOrganisationIdAsync(Guid userId, CancellationToken cancellationToken = default);
}
