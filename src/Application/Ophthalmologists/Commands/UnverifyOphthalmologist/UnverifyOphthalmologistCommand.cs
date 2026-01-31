using Application.Common.Interfaces;

namespace Application.Ophthalmologists.Commands.UnverifyOphthalmologist;

/// <summary>
/// Command to unverify (revoke verification of) an ophthalmologist profile.
/// </summary>
public record UnverifyOphthalmologistCommand(Guid Id) : ICommand;
