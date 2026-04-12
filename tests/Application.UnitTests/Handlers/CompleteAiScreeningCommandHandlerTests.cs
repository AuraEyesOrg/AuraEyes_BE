using Application.Common.Interfaces;
using Application.Common.Models;
using Application.Screenings.Commands.CompleteAiScreening;
using Domain.Common;
using Domain.Entities.Screening;
using Domain.Entities.Users;
using Domain.Enums;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using NSubstitute;

namespace Application.UnitTests.Handlers;

public class CompleteAiScreeningCommandHandlerTests
{
    private readonly IRepository<AiScreening> _screeningRepository;
    private readonly IRepository<Patient> _patientRepository;
    private readonly INotificationService _notificationService;
    private readonly IUnitOfWork _unitOfWork;
    private readonly CompleteAiScreeningCommandHandler _handler;

    public CompleteAiScreeningCommandHandlerTests()
    {
        _screeningRepository = Substitute.For<IRepository<AiScreening>>();
        _patientRepository = Substitute.For<IRepository<Patient>>();
        _notificationService = Substitute.For<INotificationService>();
        _unitOfWork = Substitute.For<IUnitOfWork>();
        _handler = new CompleteAiScreeningCommandHandler(
            _screeningRepository,
            _patientRepository,
            _notificationService,
            _unitOfWork,
            Substitute.For<ILogger<CompleteAiScreeningCommandHandler>>());
    }

    [Fact]
    public async Task Handle_MissingScreening_ShouldReturnNotFound()
    {
        _screeningRepository.GetByIdAsync(Arg.Any<Guid>(), Arg.Any<CancellationToken>()).Returns((AiScreening?)null);

        var result = await _handler.Handle(
            new CompleteAiScreeningCommand { ScreeningId = Guid.NewGuid(), RawJsonOutput = "{}", ResultStatus = "Normal" },
            CancellationToken.None);

        result.IsNotFound.Should().BeTrue();
    }

    [Fact]
    public async Task Handle_AlreadyProcessed_ShouldReturnFailure()
    {
        var patient = new Patient(Guid.NewGuid());
        var screening = new AiScreening(patient.Id, "v1");
        screening.Process("{}");
        _screeningRepository.GetByIdAsync(screening.Id, Arg.Any<CancellationToken>()).Returns(screening);

        var result = await _handler.Handle(
            new CompleteAiScreeningCommand { ScreeningId = screening.Id, RawJsonOutput = "{}", ResultStatus = "Normal" },
            CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Errors.Should().Contain(e => e.Contains("already been processed", StringComparison.OrdinalIgnoreCase));
    }

    [Fact]
    public async Task Handle_ValidWithPatient_ShouldNotify()
    {
        var userId = Guid.NewGuid();
        var patient = new Patient(userId);
        var screening = new AiScreening(patient.Id, "v1");
        _screeningRepository.GetByIdAsync(screening.Id, Arg.Any<CancellationToken>()).Returns(screening);
        _patientRepository.GetByIdAsync(patient.Id, Arg.Any<CancellationToken>()).Returns(patient);

        var result = await _handler.Handle(
            new CompleteAiScreeningCommand
            {
                ScreeningId = screening.Id,
                RawJsonOutput = "{\"x\":1}",
                ResultStatus = "Normal"
            },
            CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        screening.ProcessedAt.Should().NotBeNull();
        await _notificationService.Received(1).SendAsync(
            userId,
            Arg.Any<string>(),
            Arg.Any<string>(),
            NotificationType.AiScreeningCompleted,
            Arg.Any<object>(),
            Arg.Any<CancellationToken>(),
            Arg.Any<Guid?>());
    }

    [Fact]
    public async Task Handle_ValidWithoutPatient_ShouldSkipNotification()
    {
        var patient = new Patient(Guid.NewGuid());
        var screening = new AiScreening(patient.Id, "v1");
        _screeningRepository.GetByIdAsync(screening.Id, Arg.Any<CancellationToken>()).Returns(screening);
        _patientRepository.GetByIdAsync(patient.Id, Arg.Any<CancellationToken>()).Returns((Patient?)null);

        var result = await _handler.Handle(
            new CompleteAiScreeningCommand { ScreeningId = screening.Id, RawJsonOutput = "{}", ResultStatus = "Normal" },
            CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        await _notificationService.DidNotReceive().SendAsync(
            Arg.Any<Guid>(),
            Arg.Any<string>(),
            Arg.Any<string>(),
            Arg.Any<NotificationType>(),
            Arg.Any<object>(),
            Arg.Any<CancellationToken>(),
            Arg.Any<Guid?>());
    }

    [Fact]
    public async Task Handle_NormalStatus_ShouldUseNormalTitle()
    {
        var userId = Guid.NewGuid();
        var patient = new Patient(userId);
        var screening = new AiScreening(patient.Id, "v1");
        _screeningRepository.GetByIdAsync(screening.Id, Arg.Any<CancellationToken>()).Returns(screening);
        _patientRepository.GetByIdAsync(patient.Id, Arg.Any<CancellationToken>()).Returns(patient);
        string? title = null;
        await _notificationService.SendAsync(
            Arg.Any<Guid>(),
            Arg.Do<string>(t => title = t),
            Arg.Any<string>(),
            Arg.Any<NotificationType>(),
            Arg.Any<object>(),
            Arg.Any<CancellationToken>(),
            Arg.Any<Guid?>());

        await _handler.Handle(
            new CompleteAiScreeningCommand { ScreeningId = screening.Id, RawJsonOutput = "{}", ResultStatus = "Normal" },
            CancellationToken.None);

        title.Should().Contain("Bình thường");
    }
}
