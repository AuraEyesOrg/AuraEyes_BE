using Application.Common.Interfaces;

namespace Application.OrganisationPatients.Commands.UpdateOrganisationPatientContact;

public record UpdateOrganisationPatientContactCommand : ICommand<Guid>
{
    public required Guid OrgAdminUserId { get; init; }
    public required Guid PatientId { get; init; }

    // ── Walk-in only (ignored for registered patients) ──
    public string? FullName { get; init; }
    public DateTime? DateOfBirth { get; init; }
    public string? Gender { get; init; }
    public string? CitizenId { get; init; }

    // ── Both flows ──
    public string? Address { get; init; }
    public string? PhoneNumber { get; init; }
    public decimal? Bmi { get; init; }
    public string? DiseaseHistory { get; init; }
}