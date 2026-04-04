using Application.Common.Interfaces;
using Application.Common.Models;
using Application.Screenings.Commands.CreateAiScreeningSession;
using Domain.Common;
using Domain.Entities.Screening;
using Domain.Entities.Users;
using Domain.Enums;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using NSubstitute;

namespace Application.UnitTests.Handlers;

public class CreateAiScreeningSessionCommandHandlerTests
{
    private readonly IRepository<AiScreening> _screeningRepository;
    private readonly IRepository<Patient> _patientRepository;
    private readonly ICurrentUserService _currentUser;
    private readonly IUnitOfWork _unitOfWork;
    private readonly CreateAiScreeningSessionCommandHandler _handler;

    public CreateAiScreeningSessionCommandHandlerTests()
    {
        _screeningRepository = Substitute.For<IRepository<AiScreening>>();
        _patientRepository = Substitute.For<IRepository<Patient>>();
        _currentUser = Substitute.For<ICurrentUserService>();
        _unitOfWork = Substitute.For<IUnitOfWork>();
        _handler = new CreateAiScreeningSessionCommandHandler(
            _screeningRepository,
            _patientRepository,
            _currentUser,
            _unitOfWork,
            Substitute.For<ILogger<CreateAiScreeningSessionCommandHandler>>());
    }

    [Fact]
    public async Task Handle_WhenUserNotAuthenticated_ShouldReturnUnauthorized()
    {
        _currentUser.UserId.Returns((Guid?)null);

        var result = await _handler.Handle(
            new CreateAiScreeningSessionCommand { RetinalImages = new List<RetinalImageData>() },
            CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.IsUnauthorized.Should().BeTrue();
    }

    [Fact]
    public async Task Handle_WhenPatientMissing_ShouldReturnNotFound()
    {
        var userId = Guid.NewGuid();
        _currentUser.UserId.Returns(userId);
        _patientRepository.FindAsync(Arg.Any<System.Linq.Expressions.Expression<Func<Patient, bool>>>(), Arg.Any<CancellationToken>())
            .Returns(Array.Empty<Patient>());

        var result = await _handler.Handle(
            new CreateAiScreeningSessionCommand { RetinalImages = new List<RetinalImageData>() },
            CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.IsNotFound.Should().BeTrue();
    }

    [Fact]
    public async Task Handle_ValidCommandWithoutImages_ShouldCreateSession()
    {
        var userId = Guid.NewGuid();
        var patient = new Patient(userId);
        _currentUser.UserId.Returns(userId);
        _patientRepository.FindAsync(Arg.Any<System.Linq.Expressions.Expression<Func<Patient, bool>>>(), Arg.Any<CancellationToken>())
            .Returns(new[] { patient });
        _screeningRepository.AddAsync(Arg.Any<AiScreening>(), Arg.Any<CancellationToken>())
            .Returns(call => call.Arg<AiScreening>());

        var result = await _handler.Handle(
            new CreateAiScreeningSessionCommand { ModelVersion = "CFP_v1", RetinalImages = new List<RetinalImageData>() },
            CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Data!.PatientId.Should().Be(patient.Id);
        await _screeningRepository.Received(1).AddAsync(Arg.Any<AiScreening>(), Arg.Any<CancellationToken>());
        await _unitOfWork.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_ValidCommandWithImages_ShouldAttachImages()
    {
        var userId = Guid.NewGuid();
        var patient = new Patient(userId);
        _currentUser.UserId.Returns(userId);
        _patientRepository.FindAsync(Arg.Any<System.Linq.Expressions.Expression<Func<Patient, bool>>>(), Arg.Any<CancellationToken>())
            .Returns(new[] { patient });
        AiScreening? captured = null;
        _screeningRepository.AddAsync(Arg.Do<AiScreening>(s => captured = s), Arg.Any<CancellationToken>())
            .Returns(call => call.Arg<AiScreening>());

        var result = await _handler.Handle(
            new CreateAiScreeningSessionCommand
            {
                RetinalImages = new List<RetinalImageData>
                {
                    new() { ImageUrl = "https://img/1.png", EyeSide = EyeSide.Left }
                }
            },
            CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Data!.Images.Should().HaveCount(1);
        captured!.RetinalImages.Should().HaveCount(1);
    }

    [Fact]
    public async Task Handle_WhenSaveChangesThrows_ShouldReturnFailure()
    {
        var userId = Guid.NewGuid();
        var patient = new Patient(userId);
        _currentUser.UserId.Returns(userId);
        _patientRepository.FindAsync(Arg.Any<System.Linq.Expressions.Expression<Func<Patient, bool>>>(), Arg.Any<CancellationToken>())
            .Returns(new[] { patient });
        _screeningRepository.AddAsync(Arg.Any<AiScreening>(), Arg.Any<CancellationToken>())
            .Returns(call => call.Arg<AiScreening>());
        _unitOfWork.SaveChangesAsync(Arg.Any<CancellationToken>())
            .Returns<Task>(_ => throw new InvalidOperationException("db down"));

        var result = await _handler.Handle(
            new CreateAiScreeningSessionCommand { RetinalImages = new List<RetinalImageData>() },
            CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Errors.Should().Contain(e => e.Contains("Failed to create", StringComparison.OrdinalIgnoreCase));
    }
}
