using Application.Common.Interfaces;
using Application.OrganisationPatients.Commands.CreateWalkInPatient;
using Domain.Common;
using Domain.Entities.Users;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;

namespace Infrastructure.UnitTests.Application;

public class CreateWalkInPatientCommandHandlerTests
{
    private readonly Mock<IRepository<Patient>> _patientRepository = new();
    private readonly Mock<IIdentityService> _identityService = new();
    private readonly Mock<IEmailService> _emailService = new();
    private readonly Mock<ICurrentUserService> _currentUserService = new();
    private readonly Mock<IUnitOfWork> _unitOfWork = new();
    private readonly Mock<ILogger<CreateWalkInPatientCommandHandler>> _logger = new();

    [Fact]
    public async Task Handle_ShouldFail_WhenCitizenIdAlreadyExists()
    {
        _currentUserService.SetupGet(x => x.UserId).Returns(Guid.NewGuid());
        _identityService
            .Setup(x => x.GetUserByCitizenIdAsync("001099000000", It.IsAny<CancellationToken>()))
            .ReturnsAsync(new UserDto(Guid.NewGuid(), "existing@aura.local", "Existing", true, true, false, null));

        var handler = CreateHandler();

        var result = await handler.Handle(CreateCommand(citizenId: "001099000000"), CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Errors.Should().ContainSingle(e => e.Contains("Citizen ID already linked"));
        _unitOfWork.Verify(x => x.BeginTransactionAsync(It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task Handle_ShouldGenerateInternalEmail_WhenEmailNotProvided()
    {
        _currentUserService.SetupGet(x => x.UserId).Returns(Guid.NewGuid());
        _identityService
            .Setup(x => x.GetUserByCitizenIdAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((UserDto?)null);
        _identityService
            .Setup(x => x.GetUserByEmailAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((UserDto?)null);

        var createdUserId = Guid.NewGuid();
        _identityService
            .Setup(x => x.CreateUserWalkInPatientAsync(
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<Guid?>(),
                It.IsAny<UserProfileWalkInDto?>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync((true, createdUserId, Array.Empty<string>()));

        _identityService
            .Setup(x => x.SetStaffOnboardingStatusAsync(createdUserId))
            .ReturnsAsync((true, Array.Empty<string>()));

        _patientRepository
            .Setup(x => x.AddAsync(It.IsAny<Patient>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Patient p, CancellationToken _) => p);

        var handler = CreateHandler();

        var result = await handler.Handle(CreateCommand(email: null), CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Data.Should().NotBeNull();
        result.Data!.IsGeneratedEmail.Should().BeTrue();
        result.Data.LoginEmail.Should().EndWith("@patient.aura.local");
        result.Data.TemporaryPassword.Should().NotBeNullOrWhiteSpace();
        result.Data.EmailSent.Should().BeFalse();
        _emailService.Verify(
            x => x.SendAsync(
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<bool>(),
                It.IsAny<CancellationToken>()),
            Times.Never);
    }

    [Fact]
    public async Task Handle_ShouldRollback_WhenUserCreationFails()
    {
        _currentUserService.SetupGet(x => x.UserId).Returns(Guid.NewGuid());
        _identityService
            .Setup(x => x.GetUserByCitizenIdAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((UserDto?)null);

        _identityService
            .Setup(x => x.CreateUserWalkInPatientAsync(
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<Guid?>(),
                It.IsAny<UserProfileWalkInDto?>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync((false, (Guid?)null, ["create user failed"]));

        var handler = CreateHandler();

        var result = await handler.Handle(CreateCommand(email: "patient@example.com"), CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Errors.Should().Contain("create user failed");
        _unitOfWork.Verify(x => x.BeginTransactionAsync(It.IsAny<CancellationToken>()), Times.Once);
        _unitOfWork.Verify(x => x.RollbackTransactionAsync(It.IsAny<CancellationToken>()), Times.Once);
        _unitOfWork.Verify(x => x.CommitTransactionAsync(It.IsAny<CancellationToken>()), Times.Never);
    }

    private CreateWalkInPatientCommandHandler CreateHandler()
    {
        return new CreateWalkInPatientCommandHandler(
            _patientRepository.Object,
            _identityService.Object,
            _emailService.Object,
            _currentUserService.Object,
            _unitOfWork.Object,
            _logger.Object);
    }

    private static CreateWalkInPatientCommand CreateCommand(string? email = "patient@example.com", string citizenId = "001099000000")
    {
        return new CreateWalkInPatientCommand
        {
            FullName = "Nguyen Van A",
            DateOfBirth = new DateTime(1990, 5, 10),
            Gender = "Male",
            Address = "HCM City",
            PhoneNumber = "0909000000",
            CitizenId = citizenId,
            Email = email
        };
    }
}
