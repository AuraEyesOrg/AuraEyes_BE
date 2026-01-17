using Application.Common.Interfaces;

namespace Application.Ophthalmologists.Commands.CreateOphthalmologist;

public record CreateOphthalmologistCommand : ICommand<Guid>
{
    public Guid UserId { get; init; }
    public string Bio { get; init; } = string.Empty;
    public int YearsOfExperience { get; init; }
}
