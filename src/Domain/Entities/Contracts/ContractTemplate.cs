using Domain.Common;
using Domain.Enums;

namespace Domain.Entities.Contracts;

/// <summary>
/// Contract Template entity.
/// </summary>
public class ContractTemplate : BaseEntity, IAggregateRoot
{
    public string Title { get; private set; } = string.Empty;
    public ContractType Type { get; private set; }
    public OphthalmologistEmploymentType? EmploymentType { get; private set; }
    public string ContractVersion { get; private set; } = string.Empty;
    public string ContentTemplate { get; private set; } = string.Empty;
    public bool IsActive { get; private set; }
    public DateTime? EffectiveDate { get; private set; }

    private ContractTemplate() { } // EF Core

    public ContractTemplate(
        string title,
        ContractType type,
        string contractVersion,
        string contentTemplate,
        DateTime? effectiveDate = null,
        OphthalmologistEmploymentType? employmentType = null)
    {
        if (string.IsNullOrWhiteSpace(title))
            throw new ArgumentException("Title cannot be empty", nameof(title));
        if (string.IsNullOrWhiteSpace(contractVersion))
            throw new ArgumentException("Contract version cannot be empty", nameof(contractVersion));
        if (string.IsNullOrWhiteSpace(contentTemplate))
            throw new ArgumentException("Content template cannot be empty", nameof(contentTemplate));

        if (type == ContractType.OphthalmologistContract && !employmentType.HasValue)
            throw new ArgumentException("Employment type is required for ophthalmologist contract templates", nameof(employmentType));

        Title = title;
        Type = type;
        EmploymentType = type == ContractType.OphthalmologistContract ? employmentType : null;
        ContractVersion = contractVersion;
        ContentTemplate = contentTemplate;
        EffectiveDate = effectiveDate;
        IsActive = true;
    }

    /// <summary>Update the template metadata and HTML content.</summary>
    public void Update(
        string title,
        ContractType type,
        string contractVersion,
        string contentTemplate,
        DateTime? effectiveDate,
        OphthalmologistEmploymentType? employmentType)
    {
        if (string.IsNullOrWhiteSpace(title))
            throw new ArgumentException("Title cannot be empty", nameof(title));
        if (string.IsNullOrWhiteSpace(contractVersion))
            throw new ArgumentException("Contract version cannot be empty", nameof(contractVersion));
        if (string.IsNullOrWhiteSpace(contentTemplate))
            throw new ArgumentException("Content template cannot be empty", nameof(contentTemplate));

        if (type == ContractType.OphthalmologistContract && !employmentType.HasValue)
            throw new ArgumentException("Employment type is required for ophthalmologist contract templates", nameof(employmentType));

        Title = title;
        Type = type;
        EmploymentType = type == ContractType.OphthalmologistContract ? employmentType : null;
        ContractVersion = contractVersion;
        ContentTemplate = contentTemplate;
        EffectiveDate = effectiveDate;
        UpdatedAt = DateTime.UtcNow;
    }

    public void Activate() { IsActive = true; UpdatedAt = DateTime.UtcNow; }
    public void Deactivate() { IsActive = false; UpdatedAt = DateTime.UtcNow; }

    public void SoftDelete() { IsDeleted = true; UpdatedAt = DateTime.UtcNow; }
}
