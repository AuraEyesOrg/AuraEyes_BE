using Application.Common.Interfaces;
using Application.SystemAdmin.ContractTemplates.Common;

namespace Application.SystemAdmin.ContractTemplates.Commands.DuplicateContractTemplate;

public record DuplicateContractTemplateCommand(Guid SourceId) : ICommand<ContractTemplateDetailDto>;
