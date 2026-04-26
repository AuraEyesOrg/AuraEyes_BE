using Application.Common.Interfaces;
using Application.Common.Models;
using Domain.Entities.Platform;
using Domain.Enums;
using FluentAssertions;
using Infrastructure.Persistence;
using Infrastructure.Persistence.Repositories;
using Infrastructure.Services;
using Infrastructure.UnitTests.Common;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.UnitTests.Services;

public class NotificationServiceTests
{
    [Fact]
    public async Task SendAsync_Typed_ShouldPersistAndBroadcastAndCountUnread()
    {
        await using var context = CreateContext();
        var repo = new Repository<Notification>(context);
        var hub = new FakeNotificationHubService();
        var logger = new TestLogger<NotificationService>();
        var identity = new FakeIdentityService();
        var service = new NotificationService(repo, context, hub, identity, logger);
        var userId = Guid.NewGuid();
        var sessionId = Guid.NewGuid();

        await service.SendAsync(
            userId,
            "New Session",
            "Session created",
            NotificationType.NewConsultationRequest,
            new { sessionId },
            CancellationToken.None);

        var persisted = await context.Notifications.FirstAsync();
        persisted.UserId.Should().Be(userId);
        persisted.Title.Should().Be("New Session");
        persisted.ReferenceId.Should().Be(sessionId);
        hub.BroadcastedToUser.Should().HaveCount(1);
        hub.UnreadCountEvents.Should().ContainSingle(x => x.UserId == userId && x.Count == 1);
    }

    [Fact]
    public async Task SendAsync_Typed_WithAppointmentIdPayload_ShouldExtractReferenceId()
    {
        await using var context = CreateContext();
        var repo = new Repository<Notification>(context);
        var hub = new FakeNotificationHubService();
        var logger = new TestLogger<NotificationService>();
        var identity = new FakeIdentityService();
        var service = new NotificationService(repo, context, hub, identity, logger);
        var userId = Guid.NewGuid();
        var appointmentId = Guid.NewGuid();

        await service.SendAsync(
            userId,
            "Appointment Updated",
            "Your appointment was updated",
            NotificationType.SystemAlert,
            new { appointmentId },
            CancellationToken.None);

        var persisted = await context.Notifications.FirstAsync();
        persisted.ReferenceId.Should().Be(appointmentId);
    }

    [Fact]
    public async Task SendAsync_Typed_WithMalformedStringPayload_ShouldKeepReferenceIdNull()
    {
        await using var context = CreateContext();
        var repo = new Repository<Notification>(context);
        var hub = new FakeNotificationHubService();
        var logger = new TestLogger<NotificationService>();
        var identity = new FakeIdentityService();
        var service = new NotificationService(repo, context, hub, identity, logger);
        var userId = Guid.NewGuid();

        // payload is string => serialized JSON string, extractor should ignore
        await service.SendAsync(
            userId,
            "Text Payload",
            "payload is text",
            NotificationType.SystemAlert,
            "not-an-object-json",
            CancellationToken.None);

        var persisted = await context.Notifications.FirstAsync();
        persisted.ReferenceId.Should().BeNull();
    }

    [Theory]
    [InlineData("consultationId")]
    [InlineData("sessionId")]
    [InlineData("appointmentId")]
    [InlineData("screeningId")]
    [InlineData("aiScreeningId")]
    [InlineData("transactionId")]
    [InlineData("messageId")]
    public async Task SendAsync_Typed_ShouldExtractReferenceId_FromKnownPayloadKeys(string keyName)
    {
        await using var context = CreateContext();
        var repo = new Repository<Notification>(context);
        var hub = new FakeNotificationHubService();
        var logger = new TestLogger<NotificationService>();
        var identity = new FakeIdentityService();
        var service = new NotificationService(repo, context, hub, identity, logger);
        var userId = Guid.NewGuid();
        var refId = Guid.NewGuid();
        var payload = new Dictionary<string, string> { [keyName] = refId.ToString() };

        await service.SendAsync(
            userId,
            "Ref Test",
            "testing known keys",
            NotificationType.SystemAlert,
            payload,
            CancellationToken.None);

        var persisted = await context.Notifications.FirstAsync();
        persisted.ReferenceId.Should().Be(refId);
    }

    [Fact]
    public async Task SendAsync_Legacy_ShouldUseDefaultTitleAndType()
    {
        await using var context = CreateContext();
        var repo = new Repository<Notification>(context);
        var hub = new FakeNotificationHubService();
        var logger = new TestLogger<NotificationService>();
        var identity = new FakeIdentityService();
        var service = new NotificationService(repo, context, hub, identity, logger);
        var userId = Guid.NewGuid();

        await service.SendAsync(userId, "Legacy message");

        var persisted = await context.Notifications.FirstAsync();
        persisted.Title.Should().Be("Session Reminder");
        persisted.Type.Should().Be(NotificationType.NewConsultationRequest);
    }

    [Fact]
    public async Task SendAsync_Typed_WithExplicitReferenceId_ShouldPreferExplicitOverPayload()
    {
        await using var context = CreateContext();
        var repo = new Repository<Notification>(context);
        var hub = new FakeNotificationHubService();
        var identity = new FakeIdentityService();
        var service = new NotificationService(repo, context, hub, identity, new TestLogger<NotificationService>());
        var userId = Guid.NewGuid();
        var explicitRef = Guid.NewGuid();
        var payloadRef = Guid.NewGuid();

        await service.SendAsync(
            userId,
            "Priority Ref",
            "explicit ref id wins",
            NotificationType.SystemAlert,
            new { appointmentId = payloadRef },
            CancellationToken.None,
            explicitRef);

        var saved = await context.Notifications.FirstAsync();
        saved.ReferenceId.Should().Be(explicitRef);
    }

    [Fact]
    public async Task SendAsync_Typed_ShouldBroadcastUnreadCountIncludingExistingUnreadNotifications()
    {
        await using var context = CreateContext();
        var userId = Guid.NewGuid();
        await context.Notifications.AddRangeAsync(
            new Notification(userId, "n1", "m1", NotificationType.SystemAlert, null, null),
            new Notification(userId, "n2", "m2", NotificationType.SystemAlert, null, null));
        await context.SaveChangesAsync();

        var repo = new Repository<Notification>(context);
        var hub = new FakeNotificationHubService();
        var identity = new FakeIdentityService();
        var service = new NotificationService(repo, context, hub, identity, new TestLogger<NotificationService>());

        await service.SendAsync(userId, "n3", "m3", NotificationType.SystemAlert, null, CancellationToken.None);

        hub.UnreadCountEvents.Should().ContainSingle(x => x.UserId == userId && x.Count == 3);
    }

    [Fact]
    public async Task SendAsync_WhenHubBroadcastFails_ShouldThrowButKeepPersistedNotification()
    {
        await using var context = CreateContext();
        var repo = new Repository<Notification>(context);
        var service = new NotificationService(
            repo,
            context,
            new ThrowingNotificationHubService(),
            new FakeIdentityService(),
            new TestLogger<NotificationService>());
        var userId = Guid.NewGuid();

        var act = async () => await service.SendAsync(
            userId,
            "Hub Fail",
            "broadcast fails",
            NotificationType.SystemAlert,
            null,
            CancellationToken.None);

        await act.Should().ThrowAsync<InvalidOperationException>();
        (await context.Notifications.CountAsync()).Should().Be(1);
    }

    private static ApplicationDbContext CreateContext()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
        return new ApplicationDbContext(options);
    }

    private sealed class FakeNotificationHubService : INotificationHubService
    {
        public List<(Guid UserId, NotificationDto Notification)> BroadcastedToUser { get; } = [];
        public List<(Guid UserId, int Count)> UnreadCountEvents { get; } = [];

        public Task BroadcastToUserAsync(Guid userId, NotificationDto notification, CancellationToken cancellationToken = default)
        {
            BroadcastedToUser.Add((userId, notification));
            return Task.CompletedTask;
        }

        public Task BroadcastUnreadCountAsync(Guid userId, int count, CancellationToken cancellationToken = default)
        {
            UnreadCountEvents.Add((userId, count));
            return Task.CompletedTask;
        }
    }

    private sealed class ThrowingNotificationHubService : INotificationHubService
    {
        public Task BroadcastToUserAsync(Guid userId, NotificationDto notification, CancellationToken cancellationToken = default)
            => throw new InvalidOperationException("hub down");

        public Task BroadcastUnreadCountAsync(Guid userId, int count, CancellationToken cancellationToken = default)
            => Task.CompletedTask;
    }

    private class FakeIdentityService : IIdentityService
    {
        public Task<(bool Succeeded, string[] Errors)> CreateUserAsync(string email, string password, string fullName, CancellationToken cancellationToken = default) => Task.FromResult((true, Array.Empty<string>()));
        public Task<(bool Succeeded, string[] Errors)> CreateUserWithRoleAsync(string email, string password, string fullName, string role, CancellationToken cancellationToken = default) => Task.FromResult((true, Array.Empty<string>()));
        public Task<(bool Succeeded, Guid? UserId, string[] Errors)> CreateUserWalkInPatientAsync(string email, string password, string fullName, string role, Guid? organizationId = null, UserProfileWalkInDto? userProfile = null, CancellationToken cancellationToken = default) => Task.FromResult((true, (Guid?)Guid.NewGuid(), Array.Empty<string>()));
        public Task<bool> CheckPasswordAsync(Guid userId, string password) => Task.FromResult(false);
        public Task<UserDto?> GetUserByEmailAsync(string email, CancellationToken cancellationToken = default) => Task.FromResult<UserDto?>(null);
        public Task<UserDto?> GetUserByCitizenIdAsync(string citizenId, CancellationToken cancellationToken = default) => Task.FromResult<UserDto?>(null);
        public Task<UserDto?> GetUserByIdAsync(Guid userId, CancellationToken cancellationToken = default) => Task.FromResult<UserDto?>(null);
        public Task<bool> IsPhoneNumberInUseByOrganizationAsync(Guid organizationId, string phoneNumber, CancellationToken cancellationToken = default) => Task.FromResult(false);
        public Task<bool> IsCitizenIdInUseByOrganizationAsync(Guid organizationId, string citizenId, CancellationToken cancellationToken = default) => Task.FromResult(false);
        public Task<bool> IsEmailConfirmedAsync(Guid userId) => Task.FromResult(false);
        public Task<bool> IsUserActiveAsync(Guid userId) => Task.FromResult(false);
        public Task<string> GenerateEmailConfirmationTokenAsync(Guid userId) => Task.FromResult("token");
        public Task<(bool Succeeded, string[] Errors)> ConfirmEmailAsync(Guid userId, string token) => Task.FromResult((true, Array.Empty<string>()));
        public Task<string> GeneratePasswordResetTokenAsync(Guid userId) => Task.FromResult("reset");
        public Task<(bool Succeeded, string[] Errors)> ResetPasswordAsync(Guid userId, string token, string newPassword) => Task.FromResult((true, Array.Empty<string>()));
        public Task<IList<string>> GetUserRolesAsync(Guid userId) => Task.FromResult<IList<string>>(new List<string>());
        public Task<(bool Succeeded, string[] Errors)> AddToRoleAsync(Guid userId, string role) => Task.FromResult((true, Array.Empty<string>()));
        public Task<bool> IsInRoleAsync(Guid userId, string role) => Task.FromResult(false);
        public Task<IReadOnlyList<Guid>> GetUserIdsByRoleAndOrganizationAsync(string role, Guid organizationId, CancellationToken cancellationToken = default) => Task.FromResult<IReadOnlyList<Guid>>(new List<Guid>());
        public Task UpdateLastLoginAsync(Guid userId) => Task.CompletedTask;
        public Task<(bool Succeeded, string[] Errors)> DeactivateUserAsync(Guid userId) => Task.FromResult((true, Array.Empty<string>()));
        public Task<(bool Succeeded, string[] Errors)> SoftDeleteUserAsync(Guid userId) => Task.FromResult((true, Array.Empty<string>()));
        public Task<bool> IsTwoFactorEnabledAsync(Guid userId) => Task.FromResult(false);
        public Task<string?> GetAuthenticatorKeyAsync(Guid userId) => Task.FromResult<string?>("key");
        public Task<string> GetOrCreateAuthenticatorKeyAsync(Guid userId) => Task.FromResult("key");
        public string GenerateAuthenticatorUri(string email, string sharedKey) => "";
        public string FormatAuthenticatorKey(string key) => key;
        public Task<(bool Succeeded, string[] Errors, string[]? RecoveryCodes)> EnableTwoFactorAsync(Guid userId, string verificationCode) => Task.FromResult((true, Array.Empty<string>(), Array.Empty<string>()));
        public Task<(bool Succeeded, string[] Errors)> DisableTwoFactorAsync(Guid userId) => Task.FromResult((true, Array.Empty<string>()));
        public Task<bool> VerifyTwoFactorCodeAsync(Guid userId, string code) => Task.FromResult(false);
        public Task<(bool Succeeded, string[] Errors)> VerifyRecoveryCodeAsync(Guid userId, string recoveryCode) => Task.FromResult((true, Array.Empty<string>()));
        public Task<string[]> GenerateNewRecoveryCodesAsync(Guid userId, int count = 10) => Task.FromResult(Array.Empty<string>());
        public Task<int> GetRecoveryCodesCountAsync(Guid userId) => Task.FromResult(0);
        public Task<(List<UserAdminDto> Users, int TotalCount)> GetUsersAsync(string? searchTerm = null, string? roleFilter = null, string? statusFilter = null, int pageNumber = 1, int pageSize = 10, CancellationToken cancellationToken = default) => Task.FromResult((new List<UserAdminDto>(), 0));
        public Task<UserMetricsDto> GetUserMetricsAsync(CancellationToken cancellationToken = default) => Task.FromResult(new UserMetricsDto(0, 0, 0, 0, 0, 0, 0));
        public Task<int> GetUsersInRoleCountAsync(string role, bool activeOnly = true, CancellationToken cancellationToken = default) => Task.FromResult(0);
        public Task<(bool Succeeded, string[] Errors)> RemoveFromRoleAsync(Guid userId, string role) => Task.FromResult((true, Array.Empty<string>()));
        public Task<(bool Succeeded, string[] Errors)> ActivateUserAsync(Guid userId) => Task.FromResult((true, Array.Empty<string>()));
        public Task<(bool Succeeded, string[] Errors)> ApproveUserAsync(Guid userId) => Task.FromResult((true, Array.Empty<string>()));
        public Task<int> GetPendingApprovalsCountAsync(CancellationToken cancellationToken = default) => Task.FromResult(0);
        public Task<UserDetailsDto?> GetUserDetailsAsync(Guid userId, CancellationToken cancellationToken = default) => Task.FromResult<UserDetailsDto?>(null);
        public Task<(bool Succeeded, string[] Errors)> UpdateUserProfileAsync(Guid userId, string fullName, string? phone, DateTime? dateOfBirth, int? gender, string? address, CancellationToken cancellationToken = default) => Task.FromResult((true, Array.Empty<string>()));
        public Task<(bool Succeeded, string[] Errors)> UpdateAvatarUrlAsync(Guid userId, string avatarUrl, CancellationToken cancellationToken = default) => Task.FromResult((true, Array.Empty<string>()));
        public Task<(bool Succeeded, string[] Errors)> UpdateUserOrganizationAsync(Guid userId, Guid? organizationId, CancellationToken cancellationToken = default) => Task.FromResult((true, Array.Empty<string>()));
        public Task<(bool Succeeded, string[] Errors)> ChangePasswordAsync(Guid userId, string currentPassword, string newPassword, CancellationToken cancellationToken = default) => Task.FromResult((true, Array.Empty<string>()));
        public Task<(bool Succeeded, string[] Errors)> UpdateUserEmailAsync(Guid userId, string email, CancellationToken cancellationToken = default) => Task.FromResult((true, Array.Empty<string>()));
        public Task<IList<string>> GetUserPermissionsAsync(Guid userId) => Task.FromResult<IList<string>>(new List<string>());
        public Task SynchronizeRolesWithDefaultsAsync(CancellationToken cancellationToken = default) => Task.CompletedTask;

        public Task<IReadOnlyList<UserDto>> GetUsersByIdsAsync(IEnumerable<Guid> userIds, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Task<(bool Succeeded, string[] Errors)> UpdateUserProfileAsync(Guid userId, string fullName, string? phone, DateTime? dateOfBirth, int? gender, string? address, string? citizenId, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Task<(bool Succeeded, string[] Errors)> SetStaffOnboardingStatusAsync(Guid userId)
        {
            throw new NotImplementedException();
        }

        public Task<(bool Succeeded, string[] Errors)> ClearMustUpdateProfileFlagAsync(Guid userId)
        {
            return Task.FromResult((true, Array.Empty<string>()));
        }
    }
}
