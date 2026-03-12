using Application.Common.Interfaces;
using Application.Common.Models;
using Application.ConsultationSessions.Commands.CreateVerificationSession;
using Domain.Common;
using Domain.Entities.Consultation;
using Domain.Enums;
using Domain.Repositories;
using FluentAssertions;
using NSubstitute;

namespace Application.UnitTests.Handlers;

public class CreateVerificationSessionCommandHandlerTests
{
    private readonly IConsultationSessionRepository _sessionRepository;
    private readonly INotificationService _notificationService;
    private readonly IUnitOfWork _unitOfWork;
    private readonly CreateVerificationSessionCommandHandler _handler;

    public CreateVerificationSessionCommandHandlerTests()
    {
        _sessionRepository = Substitute.For<IConsultationSessionRepository>();
        _notificationService = Substitute.For<INotificationService>();
        _unitOfWork = Substitute.For<IUnitOfWork>();
        _handler = new CreateVerificationSessionCommandHandler(
            _sessionRepository, 
            _notificationService, 
            _unitOfWork);
    }

    [Fact]
    public async Task Handle_ValidCommand_ShouldCreateSessionAndReturnId()
    {
        // Arrange
        var command = new CreateVerificationSessionCommand
        {
            PatientId = Guid.NewGuid(),
            AiScreeningId = Guid.NewGuid(),
            Price = 50m,
            OphthalmologistId = Guid.NewGuid()
        };

        _sessionRepository.AddAsync(Arg.Any<ConsultationSession>(), Arg.Any<CancellationToken>())
            .Returns(callInfo => callInfo.Arg<ConsultationSession>());
        _unitOfWork.SaveChangesAsync(Arg.Any<CancellationToken>())
            .Returns(1);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Data.Should().NotBeEmpty();

        await _sessionRepository.Received(1)
            .AddAsync(Arg.Is<ConsultationSession>(s =>
                s.PatientId == command.PatientId &&
                s.AiScreeningId == command.AiScreeningId &&
                s.Price == command.Price &&
                s.Type == ConsultationSessionType.Verification &&
                s.Status == SessionStatus.Pending),
            Arg.Any<CancellationToken>());

        await _unitOfWork.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_WithoutDoctor_ShouldCreateSessionWithNullDoctor()
    {
        // Arrange
        var command = new CreateVerificationSessionCommand
        {
            PatientId = Guid.NewGuid(),
            AiScreeningId = Guid.NewGuid(),
            Price = 0m,
            OphthalmologistId = null
        };

        _sessionRepository.AddAsync(Arg.Any<ConsultationSession>(), Arg.Any<CancellationToken>())
            .Returns(callInfo => callInfo.Arg<ConsultationSession>());
        _unitOfWork.SaveChangesAsync(Arg.Any<CancellationToken>())
            .Returns(1);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();

        await _sessionRepository.Received(1)
            .AddAsync(Arg.Is<ConsultationSession>(s =>
                s.OphthalmologistId == null),
            Arg.Any<CancellationToken>());
    }
}
