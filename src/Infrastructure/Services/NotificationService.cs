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
    private readonly IRepository<Notification> _notificationRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly INotificationHubService _hubService;
    private readonly ILogger<NotificationService> _logger;

    public NotificationService(
        IRepository<Notification> notificationRepository,
        IUnitOfWork unitOfWork,
        INotificationHubService hubService,
        ILogger<NotificationService> logger)
    {
        _notificationRepository = notificationRepository;
        _unitOfWork = unitOfWork;
        _hubService = hubService;
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
            string? payloadJson = null;
            if (payload != null)
            {
                payloadJson = JsonSerializer.Serialize(payload, new JsonSerializerOptions
                {
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                    WriteIndented = false
                });
            }

            referenceId ??= ExtractReferenceId(payloadJson);

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

            // Step C: Broadcast via SignalR to the specific user
            var notificationDto = new NotificationDto
            {
                Id = notification.Id,
                UserId = userId,
                Title = title,
                Message = message,
                Type = type,
                ReferenceId = referenceId,
                IsRead = false,
                Payload = payloadJson,
                CreatedAt = notification.CreatedAt
            };

            await _hubService.BroadcastToUserAsync(userId, notificationDto, cancellationToken);

            var unreadCount = await _notificationRepository
                .Query()
                .AsNoTracking()
                .CountAsync(n => n.UserId == userId && !n.IsRead, cancellationToken);

            await _hubService.BroadcastUnreadCountAsync(userId, unreadCount, cancellationToken);

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

    private static Guid? ExtractReferenceId(string? payloadJson)
    {
        if (string.IsNullOrWhiteSpace(payloadJson))
            return null;

        try
        {
            using var document = JsonDocument.Parse(payloadJson);
            var root = document.RootElement;

            if (root.ValueKind != JsonValueKind.Object)
                return null;

            var keys = new[]
            {
                "consultationId",
                "sessionId",
                "appointmentId",
                "screeningId",
                "aiScreeningId",
                "transactionId",
                "messageId"
            };

            foreach (var key in keys)
            {
                if (!root.TryGetProperty(key, out var value))
                    continue;

                if (value.ValueKind == JsonValueKind.String
                    && Guid.TryParse(value.GetString(), out var parsed))
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
}
