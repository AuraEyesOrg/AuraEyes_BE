using Application.Common.Interfaces;

namespace Application.Ophthalmologists.Commands.DeleteOphthalmologist;

/// <summary>
/// Command to delete (soft-delete) an ophthalmologist profile.
/// </summary>
public record DeleteOphthalmologistCommand(Guid Id) : ICommand;
