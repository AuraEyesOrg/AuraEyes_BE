using Application.Common.Interfaces;

namespace Application.OrganisationPatients.Commands.CreateWalkInPatient;

public record CreateWalkInPatientCommand : ICommand<Guid>
{
    public required string FullName { get; init; }
    public required DateTime DateOfBirth { get; init; }
    public required string Gender { get; init; }
    public string? PhoneNumber { get; init; }
    public string? Email { get; init; }
    public string? CitizenId { get; init; }
}
