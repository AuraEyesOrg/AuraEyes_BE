using Application.Common.Interfaces;

namespace Application.OrganisationPatients.Commands.UpdateOrganisationPatientContact;

public record UpdateOrganisationPatientContactCommand : ICommand<Guid>
{
    public required Guid OrgAdminUserId { get; init; }
    public required Guid PatientId { get; init; }
    public string? Address { get; init; }
    public string? PhoneNumber { get; init; }
    public string? Email { get; init; }
}