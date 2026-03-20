using Application.Common.Interfaces;
using Application.SystemAdmin.ContractTemplates.Common;
using Domain.Enums;

namespace Application.SystemAdmin.ContractTemplates.Commands.CreateContractTemplate;

public record CreateContractTemplateCommand : ICommand<ContractTemplateDetailDto>
{
    public string Title { get; init; } = string.Empty;
    public ContractType Type { get; init; }
    public OphthalmologistEmploymentType? EmploymentType { get; init; }
    public string ContractVersion { get; init; } = string.Empty;
    public string ContentTemplate { get; init; } = string.Empty;
    public DateTime? EffectiveDate { get; init; }
}
