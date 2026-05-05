using System.Text.Json;
using Application.Common.Interfaces;
using Application.Common.Models;
using Domain.Common;
using Domain.Entities.Platform;
using Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Infrastructure.Services;

/// <summary>
/// Notification service that persists to database and broadcasts via SignalR
/// Implements real-time push notifications for the AURA system
/// </summary>
public class NotificationService : INotificationService
{
    private static readonly string[] GenericReferenceKeys =
    {
        "consultationSessionId",
        "consultationId",
        "sessionId",
        "appointmentId",
        "appointmentSlotId",
        "slotId",
        "screeningId",
        "aiScreeningId",
        "transactionId",
        "messageId",
        "ophthalmologistId",
        "ophthalmologistUserId"
    };

    private readonly IRepository<Notification> _notificationRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly INotificationHubService _hubService;
    private readonly IIdentityService _identityService;
    private readonly ILogger<NotificationService> _logger;

    public NotificationService(
        IRepository<Notification> notificationRepository,
        IUnitOfWork unitOfWork,
        INotificationHubService hubService,
        IIdentityService identityService,
        ILogger<NotificationService> logger)
    {
        _notificationRepository = notificationRepository;
        _unitOfWork = unitOfWork;
        _hubService = hubService;
        _identityService = identityService;
        _logger = logger;
    }

    /// <inheritdoc />
    public async Task SendAsync(
        Guid userId,
        string title,
        string message,
        NotificationType type,
        object? payload = null,
        CancellationToken cancellationToken = default,
        Guid? referenceId = null)
    {
        try
        {
            // Step A: Serialize payload to JSON string
            var payloadJson = SerializePayload(payload);

            referenceId ??= ExtractReferenceId(type, payloadJson);

            if (referenceId is null && RequiresReferenceId(type))
            {
                _logger.LogWarning(
                    "ReferenceId was not resolved for notification Type={Type}, UserId={UserId}",
                    type,
                    userId);
            }

            // Step B: Create and persist notification entity
            var notification = new Notification(
                userId,
                title,
                message,
                type,
                referenceId,
                payloadJson);

            await _notificationRepository.AddAsync(notification, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            _logger.LogInformation(
                "Notification persisted: Id={NotificationId}, UserId={UserId}, Type={Type}",
                notification.Id, userId, type);

            var unreadCount = await _notificationRepository
                .Query()
                .AsNoTracking()
                .CountAsync(n => n.UserId == userId && !n.IsRead, cancellationToken);

            await BroadcastPersistedNotificationAsync(
                notification,
                title,
                message,
                type,
                payloadJson,
                referenceId,
                unreadCount,
                cancellationToken);

            _logger.LogInformation(
                "Notification broadcasted via SignalR to User {UserId}",
                userId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex,
                "Failed to send notification to User {UserId}: {Message}",
                userId, ex.Message);
            throw;
        }
    }

    /// <inheritdoc />
    [Obsolete("Use SendAsync with NotificationType instead")]
    public async Task SendAsync(Guid userId, string message, CancellationToken cancellationToken = default)
    {
        await SendAsync(
            userId,
            "Session Reminder",
            message,
            NotificationType.NewConsultationRequest,
            null,
            cancellationToken,
            null);
    }

    /// <inheritdoc />
    public async Task SendToRoleAsync(
        string roleName,
        string title,
        string message,
        NotificationType type = NotificationType.SystemAlert,
        object? payload = null,
        CancellationToken cancellationToken = default,
        Guid? referenceId = null)
    {
        var (users, _) = await _identityService.GetUsersAsync(
            roleFilter: roleName,
            pageSize: 1000,
            cancellationToken: cancellationToken);

        if (users == null || users.Count == 0)
        {
            _logger.LogWarning("No users found in role {RoleName} to send notification.", roleName);
            return;
        }

        var payloadJson = SerializePayload(payload);
        var resolvedReferenceId = referenceId ?? ExtractReferenceId(type, payloadJson);
        var notifications = new List<Notification>(users.Count);

        foreach (var user in users)
        {
            var notification = new Notification(
                user.Id,
                title,
                message,
                type,
                resolvedReferenceId,
                payloadJson);

            notifications.Add(notification);
            await _notificationRepository.AddAsync(notification, cancellationToken);
        }

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        var userIds = users.Select(u => u.Id).ToArray();
        var unreadCounts = await _notificationRepository
            .Query()
            .AsNoTracking()
            .Where(n => userIds.Contains(n.UserId) && !n.IsRead)
            .GroupBy(n => n.UserId)
            .Select(group => new
            {
                UserId = group.Key,
                Count = group.Count()
            })
            .ToDictionaryAsync(item => item.UserId, item => item.Count, cancellationToken);

        var broadcastTasks = notifications.Select(notification =>
        {
            var unreadCount = unreadCounts.GetValueOrDefault(notification.UserId, 0);
            return BroadcastPersistedNotificationAsync(
                notification,
                title,
                message,
                type,
                payloadJson,
                resolvedReferenceId,
                unreadCount,
                cancellationToken);
        });

        await Task.WhenAll(broadcastTasks);
    }

    private static string? SerializePayload(object? payload)
    {
        if (payload == null)
        {
            return null;
        }

        return JsonSerializer.Serialize(payload, new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            WriteIndented = false
        });
    }

    private async Task BroadcastPersistedNotificationAsync(
        Notification notification,
        string title,
        string message,
        NotificationType type,
        string? payloadJson,
        Guid? referenceId,
        int unreadCount,
        CancellationToken cancellationToken)
    {
        var notificationDto = new NotificationDto
        {
            Id = notification.Id,
            UserId = notification.UserId,
            Title = title,
            Message = message,
            Type = type,
            ReferenceId = referenceId,
            IsRead = false,
            Payload = payloadJson,
            CreatedAt = notification.CreatedAt
        };

        await _hubService.BroadcastToUserAsync(notification.UserId, notificationDto, cancellationToken);
        await _hubService.BroadcastUnreadCountAsync(notification.UserId, unreadCount, cancellationToken);
    }

    private static Guid? ExtractReferenceId(NotificationType type, string? payloadJson)
    {
        if (string.IsNullOrWhiteSpace(payloadJson))
            return null;

        try
        {
            using var document = JsonDocument.Parse(payloadJson);
            var root = document.RootElement;

            if (root.ValueKind != JsonValueKind.Object)
                return null;

            var prioritizedKeys = GetReferenceKeysByType(type);
            foreach (var key in prioritizedKeys)
            {
                if (TryReadGuid(root, key, out var parsed))
                {
                    return parsed;
                }
            }

            foreach (var key in GenericReferenceKeys)
            {
                if (prioritizedKeys.Contains(key, StringComparer.Ordinal))
                    continue;

                if (TryReadGuid(root, key, out var parsed))
                {
                    return parsed;
                }
            }
        }
        catch
        {
            // Ignore malformed payload content and keep reference id unset.
        }

        return null;
    }

    private static IReadOnlyCollection<string> GetReferenceKeysByType(NotificationType type)
    {
        return type switch
        {
            NotificationType.AiScreeningCompleted =>
                new[] { "aiScreeningId", "screeningId" },

            NotificationType.ConsultationAccepted or
            NotificationType.ConsultationResultProvided or
            NotificationType.NewConsultationRequest or
            NotificationType.NewPatientMessage =>
                new[] { "consultationSessionId", "consultationId", "sessionId" },

            NotificationType.NewAppointmentBooked or
            NotificationType.ScheduleChanged =>
                new[]
                {
                    "appointmentId",
                    "appointmentSlotId",
                    "slotId",
                    "consultationSessionId",
                    "sessionId"
                },

            NotificationType.WalletDepositSuccess or
            NotificationType.WalletPaymentProcessed =>
                new[] { "transactionId" },

            NotificationType.SystemAlert =>
                new[] { "ophthalmologistId", "ophthalmologistUserId" },

            _ => Array.Empty<string>()
        };
    }

    private static bool TryReadGuid(JsonElement root, string key, out Guid value)
    {
        value = Guid.Empty;

        if (!root.TryGetProperty(key, out var element))
            return false;

        if (element.ValueKind != JsonValueKind.String)
            return false;

        return Guid.TryParse(element.GetString(), out value);
    }

    private static bool RequiresReferenceId(NotificationType type)
    {
        return type != NotificationType.SystemAlert;
    }
}
