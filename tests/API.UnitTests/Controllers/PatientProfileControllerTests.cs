using Application.Common.Interfaces;
using Application.Common.Models;
using Application.Patients.Commands.ChangePassword;
using Application.Patients.Commands.UpdatePatientProfile;
using Application.Patients.Commands.UploadAvatar;
using Application.Patients.Common;
using Application.Patients.Queries.GetPatientProfile;
using API.Controllers;
using FluentAssertions;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using NSubstitute;

namespace API.UnitTests.Controllers;

public class PatientProfileControllerTests
{
    private readonly IMediator _mediator;
    private readonly ICurrentUserService _currentUserService;
    private readonly ILogger<PatientProfileController> _logger;
    private readonly PatientProfileController _controller;

    public PatientProfileControllerTests()
    {
        _mediator = Substitute.For<IMediator>();
        _currentUserService = Substitute.For<ICurrentUserService>();
        _logger = Substitute.For<ILogger<PatientProfileController>>();
        _controller = new PatientProfileController(_mediator, _currentUserService, _logger)
        {
            ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext()
            }
        };
    }

    #region GetProfile Tests

    [Fact]
    public async Task GetProfile_WhenUserNotAuthenticated_ShouldReturn401()
    {
        // Arrange
        _currentUserService.UserId.Returns((Guid?)null);

        // Act
        var result = await _controller.GetProfile(CancellationToken.None);

        // Assert
        result.Should().BeOfType<UnauthorizedObjectResult>();
    }

    [Fact]
    public async Task GetProfile_WhenAuthenticated_ShouldSendQuery()
    {
        // Arrange
        var userId = Guid.NewGuid();
        _currentUserService.UserId.Returns(userId);
        _mediator.Send(Arg.Any<GetPatientProfileQuery>(), Arg.Any<CancellationToken>())
            .Returns(Result<PatientProfileDto>.Success(new PatientProfileDto
            {
                Id = Guid.NewGuid(),
                FullName = "Test Patient",
                Email = "patient@test.com"
            }));

        // Act
        var result = await _controller.GetProfile(CancellationToken.None);

        // Assert
        result.Should().BeOfType<OkObjectResult>();
        await _mediator.Received(1).Send(
            Arg.Is<GetPatientProfileQuery>(q => q.UserId == userId),
            Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task GetProfile_WhenNotFound_ShouldReturn404()
    {
        // Arrange
        var userId = Guid.NewGuid();
        _currentUserService.UserId.Returns(userId);
        _mediator.Send(Arg.Any<GetPatientProfileQuery>(), Arg.Any<CancellationToken>())
            .Returns(Result<PatientProfileDto>.NotFound("Patient profile not found"));

        // Act
        var result = await _controller.GetProfile(CancellationToken.None);

        // Assert
        result.Should().BeOfType<NotFoundObjectResult>();
    }

    #endregion

    #region UpdateProfile Tests

    [Fact]
    public async Task UpdateProfile_WhenUserNotAuthenticated_ShouldReturn401()
    {
        // Arrange
        _currentUserService.UserId.Returns((Guid?)null);
        var request = new UpdateProfileRequest { FullName = "New Name" };

        // Act
        var result = await _controller.UpdateProfile(request, CancellationToken.None);

        // Assert
        result.Should().BeOfType<UnauthorizedObjectResult>();
    }

    [Fact]
    public async Task UpdateProfile_WhenAuthenticated_ShouldSendCommand()
    {
        // Arrange
        var userId = Guid.NewGuid();
        _currentUserService.UserId.Returns(userId);
        _mediator.Send(Arg.Any<UpdatePatientProfileCommand>(), Arg.Any<CancellationToken>())
            .Returns(Result<PatientProfileDto>.Success(new PatientProfileDto
            {
                Id = userId,
                Email = "patient@test.com",
                FullName = "Updated Name"
            }));

        var request = new UpdateProfileRequest
        {
            FullName = "Updated Name",
            Phone = "+84123456789",
            DateOfBirth = "1990-01-15",
            Gender = "male",
            Address = "123 Main St"
        };

        // Act
        var result = await _controller.UpdateProfile(request, CancellationToken.None);

        // Assert
        result.Should().BeOfType<OkObjectResult>();
        await _mediator.Received(1).Send(
            Arg.Is<UpdatePatientProfileCommand>(cmd =>
                cmd.UserId == userId &&
                cmd.FullName == request.FullName &&
                cmd.Phone == request.Phone &&
                cmd.DateOfBirth == request.DateOfBirth &&
                cmd.Gender == request.Gender &&
                cmd.Address == request.Address),
            Arg.Any<CancellationToken>());
    }

    #endregion

    #region UploadAvatar Tests

    [Fact]
    public async Task UploadAvatar_WhenUserNotAuthenticated_ShouldReturn401()
    {
        // Arrange
        _currentUserService.UserId.Returns((Guid?)null);
        var file = Substitute.For<IFormFile>();
        file.Length.Returns(1024);
        file.ContentType.Returns("image/jpeg");

        // Act
        var result = await _controller.UploadAvatar(file, CancellationToken.None);

        // Assert
        result.Should().BeOfType<UnauthorizedObjectResult>();
    }

    [Fact]
    public async Task UploadAvatar_WhenEmptyFile_ShouldReturn400()
    {
        // Arrange
        var userId = Guid.NewGuid();
        _currentUserService.UserId.Returns(userId);
        var file = Substitute.For<IFormFile>();
        file.Length.Returns(0);

        // Act
        var result = await _controller.UploadAvatar(file, CancellationToken.None);

        // Assert
        result.Should().BeOfType<BadRequestObjectResult>();
    }

    [Fact]
    public async Task UploadAvatar_WhenInvalidFileType_ShouldReturn400()
    {
        // Arrange
        var userId = Guid.NewGuid();
        _currentUserService.UserId.Returns(userId);
        var file = Substitute.For<IFormFile>();
        file.Length.Returns(1024);
        file.ContentType.Returns("application/pdf");

        // Act
        var result = await _controller.UploadAvatar(file, CancellationToken.None);

        // Assert
        result.Should().BeOfType<BadRequestObjectResult>();
    }

    [Fact]
    public async Task UploadAvatar_WhenFileTooLarge_ShouldReturn400()
    {
        // Arrange
        var userId = Guid.NewGuid();
        _currentUserService.UserId.Returns(userId);
        var file = Substitute.For<IFormFile>();
        file.Length.Returns(6 * 1024 * 1024); // 6MB - exceeds 5MB limit
        file.ContentType.Returns("image/jpeg");

        // Act
        var result = await _controller.UploadAvatar(file, CancellationToken.None);

        // Assert
        result.Should().BeOfType<BadRequestObjectResult>();
    }

    [Fact]
    public async Task UploadAvatar_WhenValidFile_ShouldSendCommand()
    {
        // Arrange
        var userId = Guid.NewGuid();
        _currentUserService.UserId.Returns(userId);
        var stream = new MemoryStream(new byte[] { 1, 2, 3 });
        var file = new FormFile(stream, 0, stream.Length, "avatar", "avatar.png")
        {
            Headers = new HeaderDictionary(),
            ContentType = "image/png"
        };

        _mediator.Send(Arg.Any<UploadAvatarCommand>(), Arg.Any<CancellationToken>())
            .Returns(Result<UploadAvatarResponse>.Success(new UploadAvatarResponse
            {
                AvatarUrl = "https://cdn.example/avatar.png"
            }));

        // Act
        var result = await _controller.UploadAvatar(file, CancellationToken.None);

        // Assert
        result.Should().BeOfType<OkObjectResult>();
        await _mediator.Received(1).Send(
            Arg.Is<UploadAvatarCommand>(cmd =>
                cmd.UserId == userId &&
                cmd.FileName == "avatar.png"),
            Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task ChangePassword_WhenUserNotAuthenticated_ShouldReturn401()
    {
        // Arrange
        _currentUserService.UserId.Returns((Guid?)null);
        var request = new ChangePasswordRequest
        {
            CurrentPassword = "Old123!",
            NewPassword = "New123!",
            ConfirmNewPassword = "New123!"
        };

        // Act
        var result = await _controller.ChangePassword(request, CancellationToken.None);

        // Assert
        result.Should().BeOfType<UnauthorizedObjectResult>();
    }

    [Fact]
    public async Task ChangePassword_WhenAuthenticated_ShouldSendCommand()
    {
        // Arrange
        var userId = Guid.NewGuid();
        _currentUserService.UserId.Returns(userId);
        _mediator.Send(Arg.Any<ChangePasswordCommand>(), Arg.Any<CancellationToken>())
            .Returns(Result.Success());

        var request = new ChangePasswordRequest
        {
            CurrentPassword = "Old123!",
            NewPassword = "New123!",
            ConfirmNewPassword = "New123!"
        };

        // Act
        var result = await _controller.ChangePassword(request, CancellationToken.None);

        // Assert
        result.Should().BeOfType<OkObjectResult>();
        await _mediator.Received(1).Send(
            Arg.Is<ChangePasswordCommand>(cmd =>
                cmd.UserId == userId &&
                cmd.CurrentPassword == request.CurrentPassword &&
                cmd.NewPassword == request.NewPassword &&
                cmd.ConfirmNewPassword == request.ConfirmNewPassword),
            Arg.Any<CancellationToken>());
    }

    #endregion
}
