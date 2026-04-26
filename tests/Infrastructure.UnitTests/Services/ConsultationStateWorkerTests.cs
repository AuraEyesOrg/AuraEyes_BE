using Application.Common.Interfaces;
using Application.Common.Models;
using Application.ConsultationSessions.Commands.EndSession;
using Domain.Common;
using Domain.Entities.Users;
using Domain.Entities.Consultation;
using Domain.Repositories;
using Infrastructure.Services;
using Infrastructure.Settings;
using Infrastructure.UnitTests.Common;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using Application.Common.Constants;

namespace Infrastructure.UnitTests.Services;

public class ConsultationStateWorkerTests
{
    private readonly Mock<IServiceScopeFactory> _scopeFactoryMock;
    private readonly Mock<IBetterStackHeartbeatService> _betterStackHeartbeatMock;
    private readonly Mock<ILogger<ConsultationStateWorker>> _loggerMock;
    private readonly IOptions<GoogleMeetSettings> _googleMeetSettings;
    
    private readonly Mock<IServiceScope> _scopeMock;
    private readonly Mock<IServiceProvider> _serviceProviderMock;
    private readonly Mock<IConsultationSessionRepository> _sessionRepoMock;
    private readonly Mock<IRepository<Patient>> _patientRepoMock;
    private readonly Mock<IRepository<Ophthalmologist>> _ophthalmologistRepoMock;
    private readonly Mock<IChatHubService> _chatHubServiceMock;
    private readonly Mock<ISender> _senderMock;
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;

    public ConsultationStateWorkerTests()
    {
        _scopeFactoryMock = new Mock<IServiceScopeFactory>();
        _betterStackHeartbeatMock = new Mock<IBetterStackHeartbeatService>();
        _loggerMock = new Mock<ILogger<ConsultationStateWorker>>();
        _googleMeetSettings = Options.Create(new GoogleMeetSettings { DefaultDurationMinutes = 30 });

        _scopeMock = new Mock<IServiceScope>();
        _serviceProviderMock = new Mock<IServiceProvider>();
        _sessionRepoMock = new Mock<IConsultationSessionRepository>();
        _patientRepoMock = new Mock<IRepository<Patient>>();
        _ophthalmologistRepoMock = new Mock<IRepository<Ophthalmologist>>();
        _chatHubServiceMock = new Mock<IChatHubService>();
        _senderMock = new Mock<ISender>();
        _unitOfWorkMock = new Mock<IUnitOfWork>();

        _scopeFactoryMock.Setup(x => x.CreateScope()).Returns(_scopeMock.Object);
        _scopeMock.Setup(x => x.ServiceProvider).Returns(_serviceProviderMock.Object);
        
        _serviceProviderMock.Setup(x => x.GetService(typeof(IConsultationSessionRepository))).Returns(_sessionRepoMock.Object);
        _serviceProviderMock.Setup(x => x.GetService(typeof(IRepository<Patient>))).Returns(_patientRepoMock.Object);
        _serviceProviderMock.Setup(x => x.GetService(typeof(IRepository<Ophthalmologist>))).Returns(_ophthalmologistRepoMock.Object);
        _serviceProviderMock.Setup(x => x.GetService(typeof(IChatHubService))).Returns(_chatHubServiceMock.Object);
        _serviceProviderMock.Setup(x => x.GetService(typeof(ISender))).Returns(_senderMock.Object);
        _serviceProviderMock.Setup(x => x.GetService(typeof(IUnitOfWork))).Returns(_unitOfWorkMock.Object);
    }

    [Fact]
    public async Task ProcessStateTransitionsAsync_WhenCalled_ShouldInvokeReposAndBroadcast()
    {
        var worker = new TestConsultationStateWorker(
            _scopeFactoryMock.Object, 
            _betterStackHeartbeatMock.Object, 
            _googleMeetSettings, 
            _loggerMock.Object);

        _sessionRepoMock.Setup(x => x.GetSessionsReadyToOpenAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<ConsultationSession>());
        _sessionRepoMock.Setup(x => x.GetSessionsPastGracePeriodAsync(It.IsAny<TimeSpan>(), It.IsAny<TimeSpan>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<ConsultationSession>());
        _sessionRepoMock.Setup(x => x.GetExpiredClinicSessionsAsync(It.IsAny<TimeSpan>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<ConsultationSession>());

        await worker.PublicProcessStateTransitionsAsync(CancellationToken.None);

        _sessionRepoMock.Verify(x => x.GetSessionsReadyToOpenAsync(It.IsAny<CancellationToken>()), Times.Once);
        _sessionRepoMock.Verify(x => x.GetSessionsPastGracePeriodAsync(It.IsAny<TimeSpan>(), It.IsAny<TimeSpan>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    private class TestConsultationStateWorker : ConsultationStateWorker
    {
        public TestConsultationStateWorker(IServiceScopeFactory scopeFactory, IBetterStackHeartbeatService heartbeat, IOptions<GoogleMeetSettings> settings, ILogger<ConsultationStateWorker> logger) 
            : base(scopeFactory, heartbeat, settings, logger) { }

        public Task PublicProcessStateTransitionsAsync(CancellationToken ct)
        {
            var method = typeof(ConsultationStateWorker).GetMethod("ProcessStateTransitionsAsync", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic);
            return (Task)method.Invoke(this, new object[] { ct });
        }
    }
}
