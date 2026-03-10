using Domain.Common;
using Domain.Enums;

namespace Domain.Entities.Contracts;

/// <summary>
/// Describes a single dynamic variable inside a <see cref="ContractTemplate"/>.
/// Each variable maps to a <c>{{key}}</c> placeholder in the HTML content template.
/// </summary>
public class ContractTemplateVariable : BaseEntity
{
    /// <summary>The owning template.</summary>
    public Guid TemplateId { get; private set; }

    /// <summary>
    /// Technical key used in the HTML placeholder, e.g. <c>salary</c> → <c>{{salary}}</c>.
    /// Must be camelCase, no spaces.
    /// </summary>
    public string Key { get; private set; } = string.Empty;

    /// <summary>Human-readable label shown in the editor panel, e.g. "Base Salary".</summary>
    public string Label { get; private set; } = string.Empty;

    /// <summary>Data type / UI control hint for the variable.</summary>
    public VariableType VariableType { get; private set; }

    /// <summary>Optional description / tooltip text.</summary>
    public string? Description { get; private set; }

    /// <summary>Optional default value pre-filled when generating a contract.</summary>
    public string? DefaultValue { get; private set; }

    /// <summary>
    /// For <see cref="VariableType.Select"/> variables: JSON array of allowed option strings,
    /// e.g. <c>["Full-time","Part-time","Contract"]</c>.
    /// </summary>
    public string? SelectOptions { get; private set; }

    /// <summary>Optional display unit, e.g. "VNĐ/tháng", "giờ/tuần".</summary>
    public string? Unit { get; private set; }

    /// <summary>Whether this variable must be filled before the contract can be issued.</summary>
    public bool IsRequired { get; private set; }

    /// <summary>Display order in the editor panel (lower = shown first).</summary>
    public int SortOrder { get; private set; }

    private ContractTemplateVariable() { } // EF Core

    public ContractTemplateVariable(
        Guid templateId,
        string key,
        string label,
        VariableType variableType,
        string? description = null,
        string? defaultValue = null,
        string? selectOptions = null,
        string? unit = null,
        bool isRequired = false,
        int sortOrder = 0)
    {
        if (string.IsNullOrWhiteSpace(key))
            throw new ArgumentException("Variable key cannot be empty.", nameof(key));
        if (string.IsNullOrWhiteSpace(label))
            throw new ArgumentException("Variable label cannot be empty.", nameof(label));

        TemplateId = templateId;
        Key = key.Trim();
        Label = label.Trim();
        VariableType = variableType;
        Description = description;
        DefaultValue = defaultValue;
        SelectOptions = selectOptions;
        Unit = unit;
        IsRequired = isRequired;
        SortOrder = sortOrder;
    }

    public void Update(
        string label,
        VariableType variableType,
        string? description,
        string? defaultValue,
        string? selectOptions,
        string? unit,
        bool isRequired,
        int sortOrder)
    {
        if (string.IsNullOrWhiteSpace(label))
            throw new ArgumentException("Variable label cannot be empty.", nameof(label));

        Label = label.Trim();
        VariableType = variableType;
        Description = description;
        DefaultValue = defaultValue;
        SelectOptions = selectOptions;
        Unit = unit;
        IsRequired = isRequired;
        SortOrder = sortOrder;
        UpdatedAt = DateTime.UtcNow;
    }
}
