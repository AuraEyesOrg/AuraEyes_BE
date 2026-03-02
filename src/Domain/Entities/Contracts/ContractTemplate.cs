using Domain.Common;
using Domain.Enums;

namespace Domain.Entities.Contracts;

/// <summary>
/// Contract Template entity - templates for contracts
/// </summary>
public class ContractTemplate : BaseEntity, IAggregateRoot
{
    public string Title { get; private set; } = string.Empty;
    public ContractType Type { get; private set; }
    public string ContractVersion { get; private set; } = string.Empty;
    public string ContentTemplate { get; private set; } = string.Empty;
    public bool IsActive { get; private set; }
    public DateTime? EffectiveDate { get; private set; }

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

    public void Update(string title, string contentTemplate)
    {
        if (string.IsNullOrWhiteSpace(title))
            throw new ArgumentException("Title cannot be empty", nameof(title));
        if (string.IsNullOrWhiteSpace(contentTemplate))
            throw new ArgumentException("Content template cannot be empty", nameof(contentTemplate));

        Title = title;
        ContentTemplate = contentTemplate;
        UpdatedAt = DateTime.UtcNow;
    }

    public void Activate()
    {
        IsActive = true;
        UpdatedAt = DateTime.UtcNow;
    }

    public void Deactivate()
    {
        IsActive = false;
        UpdatedAt = DateTime.UtcNow;
    }
}
