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

        public Task<(bool Succeeded, string[] Errors)> ClearMustUpdateProfileFlagAsync(Guid userId)
        {
            return Task.FromResult((true, Array.Empty<string>()));
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
