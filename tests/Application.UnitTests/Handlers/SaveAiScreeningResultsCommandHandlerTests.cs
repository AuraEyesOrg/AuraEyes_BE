using Application.Common.Interfaces;
using Application.Common.Models;
using Application.Screenings.Commands.SaveAiScreeningResults;
using Domain.Common;
using Domain.Entities.Screening;
using Domain.Entities.Users;
using Domain.Enums;
using Application.UnitTests.TestSupport;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using NSubstitute;

namespace Application.UnitTests.Handlers;

public class SaveAiScreeningResultsCommandHandlerTests
{
    private readonly IRepository<AiScreening> _screeningRepository;
    private readonly IRepository<Patient> _patientRepository;
    private readonly INotificationService _notificationService;
    private readonly IIdentityService _identityService;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IAiScreeningQuery _query;
    private readonly SaveAiScreeningResultsCommandHandler _handler;

    public SaveAiScreeningResultsCommandHandlerTests()
    {
        _screeningRepository = Substitute.For<IRepository<AiScreening>>();
        _patientRepository = Substitute.For<IRepository<Patient>>();
        _notificationService = Substitute.For<INotificationService>();
        _identityService = Substitute.For<IIdentityService>();
        _unitOfWork = Substitute.For<IUnitOfWork>();
        _query = Substitute.For<IAiScreeningQuery>();
        _notificationService
            .SendAsync(
                Arg.Any<Guid>(),
                Arg.Any<string>(),
                Arg.Any<string>(),
                Arg.Any<NotificationType>(),
                Arg.Any<object?>(),
                Arg.Any<CancellationToken>(),
                Arg.Any<Guid?>())
            .Returns(Task.CompletedTask);
        _identityService
            .GetUserIdsByRoleAndOrganizationAsync(
                Arg.Any<string>(),
                Arg.Any<Guid>(),
                Arg.Any<CancellationToken>())
            .Returns(Array.Empty<Guid>());
        _handler = new SaveAiScreeningResultsCommandHandler(
            _screeningRepository,
            _patientRepository,
            _notificationService,
            _identityService,
            _unitOfWork,
            _query,
            Substitute.For<ILogger<SaveAiScreeningResultsCommandHandler>>());
    }

    [Theory]
    [InlineData(-1)]
    [InlineData(101)]
    public async Task Handle_InvalidConfidenceScore_ShouldReturnFailure(decimal score)
    {
        var cmd = new SaveAiScreeningResultsCommand
        {
            ScreeningId = Guid.NewGuid(),
            ConfidenceScore = score,
            RiskLevel = RiskLevel.Low,
            RawJsonOutput = "{}"
        };

        var result = await _handler.Handle(cmd, CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Errors.Should().Contain(e => e.Contains("Confidence", StringComparison.OrdinalIgnoreCase));
    }

    [Fact]
    public async Task Handle_MissingScreening_ShouldReturnNotFound()
    {
        var empty = Array.Empty<AiScreening>().AsAsyncQueryable();
        _screeningRepository.Query().Returns(empty);

        var result = await _handler.Handle(
            new SaveAiScreeningResultsCommand
            {
                ScreeningId = Guid.NewGuid(),
                ConfidenceScore = 50,
                RiskLevel = RiskLevel.Low,
                RawJsonOutput = "{}"
            },
            CancellationToken.None);

        result.IsNotFound.Should().BeTrue();
    }

    [Fact]
    public async Task Handle_MissingPatient_ShouldReturnNotFound()
    {
        var patient = new Patient(Guid.NewGuid());
        var screening = new AiScreening(patient.Id, "v1");
        screening.RecordConsent(patient.Id, "agree");
        var mock = new[] { screening }.AsAsyncQueryable();
        _screeningRepository.Query().Returns(mock);
        _patientRepository.GetByIdAsync(patient.Id, Arg.Any<CancellationToken>()).Returns((Patient?)null);

        var result = await _handler.Handle(
            new SaveAiScreeningResultsCommand
            {
                ScreeningId = screening.Id,
                ConfidenceScore = 50,
                RiskLevel = RiskLevel.Low,
                RawJsonOutput = "{}"
            },
            CancellationToken.None);

        result.IsNotFound.Should().BeTrue();
    }

    [Fact]
    public async Task Handle_MissingConsent_ShouldReturnFailure()
    {
        var patient = new Patient(Guid.NewGuid());
        var screening = new AiScreening(patient.Id, "v1");
        var mock = new[] { screening }.AsAsyncQueryable();
        _screeningRepository.Query().Returns(mock);
        _patientRepository.GetByIdAsync(patient.Id, Arg.Any<CancellationToken>()).Returns(patient);

        var result = await _handler.Handle(
            new SaveAiScreeningResultsCommand
            {
                ScreeningId = screening.Id,
                ConfidenceScore = 50,
                RiskLevel = RiskLevel.Low,
                RawJsonOutput = "{}"
            },
            CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Errors.Should().Contain(e => e.Contains("consent", StringComparison.OrdinalIgnoreCase));
    }

    [Fact]
    public async Task Handle_ValidConsentAndResults_ShouldSucceed()
    {
        var patient = new Patient(Guid.NewGuid());
        var screening = new AiScreening(patient.Id, "v1");
        screening.RecordConsent(patient.Id, "agree");
        var mock = new[] { screening }.AsAsyncQueryable();
        _screeningRepository.Query().Returns(mock);
        _patientRepository.GetByIdAsync(patient.Id, Arg.Any<CancellationToken>()).Returns(patient);
        _query.CountRetinalImagesForScreeningAsync(screening.Id, Arg.Any<CancellationToken>()).Returns(2);

        var result = await _handler.Handle(
            new SaveAiScreeningResultsCommand
            {
                ScreeningId = screening.Id,
                ConfidenceScore = 88,
                RiskLevel = RiskLevel.Moderate,
                RawJsonOutput = "{\"ok\":true}",
                Summary = "S",
                Findings = "F"
            },
            CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Data!.ImagesCount.Should().Be(2);
        await _unitOfWork.Received(1).BeginTransactionAsync(Arg.Any<CancellationToken>());
        await _unitOfWork.Received(1).CommitTransactionAsync(Arg.Any<CancellationToken>());
        await _screeningRepository.Received(1).UpdateAsync(screening, Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_TransactionFailure_ShouldReturnFailure()
    {
        var patient = new Patient(Guid.NewGuid());
        var screening = new AiScreening(patient.Id, "v1");
        screening.RecordConsent(patient.Id, "agree");
        var mock = new[] { screening }.AsAsyncQueryable();
        _screeningRepository.Query().Returns(mock);
        _patientRepository.GetByIdAsync(patient.Id, Arg.Any<CancellationToken>()).Returns(patient);
        _query.CountRetinalImagesForScreeningAsync(screening.Id, Arg.Any<CancellationToken>()).Returns(0);
        _unitOfWork.SaveChangesAsync(Arg.Any<CancellationToken>())
            .Returns<Task>(_ => throw new InvalidOperationException("tx fail"));

        var result = await _handler.Handle(
            new SaveAiScreeningResultsCommand
            {
                ScreeningId = screening.Id,
                ConfidenceScore = 50,
                RiskLevel = RiskLevel.Low,
                RawJsonOutput = "{}"
            },
            CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Errors.Should().Contain(e => e.Contains("Failed to save", StringComparison.OrdinalIgnoreCase));
        await _unitOfWork.Received(1).RollbackTransactionAsync(Arg.Any<CancellationToken>());
    }
}
