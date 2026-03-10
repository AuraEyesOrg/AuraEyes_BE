using Application.Common.Interfaces;

namespace Application.SystemAdmin.ContractTemplates.Commands.DeleteContractTemplate;

public record DeleteContractTemplateCommand(Guid Id) : ICommand;
