using Application.Common.Interfaces;

namespace Application.Organisations.Commands.DeleteOrganisation;

/// <summary>
/// Command to delete (soft-delete) an organisation.
/// </summary>
public record DeleteOrganisationCommand(Guid Id) : ICommand;
