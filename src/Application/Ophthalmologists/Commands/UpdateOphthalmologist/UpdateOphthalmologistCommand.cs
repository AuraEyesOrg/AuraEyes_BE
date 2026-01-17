using Application.Common.Interfaces;

namespace Application.Ophthalmologists.Commands.UpdateOphthalmologist;

public record UpdateOphthalmologistCommand : ICommand
{
    public Guid Id { get; init; }
    public string Bio { get; init; } = string.Empty;
    public int YearsOfExperience { get; init; }
}
