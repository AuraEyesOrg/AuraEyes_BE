using Domain.Common;

namespace Domain.Entities;

/// <summary>
/// Permission entity - granular permissions for authorization
/// </summary>
public class Permission : BaseEntity, IAggregateRoot
{
    public string Name { get; private set; } = string.Empty;
    public string DisplayName { get; private set; } = string.Empty;
    public string? Description { get; private set; }
    public string? Category { get; private set; }
    public bool IsActive { get; private set; }

    private Permission() { } // EF Core

    public Permission(string name, string displayName, string? description = null, string? category = null)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Permission name cannot be empty", nameof(name));
        if (string.IsNullOrWhiteSpace(displayName))
            throw new ArgumentException("Display name cannot be empty", nameof(displayName));

        Name = name;
        DisplayName = displayName;
        Description = description;
        Category = category;
        IsActive = true;
    }

    public void Update(string displayName, string? description, string? category)
    {
        if (string.IsNullOrWhiteSpace(displayName))
            throw new ArgumentException("Display name cannot be empty", nameof(displayName));

        DisplayName = displayName;
        Description = description;
        Category = category;
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
