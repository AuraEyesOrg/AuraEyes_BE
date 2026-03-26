using Domain.Common;

namespace Domain.Entities.Platform;

/// <summary>
/// Audit Log entity - tracks all changes in the system
/// OldValue and NewValue are stored as JSONB in PostgreSQL
/// </summary>
public class AuditLog : BaseEntity, IAggregateRoot
{
    public Guid? UserId { get; private set; }
    public string Action { get; private set; } = string.Empty;
    public string EntityName { get; private set; } = string.Empty;
    public string? EntityId { get; private set; }

    /// <summary>
    /// Previous state as JSON - stored as JSONB in PostgreSQL
    /// </summary>
    public string? OldValue { get; private set; }

    /// <summary>
    /// New state as JSON - stored as JSONB in PostgreSQL
    /// </summary>
    public string? NewValue { get; private set; }

    public string? IpAddress { get; private set; }

    private AuditLog() { } // EF Core

    public AuditLog(string action, string entityName, string? entityId = null, Guid? userId = null, string? oldValue = null, string? newValue = null, string? ipAddress = null)
    {
        if (string.IsNullOrWhiteSpace(action))
            throw new ArgumentException("Action cannot be empty", nameof(action));
        if (string.IsNullOrWhiteSpace(entityName))
            throw new ArgumentException("Entity name cannot be empty", nameof(entityName));

        Action = action;
        EntityName = entityName;
        EntityId = entityId;
        UserId = userId;
        OldValue = oldValue;
        NewValue = newValue;
        IpAddress = ipAddress;
    }
}
