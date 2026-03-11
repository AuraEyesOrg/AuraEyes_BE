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

    /// <summary>
    /// Merge <paramref name="variables"/> into the template's variable collection.
    /// Existing keys are updated in-place (preserving their row Id), keys absent from the
    /// incoming set are removed, and brand-new keys are inserted.  Using merge rather than
    /// Clear+AddAll avoids EF Core circular-dependency errors caused by the unique index on
    /// (TemplateId, Key) when both a deletion and an insertion for the same key land in the
    /// same SaveChanges call.
    /// </summary>
    public void SetVariables(IEnumerable<ContractTemplateVariable> variables)
    {
        var incoming = variables.ToList();
        var incomingKeys = incoming.Select(v => v.Key).ToHashSet(StringComparer.Ordinal);

        // Remove variables whose keys are no longer present
        foreach (var stale in _variables.Where(v => !incomingKeys.Contains(v.Key)).ToList())
            _variables.Remove(stale);

        // Update existing entries in-place; add truly new ones
        foreach (var v in incoming)
        {
            var existing = _variables.FirstOrDefault(e => e.Key == v.Key);
            if (existing is null)
                _variables.Add(v);
            else
                existing.Update(v.Label, v.VariableType, v.Description, v.DefaultValue,
                                v.SelectOptions, v.Unit, v.IsRequired, v.SortOrder);
        }

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
