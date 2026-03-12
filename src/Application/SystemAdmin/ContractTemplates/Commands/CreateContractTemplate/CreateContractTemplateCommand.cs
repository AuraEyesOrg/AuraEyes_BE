using Application.Common.Interfaces;
using Application.SystemAdmin.ContractTemplates.Common;
using Domain.Enums;

namespace Application.SystemAdmin.ContractTemplates.Commands.CreateContractTemplate;

public record CreateContractTemplateCommand : ICommand<ContractTemplateDetailDto>
{
    public string Title { get; init; } = string.Empty;
    public ContractType Type { get; init; }
    public string ContractVersion { get; init; } = string.Empty;
    public string ContentTemplate { get; init; } = string.Empty;
    public DateTime? EffectiveDate { get; init; }
    public List<CreateVariableRequest> Variables { get; init; } = new();
}

public record CreateVariableRequest
{
    public string Key { get; init; } = string.Empty;
    public string Label { get; init; } = string.Empty;
    public VariableType VariableType { get; init; }
    public string? Description { get; init; }
    public string? DefaultValue { get; init; }
    public string? SelectOptions { get; init; }
    public string? Unit { get; init; }
    public bool IsRequired { get; init; }
    public int SortOrder { get; init; }
}
