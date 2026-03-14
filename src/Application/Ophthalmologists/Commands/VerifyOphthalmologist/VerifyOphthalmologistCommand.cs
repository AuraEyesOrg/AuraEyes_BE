using Application.Common.Interfaces;

namespace Application.Ophthalmologists.Commands.VerifyOphthalmologist;

/// <summary>
/// Command to verify an ophthalmologist profile.
/// </summary>
public record VerifyOphthalmologistCommand(Guid Id) : ICommand;
