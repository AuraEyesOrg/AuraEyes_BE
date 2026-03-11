using Application.Common.Interfaces;
using Application.Common.Models;
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

    #endregion
}
