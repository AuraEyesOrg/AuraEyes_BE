using Application.Common.Interfaces;
using Application.Common.Models;
using Application.Common.Models.Auth;
using Application.Scheduling.ScheduleTemplates.Interfaces;
using Domain.Common;
using Domain.Entities.Users;
using Domain.Repositories;
using Domain.Entities;
using FluentAssertions;
using Infrastructure.Identity;
using Infrastructure.Settings;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using System.Linq.Expressions;

namespace Infrastructure.UnitTests.Identity;

public class AuthServiceTests
{
    private readonly Mock<IIdentityService> _identityServiceMock;
    private readonly Mock<ITokenService> _tokenServiceMock;
    private readonly Mock<IRefreshTokenService> _refreshTokenServiceMock;
    private readonly Mock<IEmailService> _emailServiceMock;
    private readonly Mock<IOrganisationOnboardingService> _orgOnboardingMock;
    private readonly Mock<INotificationService> _notificationServiceMock;
    private readonly Mock<IFileStorageService> _fileStorageMock;
    private readonly Mock<IRepository<Patient>> _patientRepositoryMock;
    private readonly Mock<IRepository<Ophthalmologist>> _ophthalmologistRepositoryMock;
    private readonly Mock<IClinicStaffRepository> _clinicStaffRepositoryMock;
    private readonly Mock<IContractRepository> _contractRepositoryMock;
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly Mock<UserManager<ApplicationUser>> _userManagerMock;
    private readonly Mock<SignInManager<ApplicationUser>> _signInManagerMock;
    private readonly Mock<ILogger<AuthService>> _loggerMock;
    private readonly IOptions<GoogleAuthSettings> _googleAuthSettings;
    private readonly AuthService _authService;

    public AuthServiceTests()
    {
        _identityServiceMock = new Mock<IIdentityService>();
        _tokenServiceMock = new Mock<ITokenService>();
        _refreshTokenServiceMock = new Mock<IRefreshTokenService>();
        _emailServiceMock = new Mock<IEmailService>();
        _orgOnboardingMock = new Mock<IOrganisationOnboardingService>();
        _notificationServiceMock = new Mock<INotificationService>();
        _fileStorageMock = new Mock<IFileStorageService>();
        _patientRepositoryMock = new Mock<IRepository<Patient>>();
        _ophthalmologistRepositoryMock = new Mock<IRepository<Ophthalmologist>>();
        _clinicStaffRepositoryMock = new Mock<IClinicStaffRepository>();
        _contractRepositoryMock = new Mock<IContractRepository>();
        _unitOfWorkMock = new Mock<IUnitOfWork>();

        var userStoreMock = new Mock<IUserStore<ApplicationUser>>();
        _userManagerMock = new Mock<UserManager<ApplicationUser>>(userStoreMock.Object, null, null, null, null, null, null, null, null);
        
        var contextAccessorMock = new Mock<IHttpContextAccessor>();
        var userClaimsPrincipalFactoryMock = new Mock<IUserClaimsPrincipalFactory<ApplicationUser>>();
        _signInManagerMock = new Mock<SignInManager<ApplicationUser>>(
            _userManagerMock.Object, contextAccessorMock.Object, userClaimsPrincipalFactoryMock.Object, null, null, null, null);

        _googleAuthSettings = Options.Create(new GoogleAuthSettings());
        _loggerMock = new Mock<ILogger<AuthService>>();

        _authService = new AuthService(
            _identityServiceMock.Object,
            _tokenServiceMock.Object,
            _refreshTokenServiceMock.Object,
            _emailServiceMock.Object,
            _orgOnboardingMock.Object,
            _notificationServiceMock.Object,
            _fileStorageMock.Object,
            _patientRepositoryMock.Object,
            _ophthalmologistRepositoryMock.Object,
            _clinicStaffRepositoryMock.Object,
            _contractRepositoryMock.Object,
            _unitOfWorkMock.Object,
            _userManagerMock.Object,
            _signInManagerMock.Object,
            _googleAuthSettings,
            _loggerMock.Object);
    }

    [Fact]
    public async Task LoginAsync_WithValidCredentials_ShouldReturnTokens()
    {
        var request = new LoginRequest { Email = "test@test.com", Password = "Password123!" };
        var user = new ApplicationUser { Id = Guid.NewGuid(), Email = request.Email, FullName = "Test User" };

        _userManagerMock.Setup(x => x.FindByEmailAsync(request.Email)).ReturnsAsync(user);
        _userManagerMock.Setup(x => x.CheckPasswordAsync(user, request.Password)).ReturnsAsync(true);
        _signInManagerMock.Setup(x => x.CheckPasswordSignInAsync(user, request.Password, true))
            .ReturnsAsync(Microsoft.AspNetCore.Identity.SignInResult.Success);
        _userManagerMock.Setup(x => x.GetRolesAsync(user)).ReturnsAsync(new List<string> { "Patient" });
        _identityServiceMock.Setup(x => x.IsUserActiveAsync(user.Id)).ReturnsAsync(true);
        _identityServiceMock.Setup(x => x.GetUserRolesAsync(user.Id)).ReturnsAsync(new List<string> { "Patient" });
        _identityServiceMock.Setup(x => x.GetUserPermissionsAsync(user.Id)).ReturnsAsync(new List<string>());
        _patientRepositoryMock.Setup(x => x.FindAsync(It.IsAny<Expression<Func<Patient, bool>>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<Patient>());
        _refreshTokenServiceMock.Setup(x => x.CreateRefreshTokenAsync(It.IsAny<Guid>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Guid.NewGuid());
        _identityServiceMock.Setup(x => x.UpdateLastLoginAsync(user.Id)).Returns(Task.CompletedTask);
        
        _tokenServiceMock.Setup(x => x.GenerateAccessTokenAsync(user.Id, user.Email, user.FullName, It.IsAny<IEnumerable<string>>(), It.IsAny<IEnumerable<System.Security.Claims.Claim>>()))
            .ReturnsAsync(new TokenResult("access-token", "jti", DateTime.UtcNow.AddHours(1)));
        _tokenServiceMock.Setup(x => x.GenerateRefreshToken()).Returns("refresh-token");

        var result = await _authService.LoginAsync(request, "127.0.0.1");

        result.IsSuccess.Should().BeTrue();
        result.Data.AuthResponse!.AccessToken.Should().Be("access-token");
        result.Data.AuthResponse!.RefreshToken.Should().Be("refresh-token");
    }

    [Fact]
    public async Task RegisterPatientAsync_WhenUserExists_ShouldReturnFailure()
    {
        _identityServiceMock.Setup(x => x.GetUserByEmailAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new UserDto(Guid.NewGuid(), "test@test.com", "Name", true, true, false, null));

        var request = new RegisterPatientRequest
        {
            Email = "test@test.com",
            Password = "Password123!",
            ConfirmPassword = "Password123!",
            FullName = "Test User",
            PhoneNumber = "0123456789",
            CitizenId = "123456789",
            DateOfBirth = DateTime.UtcNow.AddYears(-20),
            Gender = 1,
            Address = "Test Address"
        };

        var result = await _authService.RegisterPatientAsync(request, "http://localhost/confirm");

        result.IsSuccess.Should().BeFalse();
        result.ErrorMessage.Should().Contain("exists");
    }
}
