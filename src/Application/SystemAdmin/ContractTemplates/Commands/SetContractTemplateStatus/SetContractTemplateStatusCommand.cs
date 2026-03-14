using Application.Common.Interfaces;

namespace Application.SystemAdmin.ContractTemplates.Commands.SetContractTemplateStatus;

/// <summary>Activate or deactivate a contract template.</summary>
public record SetContractTemplateStatusCommand(Guid Id, bool IsActive) : ICommand;
