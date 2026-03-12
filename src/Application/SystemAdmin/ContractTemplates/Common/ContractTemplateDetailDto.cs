namespace Application.SystemAdmin.ContractTemplates.Common;

/// <summary>Detail DTO — includes the HTML content template and all variable definitions.</summary>
public class ContractTemplateDetailDto : ContractTemplateDto
{
    public string ContentTemplate { get; set; } = string.Empty;
    public List<ContractTemplateVariableDto> Variables { get; set; } = new();
}
