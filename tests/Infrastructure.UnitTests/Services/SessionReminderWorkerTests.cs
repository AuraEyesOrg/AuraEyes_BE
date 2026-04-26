using Application.Common.Interfaces;
using Domain.Entities.Consultation;
using Domain.Repositories;
using Domain.Common;
using Infrastructure.Services;
using Infrastructure.UnitTests.Common;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Moq;

namespace Infrastructure.UnitTests.Services;

public class SessionReminderWorkerTests
{
    private readonly Mock<IServiceScopeFactory> _scopeFactoryMock;
    private readonly Mock<IBetterStackHeartbeatService> _betterStackHeartbeatMock;
    private readonly Mock<ILogger<SessionReminderWorker>> _loggerMock;
    
    private readonly Mock<IServiceScope> _scopeMock;
    private readonly Mock<IServiceProvider> _serviceProviderMock;
    private readonly Mock<IConsultationSessionRepository> _sessionRepoMock;
    private readonly Mock<INotificationService> _notificationServiceMock;
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;

    public SessionReminderWorkerTests()
    {
        _scopeFactoryMock = new Mock<IServiceScopeFactory>();
        _betterStackHeartbeatMock = new Mock<IBetterStackHeartbeatService>();
        _loggerMock = new Mock<ILogger<SessionReminderWorker>>();

        _scopeMock = new Mock<IServiceScope>();
        _serviceProviderMock = new Mock<IServiceProvider>();
        _sessionRepoMock = new Mock<IConsultationSessionRepository>();
        _notificationServiceMock = new Mock<INotificationService>();
        _unitOfWorkMock = new Mock<IUnitOfWork>();

        _scopeFactoryMock.Setup(x => x.CreateScope()).Returns(_scopeMock.Object);
        _scopeMock.Setup(x => x.ServiceProvider).Returns(_serviceProviderMock.Object);
        
        _serviceProviderMock.Setup(x => x.GetService(typeof(IConsultationSessionRepository))).Returns(_sessionRepoMock.Object);
        _serviceProviderMock.Setup(x => x.GetService(typeof(INotificationService))).Returns(_notificationServiceMock.Object);
        _serviceProviderMock.Setup(x => x.GetService(typeof(IUnitOfWork))).Returns(_unitOfWorkMock.Object);
    }

    [Fact]
    public async Task CheckAndNotifyStaleSessionsAsync_WhenStaleSessionsExist_ShouldSendReminders()
    {
        var worker = new TestSessionReminderWorker(
            _scopeFactoryMock.Object, 
            _betterStackHeartbeatMock.Object, 
            _loggerMock.Object);

        var sessions = new List<ConsultationSession>();
        // We need to use Reflection to set Id and other properties since it's a private constructor or protected fields
        var session = (ConsultationSession)System.Runtime.Serialization.FormatterServices.GetUninitializedObject(typeof(ConsultationSession));
        typeof(ConsultationSession).GetProperty(nameof(session.Id))?.SetValue(session, Guid.NewGuid());
        typeof(ConsultationSession).GetProperty(nameof(session.PatientId))?.SetValue(session, Guid.NewGuid());
        typeof(ConsultationSession).GetProperty(nameof(session.OphthalmologistId))?.SetValue(session, Guid.NewGuid());
        typeof(ConsultationSession).GetProperty(nameof(session.LastActivityAt))?.SetValue(session, DateTime.UtcNow.AddDays(-5));
        sessions.Add(session);

        _sessionRepoMock.Setup(x => x.GetStaleSessions(It.IsAny<TimeSpan>(), It.IsAny<TimeSpan>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(sessions);

        await worker.PublicCheckAndNotifyStaleSessionsAsync(CancellationToken.None);

        _notificationServiceMock.Verify(x => x.SendAsync(It.IsAny<Guid>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<Domain.Enums.NotificationType>(), It.IsAny<object>(), It.IsAny<CancellationToken>(), It.IsAny<Guid?>()), Times.Once);
        _unitOfWorkMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    private class TestSessionReminderWorker : SessionReminderWorker
    {
        public TestSessionReminderWorker(IServiceScopeFactory scopeFactory, IBetterStackHeartbeatService heartbeat, ILogger<SessionReminderWorker> logger) 
            : base(scopeFactory, heartbeat, logger) { }

        public Task PublicCheckAndNotifyStaleSessionsAsync(CancellationToken ct)
        {
            var method = typeof(SessionReminderWorker).GetMethod("CheckAndNotifyStaleSessionsAsync", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic);
            return (Task)method.Invoke(this, new object[] { ct });
        }
    }
}
