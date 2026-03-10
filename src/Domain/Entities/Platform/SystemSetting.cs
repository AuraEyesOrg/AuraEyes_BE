namespace Domain.Entities.Platform;

/// <summary>
/// SystemSetting - dynamic config store (FREE_AI_QUOTA, quota price, commission rate, etc.).
/// Key is the primary key; not inheriting BaseEntity because the PK is a string, not Guid.
/// </summary>
public class SystemSetting
{
    public string Key { get; private set; } = string.Empty;
    public string Value { get; private set; } = string.Empty;
    public string? Description { get; private set; }

    private SystemSetting() { } // EF Core

    public SystemSetting(string key, string value, string? description = null)
    {
        if (string.IsNullOrWhiteSpace(key))
            throw new ArgumentException("Key cannot be empty", nameof(key));

        Key = key;
        Value = value;
        Description = description;
    }

    public void UpdateValue(string value)
    {
        Value = value;
    }
}
