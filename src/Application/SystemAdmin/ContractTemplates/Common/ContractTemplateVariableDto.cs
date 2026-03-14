namespace Application.SystemAdmin.ContractTemplates.Common;

public class ContractTemplateVariableDto
{
    public Guid Id { get; set; }
    public string Key { get; set; } = string.Empty;
    public string Label { get; set; } = string.Empty;
    public string VariableType { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? DefaultValue { get; set; }

    /// <summary>JSON array string for Select-type variables, e.g. ["Full-time","Part-time"].</summary>
    public string? SelectOptions { get; set; }

    public string? Unit { get; set; }
    public bool IsRequired { get; set; }
    public int SortOrder { get; set; }
}
