using Application.Common.Interfaces;
using Application.Common.Models;
using Application.Common.Models.Auth;
using Domain.Common;
using Domain.Enums;
using FluentAssertions;
using Infrastructure.Identity;
using Infrastructure.Settings;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using NSubstitute;

namespace Infrastructure.UnitTests.Identity;

/// <summary>
/// Focused registration-path tests that avoid full SMTP / Google / token flows.
/// </summary>
public class AuthServiceRegistrationTests
{
    private static UserManager<ApplicationUser> CreateUserManager(IUserStore<ApplicationUser> store)
        => new(
            store,
            Options.Create(new IdentityOptions()),
            Substitute.For<IPasswordHasher<ApplicationUser>>(),
            Array.Empty<IUserValidator<ApplicationUser>>(),
            Array.Empty<IPasswordValidator<ApplicationUser>>(),
            new UpperInvariantLookupNormalizer(),
            new IdentityErrorDescriber(),
            Substitute.For<IServiceProvider>(),
            Substitute.For<ILogger<UserManager<ApplicationUser>>>());

    private static SignInManager<ApplicationUser> CreateSignInManager(UserManager<ApplicationUser> userManager)
        => new(
            userManager,
            Substitute.For<IHttpContextAccessor>(),
            Substitute.For<IUserClaimsPrincipalFactory<ApplicationUser>>(),
            Options.Create(new IdentityOptions()),
            Substitute.For<ILogger<SignInManager<ApplicationUser>>>(),
            Substitute.For<IAuthenticationSchemeProvider>(),
            Substitute.For<IUserConfirmation<ApplicationUser>>());

    private static AuthService CreateSut(
        IIdentityService identityService,
        UserManager<ApplicationUser>? userManager = null,
        SignInManager<ApplicationUser>? signInManager = null)
    {
        var store = Substitute.For<IUserStore<ApplicationUser>>();
        userManager ??= CreateUserManager(store);
        signInManager ??= CreateSignInManager(userManager);

        return new AuthService(
            identityService,
            Substitute.For<ITokenService>(),
            Substitute.For<IRefreshTokenService>(),
            Substitute.For<IEmailService>(),
            Substitute.For<IOrganisationOnboardingService>(),
            Substitute.For<INotificationService>(),
            Substitute.For<IFileStorageService>(),
            Substitute.For<IRepository<Domain.Entities.Users.Patient>>(),
            Substitute.For<IRepository<Domain.Entities.Users.Ophthalmologist>>(),
            Substitute.For<Domain.Repositories.IContractRepository>(),
            Substitute.For<Application.Scheduling.ScheduleTemplates.Interfaces.IFullTimeTemplateProvisioningService>(),
            Substitute.For<IUnitOfWork>(),
            userManager,
            signInManager,
            Options.Create(new GoogleAuthSettings()),
            Substitute.For<ILogger<AuthService>>());
    }

    [Fact]
    public async Task RegisterPatientAsync_WhenEmailExists_ShouldReturnFailure()
    {
        var identity = Substitute.For<IIdentityService>();
        identity.GetUserByEmailAsync("dup@test.com", Arg.Any<CancellationToken>())
            .Returns(new UserDto(Guid.NewGuid(), "dup@test.com", "Dup", true, true, false, null));

        var sut = CreateSut(identity);
        var req = new RegisterPatientRequest
        {
            Email = "dup@test.com",
            Password = "Password1!",
            ConfirmPassword = "Password1!",
            FullName = "Dup User"
        };

        var result = await sut.RegisterPatientAsync(req, "https://confirm.local");

        result.IsSuccess.Should().BeFalse();
        result.Errors.Should().Contain(e => e.Contains("already exists", StringComparison.OrdinalIgnoreCase));
    }

    [Fact]
    public async Task RegisterOphthalmologistAsync_WhenNoCredentials_ShouldReturnFailure()
    {
        var identity = Substitute.For<IIdentityService>();
        var sut = CreateSut(identity);
        var req = new RegisterOphthalmologistRequest
        {
            Email = "doc@test.com",
            Password = "Password1!",
            ConfirmPassword = "Password1!",
            FullName = "Doc",
            Certificates = new List<CredentialItemDto>()
        };

        var result = await sut.RegisterOphthalmologistAsync(req, "https://confirm.local");

        result.IsSuccess.Should().BeFalse();
        result.Errors.Should().Contain(e => e.Contains("credential", StringComparison.OrdinalIgnoreCase));
    }

    [Fact]
    public async Task RegisterOphthalmologistAsync_WhenMissingDegree_ShouldReturnFailure()
    {
        var identity = Substitute.For<IIdentityService>();
        var sut = CreateSut(identity);
        var file = Substitute.For<IFormFile>();
        file.Length.Returns(10);
        var req = new RegisterOphthalmologistRequest
        {
            Email = "doc@test.com",
            Password = "Password1!",
            ConfirmPassword = "Password1!",
            FullName = "Doc",
            Certificates = new List<CredentialItemDto>
            {
                new()
                {
                    Type = CertificateType.License,
                    Name = "Lic",
                    IssuingAuthority = "Gov",
                    IssuedDate = DateTime.UtcNow.AddYears(-1),
                    ExpiryDate = DateTime.UtcNow.AddYears(1),
                    File = file
                }
            }
        };

        var result = await sut.RegisterOphthalmologistAsync(req, "https://confirm.local");

        result.IsSuccess.Should().BeFalse();
        result.Errors.Should().Contain(e => e.Contains("degree", StringComparison.OrdinalIgnoreCase));
    }

    [Fact]
    public async Task RegisterOphthalmologistAsync_WhenMissingLicense_ShouldReturnFailure()
    {
        var identity = Substitute.For<IIdentityService>();
        var sut = CreateSut(identity);
        var file = Substitute.For<IFormFile>();
        file.Length.Returns(10);
        var req = new RegisterOphthalmologistRequest
        {
            Email = "doc@test.com",
            Password = "Password1!",
            ConfirmPassword = "Password1!",
            FullName = "Doc",
            Certificates = new List<CredentialItemDto>
            {
                new()
                {
                    Type = CertificateType.Degree,
                    DegreeLevel = DegreeLevel.Master,
                    Name = "MD",
                    IssuingAuthority = "Uni",
                    IssuedDate = DateTime.UtcNow.AddYears(-2),
                    File = file
                }
            }
        };

        var result = await sut.RegisterOphthalmologistAsync(req, "https://confirm.local");

        result.IsSuccess.Should().BeFalse();
        result.Errors.Should().Contain(e => e.Contains("license", StringComparison.OrdinalIgnoreCase));
    }

    [Fact]
    public async Task RegisterOrganisationAsync_ShouldDelegateToOnboardingService()
    {
        var identity = Substitute.For<IIdentityService>();
        var onboarding = Substitute.For<IOrganisationOnboardingService>();
        var store = Substitute.For<IUserStore<ApplicationUser>>();
        var userManager = CreateUserManager(store);
        var signInManager = CreateSignInManager(userManager);

        var sut = new AuthService(
            identity,
            Substitute.For<ITokenService>(),
            Substitute.For<IRefreshTokenService>(),
            Substitute.For<IEmailService>(),
            onboarding,
            Substitute.For<INotificationService>(),
            Substitute.For<IFileStorageService>(),
            Substitute.For<IRepository<Domain.Entities.Users.Patient>>(),
            Substitute.For<IRepository<Domain.Entities.Users.Ophthalmologist>>(),
            Substitute.For<Domain.Repositories.IContractRepository>(),
            Substitute.For<Application.Scheduling.ScheduleTemplates.Interfaces.IFullTimeTemplateProvisioningService>(),
            Substitute.For<IUnitOfWork>(),
            userManager,
            signInManager,
            Options.Create(new GoogleAuthSettings()),
            Substitute.For<ILogger<AuthService>>());

        var req = new RegisterOrganisationRequest
        {
            OrganisationName = "H",
            ContactEmail = "c@h.com",
            ContactFullName = "Contact",
            OrgType = 1
        };
        var expected = Result<OrganisationRegistrationResponse>.Success(new OrganisationRegistrationResponse
        {
            RequestId = Guid.NewGuid(),
            Email = req.ContactEmail,
            Message = "OK"
        });
        onboarding.SubmitRequestAsync(req, Arg.Any<CancellationToken>()).Returns(expected);

        var result = await sut.RegisterOrganisationAsync(req, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Data!.Email.Should().Be(req.ContactEmail);
        await onboarding.Received(1).SubmitRequestAsync(req, Arg.Any<CancellationToken>());
    }
}
