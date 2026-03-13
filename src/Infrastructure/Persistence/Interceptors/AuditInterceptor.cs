using System.Text.Json;
using Application.Common.Interfaces;
using Domain.Entities.Platform;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace Infrastructure.Persistence.Interceptors;

/// <summary>
/// EF Core SaveChanges interceptor that automatically creates AuditLog entries
/// for all Insert/Update/Delete operations (except AuditLog itself to prevent infinite loops).
/// Fulfills FR-43: Log system activities and financial transactions for auditing and compliance.
/// </summary>
public class AuditInterceptor : SaveChangesInterceptor
{
    private readonly ICurrentUserService _currentUserService;

    private static readonly HashSet<string> SensitiveProperties = new(StringComparer.OrdinalIgnoreCase)
    {
        "PasswordHash",
        "SecurityStamp",
        "ConcurrencyStamp",
        "TwoFactorEnabled",
        "AccessFailedCount",
        "LockoutEnd",
        "LockoutEnabled",
        "NormalizedEmail",
        "NormalizedUserName",
        "PhoneNumberConfirmed",
        "EmailConfirmed",
        "RefreshToken",
        "Token",
        "Secret",
        "SecretKey"
    };

    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        WriteIndented = false,
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
    };

    public AuditInterceptor(ICurrentUserService currentUserService)
    {
        _currentUserService = currentUserService;
    }

    public override ValueTask<InterceptionResult<int>> SavingChangesAsync(
        DbContextEventData eventData,
        InterceptionResult<int> result,
        CancellationToken cancellationToken = default)
    {
        if (eventData.Context is null)
            return base.SavingChangesAsync(eventData, result, cancellationToken);

        var auditEntries = OnBeforeSaveChanges(eventData.Context);

        foreach (var auditLog in auditEntries)
        {
            eventData.Context.Add(auditLog);
        }

        return base.SavingChangesAsync(eventData, result, cancellationToken);
    }

    private List<AuditLog> OnBeforeSaveChanges(DbContext context)
    {
        context.ChangeTracker.DetectChanges();
        var auditEntries = new List<AuditLog>();

        var userId = _currentUserService.UserId;
        var ipAddress = _currentUserService.IpAddress;

        foreach (var entry in context.ChangeTracker.Entries())
        {
            // Skip AuditLog itself to prevent infinite loop
            if (entry.Entity is AuditLog)
                continue;

            // Skip unchanged or detached entities
            if (entry.State == EntityState.Unchanged || entry.State == EntityState.Detached)
                continue;

            var entityName = entry.Entity.GetType().Name;
            var entityId = entry.Properties
                .FirstOrDefault(p => p.Metadata.Name == "Id")?.CurrentValue?.ToString();

            string action;
            string? oldValue = null;
            string? newValue = null;

            switch (entry.State)
            {
                case EntityState.Added:
                    action = "Insert";
                    newValue = SerializeProperties(
                        entry.Properties
                            .Where(p => p.CurrentValue != null)
                            .ToDictionary(p => p.Metadata.Name, p => p.CurrentValue));
                    break;

                case EntityState.Modified:
                    action = "Update";
                    var changedProperties = entry.Properties
                        .Where(p => p.IsModified)
                        .ToList();

                    if (changedProperties.Count == 0)
                        continue;

                    oldValue = SerializeProperties(
                        changedProperties.ToDictionary(
                            p => p.Metadata.Name,
                            p => p.OriginalValue));
                    newValue = SerializeProperties(
                        changedProperties.ToDictionary(
                            p => p.Metadata.Name,
                            p => p.CurrentValue));
                    break;

                case EntityState.Deleted:
                    action = "Delete";
                    oldValue = SerializeProperties(
                        entry.Properties
                            .Where(p => p.OriginalValue != null)
                            .ToDictionary(p => p.Metadata.Name, p => p.OriginalValue));
                    break;

                default:
                    continue;
            }

            var auditLog = new AuditLog(
                action: action,
                entityName: entityName,
                entityId: entityId,
                userId: userId,
                oldValue: oldValue,
                newValue: newValue,
                ipAddress: ipAddress);

            auditEntries.Add(auditLog);
        }

        return auditEntries;
    }

    private static string? SerializeProperties(Dictionary<string, object?> properties)
    {
        var sanitizedProperties = properties
            .Where(x => !SensitiveProperties.Contains(x.Key))
            .ToDictionary(x => x.Key, x => x.Value);

        if (sanitizedProperties.Count == 0)
            return null;

        return JsonSerializer.Serialize(sanitizedProperties, JsonOptions);
    }
}
