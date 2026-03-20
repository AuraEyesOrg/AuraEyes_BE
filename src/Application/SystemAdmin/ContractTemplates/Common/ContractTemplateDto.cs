namespace Application.SystemAdmin.ContractTemplates.Common;

public class ContractTemplateDto
{
    public Guid Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty;
    public string? EmploymentType { get; set; }
    public string ContractVersion { get; set; } = string.Empty;
    public bool IsActive { get; set; }
    public int VariableCount { get; set; }

    /// <summary>Number of contracts currently linked to this template.</summary>
    public int UsageCount { get; set; }

    public DateTime? EffectiveDate { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}
