using Domain.Common;
using Domain.Enums;

namespace Domain.Entities.Contracts;

/// <summary>
/// Contract Template entity — stores an HTML layout with <c>{{variable}}</c> placeholders
/// plus structured variable metadata (key, type, label, options) used by the editor UI.
/// </summary>
public class ContractTemplate : BaseEntity, IAggregateRoot
{
    public string Title { get; private set; } = string.Empty;
    public ContractType Type { get; private set; }
    public string ContractVersion { get; private set; } = string.Empty;
    public string ContentTemplate { get; private set; } = string.Empty;
    public bool IsActive { get; private set; }
    public DateTime? EffectiveDate { get; private set; }

    private readonly List<ContractTemplateVariable> _variables = new();

    /// <summary>Structured definition of every <c>{{key}}</c> placeholder in <see cref="ContentTemplate"/>.</summary>
    public IReadOnlyCollection<ContractTemplateVariable> Variables => _variables.AsReadOnly();

    private ContractTemplate() { } // EF Core

    public ContractTemplate(string title, ContractType type, string contractVersion, string contentTemplate, DateTime? effectiveDate = null)
    {
        if (string.IsNullOrWhiteSpace(title))
            throw new ArgumentException("Title cannot be empty", nameof(title));
        if (string.IsNullOrWhiteSpace(contractVersion))
            throw new ArgumentException("Contract version cannot be empty", nameof(contractVersion));
        if (string.IsNullOrWhiteSpace(contentTemplate))
            throw new ArgumentException("Content template cannot be empty", nameof(contentTemplate));

        Title = title;
        Type = type;
        ContractVersion = contractVersion;
        ContentTemplate = contentTemplate;
        EffectiveDate = effectiveDate;
        IsActive = true;
    }

    /// <summary>Update the template metadata and HTML content.</summary>
    public void Update(string title, ContractType type, string contractVersion, string contentTemplate, DateTime? effectiveDate)
    {
        if (string.IsNullOrWhiteSpace(title))
            throw new ArgumentException("Title cannot be empty", nameof(title));
        if (string.IsNullOrWhiteSpace(contractVersion))
            throw new ArgumentException("Contract version cannot be empty", nameof(contractVersion));
        if (string.IsNullOrWhiteSpace(contentTemplate))
            throw new ArgumentException("Content template cannot be empty", nameof(contentTemplate));

        Title = title;
        Type = type;
        ContractVersion = contractVersion;
        ContentTemplate = contentTemplate;
        EffectiveDate = effectiveDate;
        UpdatedAt = DateTime.UtcNow;
    }

    /// <summary>Replace all variable definitions with a new set (full-replace from editor save).</summary>
    public void SetVariables(IEnumerable<ContractTemplateVariable> variables)
    {
        _variables.Clear();
        foreach (var v in variables)
            _variables.Add(v);
        UpdatedAt = DateTime.UtcNow;
    }

    /// <summary>Add a single variable definition.</summary>
    public void AddVariable(ContractTemplateVariable variable)
    {
        if (_variables.Any(v => v.Key == variable.Key))
            throw new InvalidOperationException($"Variable key '{variable.Key}' already exists in this template.");
        _variables.Add(variable);
        UpdatedAt = DateTime.UtcNow;
    }

    public void Activate() { IsActive = true; UpdatedAt = DateTime.UtcNow; }
    public void Deactivate() { IsActive = false; UpdatedAt = DateTime.UtcNow; }

    public void SoftDelete() { IsDeleted = true; UpdatedAt = DateTime.UtcNow; }
}
