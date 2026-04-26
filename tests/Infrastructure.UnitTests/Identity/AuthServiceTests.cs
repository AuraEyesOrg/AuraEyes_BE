using Application.Common.Interfaces;
using Application.Common.Models;
using Application.Common.Models.Auth;
using Application.SystemAdmin.Organisations.Common;
using Domain.Common;
using Domain.Entities.Contracts;
using Domain.Entities.Users;
using Domain.Enums;
using Domain.Repositories;
using FluentAssertions;
using Infrastructure.Identity;
using Infrastructure.Settings;
using Infrastructure.UnitTests.Common;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;

namespace Infrastructure.UnitTests.Identity;

public class AuthServiceTests
{
    [Theory]
    [InlineData(1)]
    [InlineData(2)]
    [InlineData(3)]
    [InlineData(4)]
    [InlineData(5)]
    [InlineData(6)]
    [InlineData(7)]
    [InlineData(8)]
    [InlineData(9)]
    [InlineData(10)]
    public async Task RegisterOphthalmologistAsync_WhenNoCredentials_ShouldFailImmediately(int caseId)
    {
        var service = CreateServiceForValidationOnly();
        var request = CreateBaseRequest();
        request.FullName = $"Doctor Test {caseId}";
        request.Certificates.Clear();
        request.Degrees.Clear();

        var result = await service.RegisterOphthalmologistAsync(request, "https://example.com/confirm");

        result.IsSuccess.Should().BeFalse();
        result.Errors.Should().Contain("At least one credential is required");
    }

    [Theory]
    [InlineData("Medical License A", 1)]
    [InlineData("Medical License B", 2)]
    [InlineData("Medical License C", 3)]
    [InlineData("Medical License D", 4)]
    [InlineData("Medical License E", 5)]
    [InlineData("Medical License F", 6)]
    [InlineData("Medical License G", 7)]
    [InlineData("Medical License H", 8)]
    [InlineData("Medical License I", 9)]
    [InlineData("Medical License J", 10)]
    public async Task RegisterOphthalmologistAsync_WhenNoDegree_ShouldFail(string licenseName, int caseId)
    {
        var service = CreateServiceForValidationOnly();
        var request = CreateBaseRequest();
        request.FullName = $"NoDegree {caseId}";
        request.Certificates =
        [
            new CredentialItemDto
            {
                Type = CertificateType.License,
                Name = licenseName,
                IssuedDate = DateTime.UtcNow.AddYears(-1),
                ExpiryDate = DateTime.UtcNow.AddYears(1),
                File = new FormFile(new MemoryStream([1, 2, 3]), 0, 3, "file", "license.pdf")
            }
        ];

        var result = await service.RegisterOphthalmologistAsync(request, "https://example.com/confirm");

        result.IsSuccess.Should().BeFalse();
        result.Errors.Should().Contain("At least one degree is required");
    }

    [Theory]
    [InlineData("Degree A", 1)]
    [InlineData("Degree B", 2)]
    [InlineData("Degree C", 3)]
    [InlineData("Degree D", 4)]
    [InlineData("Degree E", 5)]
    [InlineData("Degree F", 6)]
    [InlineData("Degree G", 7)]
    [InlineData("Degree H", 8)]
    [InlineData("Degree I", 9)]
    [InlineData("Degree J", 10)]
    public async Task RegisterOphthalmologistAsync_WhenNoLicense_ShouldFail(string degreeName, int caseId)
    {
        var service = CreateServiceForValidationOnly();
        var request = CreateBaseRequest();
        request.FullName = $"NoLicense {caseId}";
        request.Certificates =
        [
            new CredentialItemDto
            {
                Type = CertificateType.Degree,
                DegreeLevel = DegreeLevel.Bachelor,
                Name = degreeName,
                IssuedDate = DateTime.UtcNow.AddYears(-5),
                ExpiryDate = null,
                File = new FormFile(new MemoryStream([1, 2, 3]), 0, 3, "file", "degree.pdf")
            }
        ];

        var result = await service.RegisterOphthalmologistAsync(request, "https://example.com/confirm");

        result.IsSuccess.Should().BeFalse();
        result.Errors.Should().Contain("At least one license/certificate is required");
    }

    [Theory]
    [InlineData(1)]
    [InlineData(2)]
    [InlineData(3)]
    [InlineData(4)]
    [InlineData(5)]
    [InlineData(6)]
    [InlineData(7)]
    [InlineData(8)]
    [InlineData(9)]
    [InlineData(10)]
    public async Task RegisterOphthalmologistAsync_WhenDegreeLevelMissing_ShouldFail(int caseId)
    {
        var service = CreateServiceForValidationOnly();
        var request = CreateBaseRequest();
        request.FullName = $"MissingDegreeLevel {caseId}";
        request.Certificates =
        [
            new CredentialItemDto
            {
                Type = CertificateType.Degree,
                DegreeLevel = null,
                Name = "Medical Degree",
                IssuedDate = DateTime.UtcNow.AddYears(-5),
                File = new FormFile(new MemoryStream([1, 2, 3]), 0, 3, "file", "degree.pdf")
            },
            new CredentialItemDto
            {
                Type = CertificateType.License,
                Name = "Medical License",
                IssuedDate = DateTime.UtcNow.AddYears(-1),
                ExpiryDate = DateTime.UtcNow.AddYears(1),
                File = new FormFile(new MemoryStream([1, 2, 3]), 0, 3, "file", "license.pdf")
            }
        ];

        var result = await service.RegisterOphthalmologistAsync(request, "https://example.com/confirm");

        result.IsSuccess.Should().BeFalse();
        result.Errors.Should().Contain("Degree level is required for degree credentials");
    }

    [Theory]
    [InlineData(1)]
    [InlineData(2)]
    [InlineData(3)]
    [InlineData(4)]
    [InlineData(5)]
    [InlineData(6)]
    [InlineData(7)]
    [InlineData(8)]
    [InlineData(9)]
    [InlineData(10)]
    public async Task RegisterOphthalmologistAsync_WhenCredentialFileMissing_ShouldFail(int caseId)
    {
        var service = CreateServiceForValidationOnly();
        var request = CreateBaseRequest();
        request.FullName = $"MissingFile {caseId}";
        request.Certificates =
        [
            new CredentialItemDto
            {
                Type = CertificateType.Degree,
                DegreeLevel = DegreeLevel.Bachelor,
                Name = "Medical Degree",
                IssuedDate = DateTime.UtcNow.AddYears(-5),
                File = null
            },
            new CredentialItemDto
            {
                Type = CertificateType.License,
                Name = "Medical License",
                IssuedDate = DateTime.UtcNow.AddYears(-1),
                ExpiryDate = DateTime.UtcNow.AddYears(1),
                File = new FormFile(new MemoryStream([1, 2, 3]), 0, 3, "file", "license.pdf")
            }
        ];

        var result = await service.RegisterOphthalmologistAsync(request, "https://example.com/confirm");

        result.IsSuccess.Should().BeFalse();
        result.Errors.Should().Contain("Credential file is required");
    }

    [Theory]
    [InlineData(0)]
    [InlineData(1)]
    [InlineData(2)]
    [InlineData(3)]
    [InlineData(4)]
    [InlineData(5)]
    [InlineData(6)]
    [InlineData(7)]
    [InlineData(8)]
    [InlineData(9)]
    public async Task RegisterOphthalmologistAsync_WhenLicenseExpiryNotGreaterThanIssued_ShouldFail(int minutesDelta)
    {
        var service = CreateServiceForValidationOnly();
        var request = CreateBaseRequest();
        var issued = DateTime.UtcNow.AddYears(-1);
        var expiry = issued.AddMinutes(minutesDelta == 0 ? 0 : -minutesDelta);
        request.Certificates =
        [
            new CredentialItemDto
            {
                Type = CertificateType.Degree,
                DegreeLevel = DegreeLevel.Bachelor,
                Name = "Medical Degree",
                IssuedDate = DateTime.UtcNow.AddYears(-5),
                File = new FormFile(new MemoryStream([1, 2, 3]), 0, 3, "file", "degree.pdf")
            },
            new CredentialItemDto
            {
                Type = CertificateType.License,
                Name = "Medical License",
                IssuedDate = issued,
                ExpiryDate = expiry,
                File = new FormFile(new MemoryStream([1, 2, 3]), 0, 3, "file", "license.pdf")
            }
        ];

        var result = await service.RegisterOphthalmologistAsync(request, "https://example.com/confirm");

        result.IsSuccess.Should().BeFalse();
        result.Errors.Should().Contain("Certificate expiry date must be later than issued date");
    }

    [Theory]
    [InlineData("not-a-guid")]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData("123")]
    [InlineData("guid-guid")]
    [InlineData("00000000-0000-0000-0000-00000000000Z")]
    [InlineData("{not-guid}")]
    [InlineData("null")]
    [InlineData("undefined")]
    [InlineData("abc-def-ghi")]
    public async Task ConfirmEmailAsync_WhenUserIdInvalid_ShouldFail(string userId)
    {
        var service = CreateServiceForValidationOnly();

        var result = await service.ConfirmEmailAsync(userId, "token");

        result.IsSuccess.Should().BeFalse();
        result.Errors.Should().Contain("Invalid user ID");
    }

    [Theory]
    [InlineData("bad-guid")]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData("123")]
    [InlineData("not-guid-value")]
    [InlineData("00000000-0000-0000-0000-00000000000Z")]
    [InlineData("{not-guid}")]
    [InlineData("null")]
    [InlineData("undefined")]
    [InlineData("abc-def-ghi")]
    public async Task ResetPasswordAsync_WhenUserIdInvalid_ShouldFail(string userId)
    {
        var service = CreateServiceForValidationOnly();
        var request = new ResetPasswordRequest
        {
            UserId = userId,
            Token = "token",
            NewPassword = "Password@123",
            ConfirmPassword = "Password@123"
        };

        var result = await service.ResetPasswordAsync(request);

        result.IsSuccess.Should().BeFalse();
        result.Errors.Should().Contain("Invalid user ID");
    }

    [Theory]
    [InlineData("org1@test.local")]
    [InlineData("org2@test.local")]
    [InlineData("org3@test.local")]
    [InlineData("org4@test.local")]
    [InlineData("org5@test.local")]
    [InlineData("org6@test.local")]
    [InlineData("org7@test.local")]
    [InlineData("org8@test.local")]
    [InlineData("org9@test.local")]
    [InlineData("org10@test.local")]
    public async Task RegisterOrganisationAsync_ShouldReturnServiceResult(string email)
    {
        var onboarding = new FakeOrganisationOnboardingService
        {
            SubmitResult = Result<OrganisationRegistrationResponse>.Success(new OrganisationRegistrationResponse
            {
                RequestId = Guid.NewGuid(),
                Email = email,
                Message = "submitted"
            })
        };
        var service = CreateServiceWithFakes(organisationOnboardingService: onboarding);

        var result = await service.RegisterOrganisationAsync(new RegisterOrganisationRequest
        {
            ContactEmail = email,
            ContactFullName = "Contact",
            OrganisationName = "Org",
            OrgType = (int)OrgType.Clinic
        });

        result.IsSuccess.Should().BeTrue();
        result.Data!.Email.Should().Be(email);
    }

    [Theory]
    [InlineData("lost1@test.local")]
    [InlineData("lost2@test.local")]
    [InlineData("lost3@test.local")]
    [InlineData("lost4@test.local")]
    [InlineData("lost5@test.local")]
    [InlineData("lost6@test.local")]
    [InlineData("lost7@test.local")]
    [InlineData("lost8@test.local")]
    [InlineData("lost9@test.local")]
    [InlineData("lost10@test.local")]
    public async Task ForgotPasswordAsync_WhenUserMissing_ShouldStillReturnSuccess(string email)
    {
        var identity = new FakeIdentityService();
        var service = CreateServiceWithFakes(identityService: identity, emailService: new FakeEmailServiceBridge());

        var result = await service.ForgotPasswordAsync(email, "https://example.com/reset");

        result.IsSuccess.Should().BeTrue();
    }

    [Theory]
    [InlineData("ok1@test.local")]
    [InlineData("ok2@test.local")]
    [InlineData("ok3@test.local")]
    [InlineData("ok4@test.local")]
    [InlineData("ok5@test.local")]
    [InlineData("ok6@test.local")]
    [InlineData("ok7@test.local")]
    [InlineData("ok8@test.local")]
    [InlineData("ok9@test.local")]
    [InlineData("ok10@test.local")]
    public async Task ResendConfirmationAsync_WhenUserMissing_ShouldReturnSuccess(string email)
    {
        var identity = new FakeIdentityService();
        var service = CreateServiceWithFakes(identityService: identity, emailService: new FakeEmailServiceBridge());

        var result = await service.ResendConfirmationAsync(email, "https://example.com/confirm");

        result.IsSuccess.Should().BeTrue();
    }

    [Theory]
    [InlineData("not-a-jwt", "refresh-token")]
    [InlineData("", "refresh-token")]
    [InlineData(" ", "refresh-token")]
    [InlineData("x.y.z", "refresh-token")]
    [InlineData("broken", "refresh-token")]
    [InlineData("header.payload.sig", "refresh-token")]
    [InlineData("jwt", "refresh-token")]
    [InlineData("a.b", "refresh-token")]
    [InlineData("a.b.c.d", "refresh-token")]
    [InlineData("null", "refresh-token")]
    public async Task RefreshTokenAsync_WhenAccessTokenInvalid_ShouldReturnUnauthorized(string accessToken, string refreshToken)
    {
        var tokenService = new FakeTokenService
        {
            UserIdFromToken = null,
            JtiFromToken = null
        };
        var service = CreateServiceWithFakes(tokenService: tokenService);

        var result = await service.RefreshTokenAsync(accessToken, refreshToken, "127.0.0.1");

        result.IsUnauthorized.Should().BeTrue();
        result.Errors.Should().Contain("Invalid access token");
    }

    [Theory]
    [InlineData("hash-a")]
    [InlineData("hash-b")]
    [InlineData("hash-c")]
    [InlineData("hash-d")]
    [InlineData("hash-e")]
    [InlineData("hash-f")]
    [InlineData("hash-g")]
    [InlineData("hash-h")]
    [InlineData("hash-i")]
    [InlineData("hash-j")]
    public async Task LogoutAsync_WhenTokenNotFound_ShouldStillSucceed(string refreshToken)
    {
        var refreshService = new FakeRefreshTokenService { TokenByHash = null };
        var service = CreateServiceWithFakes(refreshTokenService: refreshService);

        var result = await service.LogoutAsync(refreshToken);

        result.IsSuccess.Should().BeTrue();
    }

    [Theory]
    [InlineData("c1")]
    [InlineData("c2")]
    [InlineData("c3")]
    [InlineData("c4")]
    [InlineData("c5")]
    [InlineData("c6")]
    [InlineData("c7")]
    [InlineData("c8")]
    [InlineData("c9")]
    [InlineData("c10")]
    public async Task LogoutAllAsync_WhenRevocationWorks_ShouldSucceed(string marker)
    {
        var refreshService = new FakeRefreshTokenService();
        var service = CreateServiceWithFakes(refreshTokenService: refreshService);
        marker.Should().NotBeNullOrWhiteSpace();

        var result = await service.LogoutAllAsync(Guid.NewGuid());

        result.IsSuccess.Should().BeTrue();
    }

    [Theory]
    [InlineData("bad-1")]
    [InlineData("bad-2")]
    [InlineData("bad-3")]
    [InlineData("bad-4")]
    [InlineData("bad-5")]
    [InlineData("bad-6")]
    [InlineData("bad-7")]
    [InlineData("bad-8")]
    [InlineData("bad-9")]
    [InlineData("bad-10")]
    public async Task GetCurrentUserAsync_WhenUserMissing_ShouldReturnUnauthorized(string marker)
    {
        var identity = new FakeIdentityService();
        var service = CreateServiceWithFakes(identityService: identity);
        marker.Should().NotBeNullOrWhiteSpace();

        var result = await service.GetCurrentUserAsync(Guid.NewGuid());

        result.IsUnauthorized.Should().BeTrue();
        result.Errors.Should().Contain("User not found");
    }

    [Theory]
    [InlineData("dup1@test.local")]
    [InlineData("dup2@test.local")]
    [InlineData("dup3@test.local")]
    [InlineData("dup4@test.local")]
    [InlineData("dup5@test.local")]
    [InlineData("dup6@test.local")]
    [InlineData("dup7@test.local")]
    [InlineData("dup8@test.local")]
    [InlineData("dup9@test.local")]
    [InlineData("dup10@test.local")]
    public async Task RegisterOrganisationAsync_WhenOnboardingReturnsConflict_ShouldPassThroughConflict(string email)
    {
        var onboarding = new FakeOrganisationOnboardingService
        {
            SubmitResult = Result<OrganisationRegistrationResponse>.Conflict("duplicate")
        };
        var service = CreateServiceWithFakes(organisationOnboardingService: onboarding);

        var result = await service.RegisterOrganisationAsync(new RegisterOrganisationRequest
        {
            ContactEmail = email,
            ContactFullName = "Contact",
            OrganisationName = "Org",
            OrgType = (int)OrgType.Clinic
        });

        result.IsConflict.Should().BeTrue();
        result.Errors.Should().Contain("duplicate");
    }

    [Theory]
    [InlineData("err1@test.local")]
    [InlineData("err2@test.local")]
    [InlineData("err3@test.local")]
    [InlineData("err4@test.local")]
    [InlineData("err5@test.local")]
    [InlineData("err6@test.local")]
    [InlineData("err7@test.local")]
    [InlineData("err8@test.local")]
    [InlineData("err9@test.local")]
    [InlineData("err10@test.local")]
    public async Task ForgotPasswordAsync_WhenIdentityThrows_ShouldReturnFailure(string email)
    {
        var service = CreateServiceWithFakes(identityService: new ThrowingIdentityService());

        var result = await service.ForgotPasswordAsync(email, "https://example.com/reset");

        result.IsSuccess.Should().BeFalse();
        result.Errors.Should().Contain("An error occurred while processing your request");
    }

    [Theory]
    [InlineData("fail1@test.local")]
    [InlineData("fail2@test.local")]
    [InlineData("fail3@test.local")]
    [InlineData("fail4@test.local")]
    [InlineData("fail5@test.local")]
    [InlineData("fail6@test.local")]
    [InlineData("fail7@test.local")]
    [InlineData("fail8@test.local")]
    [InlineData("fail9@test.local")]
    [InlineData("fail10@test.local")]
    public async Task ResendConfirmationAsync_WhenIdentityThrows_ShouldReturnFailure(string email)
    {
        var service = CreateServiceWithFakes(identityService: new ThrowingIdentityService());

        var result = await service.ResendConfirmationAsync(email, "https://example.com/confirm");

        result.IsSuccess.Should().BeFalse();
        result.Errors.Should().Contain("An error occurred while processing your request");
    }

    [Theory]
    [InlineData("logout-f1")]
    [InlineData("logout-f2")]
    [InlineData("logout-f3")]
    [InlineData("logout-f4")]
    [InlineData("logout-f5")]
    [InlineData("logout-f6")]
    [InlineData("logout-f7")]
    [InlineData("logout-f8")]
    [InlineData("logout-f9")]
    [InlineData("logout-f10")]
    public async Task LogoutAsync_WhenRefreshTokenServiceThrows_ShouldReturnFailure(string refreshToken)
    {
        var service = CreateServiceWithFakes(refreshTokenService: new ThrowingRefreshTokenService());

        var result = await service.LogoutAsync(refreshToken);

        result.IsSuccess.Should().BeFalse();
        result.Errors.Should().Contain("An error occurred during logout");
    }

    [Theory]
    [InlineData(1)]
    [InlineData(2)]
    [InlineData(3)]
    [InlineData(4)]
    [InlineData(5)]
    [InlineData(6)]
    [InlineData(7)]
    [InlineData(8)]
    [InlineData(9)]
    [InlineData(10)]
    public async Task LogoutAllAsync_WhenRefreshTokenServiceThrows_ShouldReturnFailure(int caseId)
    {
        var service = CreateServiceWithFakes(refreshTokenService: new ThrowingRefreshTokenService());
        caseId.Should().BeGreaterThan(0);

        var result = await service.LogoutAllAsync(Guid.NewGuid());

        result.IsSuccess.Should().BeFalse();
        result.Errors.Should().Contain("An error occurred during logout");
    }

    [Theory]
    [InlineData("p1@test.local")]
    [InlineData("p2@test.local")]
    [InlineData("p3@test.local")]
    [InlineData("p4@test.local")]
    [InlineData("p5@test.local")]
    [InlineData("p6@test.local")]
    [InlineData("p7@test.local")]
    [InlineData("p8@test.local")]
    [InlineData("p9@test.local")]
    [InlineData("p10@test.local")]
    public async Task RegisterPatientAsync_WhenDependenciesMissing_ShouldReturnFailure(string email)
    {
        var service = CreateServiceForValidationOnly();
        var request = new RegisterPatientRequest
        {
            Email = email,
            Password = "Password@123",
            ConfirmPassword = "Password@123",
            FullName = "Patient"
        };

        var result = await service.RegisterPatientAsync(request, "https://example.com/confirm");

        result.IsSuccess.Should().BeFalse();
        result.Errors.Should().Contain("An error occurred during registration");
    }

    [Theory]
    [InlineData("g1")]
    [InlineData("g2")]
    [InlineData("g3")]
    [InlineData("g4")]
    [InlineData("g5")]
    [InlineData("g6")]
    [InlineData("g7")]
    [InlineData("g8")]
    [InlineData("g9")]
    [InlineData("g10")]
    public async Task GoogleLoginAsync_WithInvalidToken_ShouldReturnUnauthorized(string caseId)
    {
        var service = CreateServiceForValidationOnly();

        var result = await service.GoogleLoginAsync(new GoogleLoginRequest
        {
            Credential = $"invalid-{caseId}",
            DeviceInfo = "web"
        }, "127.0.0.1");

        result.IsUnauthorized.Should().BeTrue();
        result.Errors.Should().Contain("Invalid Google token");
    }

    [Theory]
    [InlineData("l1@test.local")]
    [InlineData("l2@test.local")]
    [InlineData("l3@test.local")]
    [InlineData("l4@test.local")]
    [InlineData("l5@test.local")]
    [InlineData("l6@test.local")]
    [InlineData("l7@test.local")]
    [InlineData("l8@test.local")]
    [InlineData("l9@test.local")]
    [InlineData("l10@test.local")]
    public async Task LoginAsync_WhenDependenciesMissing_ShouldReturnFailure(string email)
    {
        var service = CreateServiceForValidationOnly();

        var result = await service.LoginAsync(new LoginRequest
        {
            Email = email,
            Password = "Password@123",
            DeviceInfo = "web"
        }, "127.0.0.1");

        result.IsSuccess.Should().BeFalse();
        result.Errors.Should().Contain("An error occurred during login");
    }

    [Theory]
    [InlineData(1)]
    [InlineData(2)]
    [InlineData(3)]
    [InlineData(4)]
    [InlineData(5)]
    [InlineData(6)]
    [InlineData(7)]
    [InlineData(8)]
    [InlineData(9)]
    [InlineData(10)]
    public async Task VerifyTwoFactorLoginAsync_WhenDependenciesMissing_ShouldReturnFailure(int caseId)
    {
        var service = CreateServiceForValidationOnly();
        caseId.Should().BeGreaterThan(0);

        var result = await service.VerifyTwoFactorLoginAsync(new VerifyTwoFactorRequest
        {
            UserId = Guid.NewGuid(),
            Code = "123456",
            UseRecoveryCode = false,
            DeviceInfo = "web"
        }, "127.0.0.1");

        result.IsSuccess.Should().BeFalse();
        result.Errors.Should().Contain("An error occurred during verification");
    }

    private static RegisterOphthalmologistRequest CreateBaseRequest()
        => new()
        {
            Email = "doc@test.local",
            Password = "Password@123",
            ConfirmPassword = "Password@123",
            FullName = "Doctor Test",
            EmploymentType = OphthalmologistEmploymentType.FullTime
        };

    private static AuthService CreateServiceForValidationOnly()
    {
        var google = Options.Create(new GoogleAuthSettings { ClientId = "x" });
        return new AuthService(
            identityService: null!,
            tokenService: null!,
            refreshTokenService: null!,
            emailService: null!,
            organisationOnboardingService: null!,
            notificationService: null!,
            fileStorageService: null!,
            patientRepository: null!,
            ophthalmologistRepository: null!,
            clinicStaffRepository: null!,
            contractRepository: null!,
            unitOfWork: null!,
            userManager: null!,
            signInManager: null!,
            googleAuthSettings: google,
            logger: new TestLogger<AuthService>());
    }

    private static AuthService CreateServiceWithFakes(
        IIdentityService? identityService = null,
        ITokenService? tokenService = null,
        IRefreshTokenService? refreshTokenService = null,
        IEmailService? emailService = null,
        IOrganisationOnboardingService? organisationOnboardingService = null)
    {
        var google = Options.Create(new GoogleAuthSettings { ClientId = "x" });
        return new AuthService(
            identityService: identityService ?? new FakeIdentityService(),
            tokenService: tokenService ?? new FakeTokenService(),
            refreshTokenService: refreshTokenService ?? new FakeRefreshTokenService(),
            emailService: emailService ?? new FakeEmailServiceBridge(),
            organisationOnboardingService: organisationOnboardingService ?? new FakeOrganisationOnboardingService(),
            notificationService: null!,
            fileStorageService: null!,
            patientRepository: new FakeRepository<Patient>(),
            ophthalmologistRepository: new FakeRepository<Ophthalmologist>(),
            clinicStaffRepository: new FakeClinicStaffRepository(),
            contractRepository: new FakeContractRepository(),
            unitOfWork: new FakeUnitOfWork(),
            userManager: null!,
            signInManager: null!,
            googleAuthSettings: google,
            logger: new TestLogger<AuthService>());
    }

    private class FakeIdentityService : IIdentityService
    {
        public Task<(bool Succeeded, string[] Errors)> CreateUserAsync(string email, string password, string fullName, CancellationToken cancellationToken = default) => Task.FromResult((true, Array.Empty<string>()));
        public Task<(bool Succeeded, string[] Errors)> CreateUserWithRoleAsync(string email, string password, string fullName, string role, CancellationToken cancellationToken = default) => Task.FromResult((true, Array.Empty<string>()));
        public Task<(bool Succeeded, Guid? UserId, string[] Errors)> CreateUserWalkInPatientAsync(string email, string password, string fullName, string role, Guid? organizationId = null, UserProfileWalkInDto? userProfile = null, CancellationToken cancellationToken = default) => Task.FromResult((true, (Guid?)Guid.NewGuid(), Array.Empty<string>()));
        public Task<bool> CheckPasswordAsync(Guid userId, string password) => Task.FromResult(false);
        public virtual Task<UserDto?> GetUserByEmailAsync(string email, CancellationToken cancellationToken = default) => Task.FromResult<UserDto?>(null);
        public Task<UserDto?> GetUserByCitizenIdAsync(string citizenId, CancellationToken cancellationToken = default) => Task.FromResult<UserDto?>(null);
        public Task<UserDto?> GetUserByIdAsync(Guid userId, CancellationToken cancellationToken = default) => Task.FromResult<UserDto?>(null);
        public Task<bool> IsPhoneNumberInUseByOrganizationAsync(Guid organizationId, string phoneNumber, CancellationToken cancellationToken = default) => Task.FromResult(false);
        public Task<bool> IsCitizenIdInUseByOrganizationAsync(Guid organizationId, string citizenId, CancellationToken cancellationToken = default) => Task.FromResult(false);
        public Task<bool> IsEmailConfirmedAsync(Guid userId) => Task.FromResult(false);
        public Task<bool> IsUserActiveAsync(Guid userId) => Task.FromResult(false);
        public Task<string> GenerateEmailConfirmationTokenAsync(Guid userId) => Task.FromResult("token");
        public Task<(bool Succeeded, string[] Errors)> ConfirmEmailAsync(Guid userId, string token) => Task.FromResult((true, Array.Empty<string>()));
        public Task<string> GeneratePasswordResetTokenAsync(Guid userId) => Task.FromResult("reset");
        public Task<(bool Succeeded, string[] Errors)> ResetPasswordAsync(Guid userId, string token, string newPassword) => Task.FromResult((true, Array.Empty<string>()));
        public Task<IList<string>> GetUserRolesAsync(Guid userId) => Task.FromResult<IList<string>>(Array.Empty<string>());
        public Task<(bool Succeeded, string[] Errors)> AddToRoleAsync(Guid userId, string role) => Task.FromResult((true, Array.Empty<string>()));
        public Task<bool> IsInRoleAsync(Guid userId, string role) => Task.FromResult(false);
        public Task<IReadOnlyList<Guid>> GetUserIdsByRoleAndOrganizationAsync(string role, Guid organizationId, CancellationToken cancellationToken = default) => Task.FromResult<IReadOnlyList<Guid>>(Array.Empty<Guid>());
        public Task UpdateLastLoginAsync(Guid userId) => Task.CompletedTask;
        public Task<(bool Succeeded, string[] Errors)> DeactivateUserAsync(Guid userId) => Task.FromResult((true, Array.Empty<string>()));
        public Task<(bool Succeeded, string[] Errors)> SoftDeleteUserAsync(Guid userId) => Task.FromResult((true, Array.Empty<string>()));
        public Task<bool> IsTwoFactorEnabledAsync(Guid userId) => Task.FromResult(false);
        public Task<string?> GetAuthenticatorKeyAsync(Guid userId) => Task.FromResult<string?>("key");
        public Task<string> GetOrCreateAuthenticatorKeyAsync(Guid userId) => Task.FromResult("key");
        public string GenerateAuthenticatorUri(string email, string sharedKey) => "";
        public string FormatAuthenticatorKey(string key) => key;
        public Task<(bool Succeeded, string[] Errors, string[]? RecoveryCodes)> EnableTwoFactorAsync(Guid userId, string verificationCode) => Task.FromResult((true, Array.Empty<string>(), Array.Empty<string>() as string[]));
        public Task<(bool Succeeded, string[] Errors)> DisableTwoFactorAsync(Guid userId) => Task.FromResult((true, Array.Empty<string>()));
        public Task<bool> VerifyTwoFactorCodeAsync(Guid userId, string code) => Task.FromResult(false);
        public Task<(bool Succeeded, string[] Errors)> VerifyRecoveryCodeAsync(Guid userId, string recoveryCode) => Task.FromResult((false, new[] { "invalid" }));
        public Task<string[]> GenerateNewRecoveryCodesAsync(Guid userId, int count = 10) => Task.FromResult(Array.Empty<string>());
        public Task<int> GetRecoveryCodesCountAsync(Guid userId) => Task.FromResult(0);
        public Task<(List<UserAdminDto> Users, int TotalCount)> GetUsersAsync(string? searchTerm = null, string? roleFilter = null, string? statusFilter = null, int pageNumber = 1, int pageSize = 10, CancellationToken cancellationToken = default) => Task.FromResult((new List<UserAdminDto>(), 0));
        public Task<UserMetricsDto> GetUserMetricsAsync(CancellationToken cancellationToken = default) => Task.FromResult(new UserMetricsDto(0, 0, 0, 0, 0, 0, 0));
        public Task<int> GetUsersInRoleCountAsync(string role, bool activeOnly = true, CancellationToken cancellationToken = default) => Task.FromResult(0);
        public Task<(bool Succeeded, string[] Errors)> RemoveFromRoleAsync(Guid userId, string role) => Task.FromResult((true, Array.Empty<string>()));
        public Task<(bool Succeeded, string[] Errors)> ActivateUserAsync(Guid userId) => Task.FromResult((true, Array.Empty<string>()));
        public Task<(bool Succeeded, string[] Errors)> ApproveUserAsync(Guid userId) => Task.FromResult((true, Array.Empty<string>()));
        public Task<int> GetPendingApprovalsCountAsync(CancellationToken cancellationToken = default) => Task.FromResult(0);
        public Task<UserDetailsDto?> GetUserDetailsAsync(Guid userId, CancellationToken cancellationToken = default) => Task.FromResult<UserDetailsDto?>(null);
        public Task<(bool Succeeded, string[] Errors)> UpdateUserProfileAsync(Guid userId, string fullName, string? phone, DateTime? dateOfBirth, int? gender, string? address, CancellationToken cancellationToken = default) => Task.FromResult((true, Array.Empty<string>()));
        public Task<(bool Succeeded, string[] Errors)> UpdateAvatarUrlAsync(Guid userId, string avatarUrl, CancellationToken cancellationToken = default) => Task.FromResult((true, Array.Empty<string>()));
        public Task<(bool Succeeded, string[] Errors)> UpdateUserOrganizationAsync(Guid userId, Guid? organizationId, CancellationToken cancellationToken = default) => Task.FromResult((true, Array.Empty<string>()));
        public Task<(bool Succeeded, string[] Errors)> ChangePasswordAsync(Guid userId, string currentPassword, string newPassword, CancellationToken cancellationToken = default) => Task.FromResult((true, Array.Empty<string>()));
        public Task<(bool Succeeded, string[] Errors)> UpdateUserEmailAsync(Guid userId, string email, CancellationToken cancellationToken = default) => Task.FromResult((true, Array.Empty<string>()));
        public Task<IList<string>> GetUserPermissionsAsync(Guid userId) => Task.FromResult<IList<string>>(Array.Empty<string>());
        public Task SynchronizeRolesWithDefaultsAsync(CancellationToken cancellationToken = default) => Task.CompletedTask;

        public Task<IReadOnlyList<UserDto>> GetUsersByIdsAsync(IEnumerable<Guid> userIds, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Task<(bool Succeeded, string[] Errors)> UpdateUserProfileAsync(Guid userId, string fullName, string? phone, DateTime? dateOfBirth, int? gender, string? address, string? citizenId, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Task<(bool Succeeded, string[] Errors)> SetStaffOnboardingStatusAsync(Guid userId)
        {
            throw new NotImplementedException();
        }
    }

    private sealed class FakeTokenService : ITokenService
    {
        public Guid? UserIdFromToken { get; set; }
        public string? JtiFromToken { get; set; }
        public Task<TokenResult> GenerateAccessTokenAsync(Guid userId, string email, string fullName, IEnumerable<string> roles, IEnumerable<System.Security.Claims.Claim>? additionalClaims = null)
            => Task.FromResult(new TokenResult("access", Guid.NewGuid().ToString(), DateTime.UtcNow.AddMinutes(15)));
        public string GenerateRefreshToken() => "refresh";
        public System.Security.Claims.ClaimsPrincipal? ValidateToken(string token) => null;
        public Guid? GetUserIdFromToken(string token) => UserIdFromToken;
        public string? GetJtiFromToken(string token) => JtiFromToken;
    }

    private sealed class FakeRefreshTokenService : IRefreshTokenService
    {
        public RefreshTokenDto? TokenByHash { get; set; }
        public Task<Guid> CreateRefreshTokenAsync(Guid userId, string tokenHash, string jwtId, int expiryDays = 7, string? deviceInfo = null, string? ipAddress = null, CancellationToken cancellationToken = default) => Task.FromResult(Guid.NewGuid());
        public Task<RefreshTokenDto?> GetByTokenHashAsync(string tokenHash, CancellationToken cancellationToken = default) => Task.FromResult(TokenByHash);
        public Task<Guid> RotateRefreshTokenAsync(Guid oldTokenId, string newTokenHash, string newJwtId, int expiryDays = 7, string? deviceInfo = null, string? ipAddress = null, CancellationToken cancellationToken = default) => Task.FromResult(Guid.NewGuid());
        public Task RevokeTokenAsync(Guid tokenId, string reason = "manual_revocation", CancellationToken cancellationToken = default) => Task.CompletedTask;
        public Task RevokeAllUserTokensAsync(Guid userId, string reason = "logout_all", CancellationToken cancellationToken = default) => Task.CompletedTask;
        public Task RevokeTokenFamilyAsync(Guid tokenId, string reason = "token_reuse_detected", CancellationToken cancellationToken = default) => Task.CompletedTask;
        public Task<int> CleanupExpiredTokensAsync(int daysToKeep = 30, CancellationToken cancellationToken = default) => Task.FromResult(0);
    }

    private sealed class ThrowingRefreshTokenService : IRefreshTokenService
    {
        public Task<Guid> CreateRefreshTokenAsync(Guid userId, string tokenHash, string jwtId, int expiryDays = 7, string? deviceInfo = null, string? ipAddress = null, CancellationToken cancellationToken = default) => throw new InvalidOperationException("boom");
        public Task<RefreshTokenDto?> GetByTokenHashAsync(string tokenHash, CancellationToken cancellationToken = default) => throw new InvalidOperationException("boom");
        public Task<Guid> RotateRefreshTokenAsync(Guid oldTokenId, string newTokenHash, string newJwtId, int expiryDays = 7, string? deviceInfo = null, string? ipAddress = null, CancellationToken cancellationToken = default) => throw new InvalidOperationException("boom");
        public Task RevokeTokenAsync(Guid tokenId, string reason = "manual_revocation", CancellationToken cancellationToken = default) => throw new InvalidOperationException("boom");
        public Task RevokeAllUserTokensAsync(Guid userId, string reason = "logout_all", CancellationToken cancellationToken = default) => throw new InvalidOperationException("boom");
        public Task RevokeTokenFamilyAsync(Guid tokenId, string reason = "token_reuse_detected", CancellationToken cancellationToken = default) => throw new InvalidOperationException("boom");
        public Task<int> CleanupExpiredTokensAsync(int daysToKeep = 30, CancellationToken cancellationToken = default) => throw new InvalidOperationException("boom");
    }

    private sealed class FakeEmailServiceBridge : IEmailService
    {
        public Task SendEmailConfirmationAsync(string email, string confirmationLink, CancellationToken cancellationToken = default) => Task.CompletedTask;
        public Task SendPasswordResetAsync(string email, string resetLink, CancellationToken cancellationToken = default) => Task.CompletedTask;
        public Task SendWelcomeEmailAsync(string email, string fullName, CancellationToken cancellationToken = default) => Task.CompletedTask;
        public Task SendAsync(string to, string subject, string body, bool isHtml = true, CancellationToken cancellationToken = default) => Task.CompletedTask;
        public Task SendClinicAppointmentConfirmationAsync(string email, ClinicAppointmentConfirmationEmailPayload payload, CancellationToken cancellationToken = default) => Task.CompletedTask;
        public Task SendOrganisationScreeningResultShareAsync(string email, OrganisationScreeningResultShareEmailPayload payload, IReadOnlyCollection<EmailAttachment> attachments, CancellationToken cancellationToken = default) => Task.CompletedTask;
        public Task SendWithAttachmentsAsync(string to, string subject, string body, IReadOnlyCollection<EmailAttachment> attachments, bool isHtml = true, CancellationToken cancellationToken = default) => Task.CompletedTask;

        public Task SendStaffOnboardingEmailAsync(string email, string fullName, string temporaryPassword, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }
    }

    private sealed class ThrowingIdentityService : FakeIdentityService
    {
        public override Task<UserDto?> GetUserByEmailAsync(string email, CancellationToken cancellationToken = default) => throw new InvalidOperationException("boom");
    }

    private sealed class FakeOrganisationOnboardingService : IOrganisationOnboardingService
    {
        public Result<OrganisationRegistrationResponse> SubmitResult { get; set; } = Result<OrganisationRegistrationResponse>.Success(new OrganisationRegistrationResponse());
        public Task<Result<OrganisationRegistrationResponse>> SubmitRequestAsync(RegisterOrganisationRequest request, CancellationToken cancellationToken = default) => Task.FromResult(SubmitResult);
        public Task<Result<IReadOnlyList<OrganisationOnboardingRequestDto>>> GetRequestsAsync(CancellationToken cancellationToken = default) => Task.FromResult(Result<IReadOnlyList<OrganisationOnboardingRequestDto>>.Success(Array.Empty<OrganisationOnboardingRequestDto>()));
        public Task<Result<ApproveOrganisationOnboardingResult>> ApproveRequestAsync(Guid requestId, Guid approvedByUserId, CancellationToken cancellationToken = default) => Task.FromResult(Result<ApproveOrganisationOnboardingResult>.Failure("not implemented"));
    }

    private sealed class FakeUnitOfWork : IUnitOfWork
    {
        public Task<int> SaveChangesAsync(CancellationToken cancellationToken = default) => Task.FromResult(0);
        public Task BeginTransactionAsync(CancellationToken cancellationToken = default) => Task.CompletedTask;
        public Task CommitTransactionAsync(CancellationToken cancellationToken = default) => Task.CompletedTask;
        public Task RollbackTransactionAsync(CancellationToken cancellationToken = default) => Task.CompletedTask;
    }

    private class FakeRepository<T> : IRepository<T> where T : BaseEntity, IAggregateRoot
    {
        public IQueryable<T> Query() => new List<T>().AsQueryable();
        public Task<T?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default) => Task.FromResult<T?>(null);
        public Task<IReadOnlyList<T>> GetAllAsync(CancellationToken cancellationToken = default) => Task.FromResult<IReadOnlyList<T>>(Array.Empty<T>());
        public Task<IReadOnlyList<T>> FindAsync(System.Linq.Expressions.Expression<Func<T, bool>> predicate, CancellationToken cancellationToken = default) => Task.FromResult<IReadOnlyList<T>>(Array.Empty<T>());
        public Task<T> AddAsync(T entity, CancellationToken cancellationToken = default) => Task.FromResult(entity);
        public Task UpdateAsync(T entity, CancellationToken cancellationToken = default) => Task.CompletedTask;
        public Task DeleteAsync(T entity, CancellationToken cancellationToken = default) => Task.CompletedTask;
        public Task<int> CountAsync(System.Linq.Expressions.Expression<Func<T, bool>>? predicate = null, CancellationToken cancellationToken = default) => Task.FromResult(0);
        public Task<bool> ExistsAsync(System.Linq.Expressions.Expression<Func<T, bool>> predicate, CancellationToken cancellationToken = default) => Task.FromResult(false);
    }

    private sealed class FakeContractRepository : FakeRepository<Contract>, IContractRepository
    {
        public Task<Contract?> GetByIdWithDetailsAsync(Guid id, CancellationToken cancellationToken = default) => Task.FromResult<Contract?>(null);
        public Task<(IReadOnlyList<Contract> Items, int TotalCount)> GetPagedAsync(string? searchTerm = null, Guid? userId = null, ContractStatus? status = null, ContractType? contractType = null, int pageNumber = 1, int pageSize = 20, CancellationToken cancellationToken = default) => Task.FromResult<(IReadOnlyList<Contract>, int)>((Array.Empty<Contract>(), 0));
        public Task<bool> ExistsByContractNumberAsync(string contractNumber, Guid? excludeId = null, CancellationToken cancellationToken = default) => Task.FromResult(false);
        public Task<Contract?> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken = default) => Task.FromResult<Contract?>(null);
    }

    private sealed class FakeClinicStaffRepository : FakeRepository<ClinicStaff>, IClinicStaffRepository
    {
        public Task<ClinicStaff?> GetByIdWithDetailsAsync(Guid id, CancellationToken cancellationToken = default) => Task.FromResult<ClinicStaff?>(null);
        public Task<ClinicStaff?> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken = default) => Task.FromResult<ClinicStaff?>(null);
        public Task<List<ClinicStaff>> GetAllActiveAsync(CancellationToken cancellationToken = default) => Task.FromResult(new List<ClinicStaff>());
        public Task<List<ClinicStaff>> GetBySubRoleAsync(ClinicStaffRole subRole, CancellationToken cancellationToken = default) => Task.FromResult(new List<ClinicStaff>());
        public Task<bool> ExistsByUserIdAsync(Guid userId, CancellationToken cancellationToken = default) => Task.FromResult(false);
        public void Remove(ClinicStaff clinicStaff) { }
    }
}
