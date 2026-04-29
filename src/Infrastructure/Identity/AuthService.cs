using System.Security.Claims;
using Application.Common.Constants;
using Application.Common.Interfaces;
using Application.Common.Models;
using Application.Common.Models.Auth;
using Application.Scheduling.ScheduleTemplates.Interfaces;
using Domain.Common;
using Domain.Entities.Users;
using Domain.Enums;
using Domain.Repositories;
using Google.Apis.Auth;
using Infrastructure.Settings;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Infrastructure.Identity;

/// <summary>
/// Authentication service implementation.
/// Orchestrates IIdentityService, ITokenService, IRefreshTokenService, and IEmailService.
/// Follows the "delegates/uses" pattern - AuthService uses IEmailService but doesn't own it.
/// Uses Repository pattern and Unit of Work for data access abstraction.
/// Note: Try-catch is minimal as ExceptionHandlingMiddleware handles exceptions globally.
/// </summary>
public class AuthService : IAuthService
{
    private const string GoogleLoginProvider = "Google";
    private const string ProviderAvatarClaimType = "provider_avatar_url";

    private readonly IIdentityService _identityService;
    private readonly ITokenService _tokenService;
    private readonly IRefreshTokenService _refreshTokenService;
    private readonly IEmailService _emailService;
    private readonly INotificationService _notificationService;
    private readonly IFileStorageService _fileStorageService;
    private readonly IRepository<Patient> _patientRepository;
    private readonly IRepository<Ophthalmologist> _ophthalmologistRepository;
    private readonly IClinicStaffRepository _clinicStaffRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly SignInManager<ApplicationUser> _signInManager;
    private readonly GoogleAuthSettings _googleAuthSettings;
    private readonly ILogger<AuthService> _logger;

    public AuthService(
        IIdentityService identityService,
        ITokenService tokenService,
        IRefreshTokenService refreshTokenService,
        IEmailService emailService,
        INotificationService notificationService,
        IFileStorageService fileStorageService,
        IRepository<Patient> patientRepository,
        IRepository<Ophthalmologist> ophthalmologistRepository,
        IClinicStaffRepository clinicStaffRepository,
        IUnitOfWork unitOfWork,
        UserManager<ApplicationUser> userManager,
        SignInManager<ApplicationUser> signInManager,
        IOptions<GoogleAuthSettings> googleAuthSettings,
        ILogger<AuthService> logger)
    {
        _identityService = identityService;
        _tokenService = tokenService;
        _refreshTokenService = refreshTokenService;
        _emailService = emailService;
        _notificationService = notificationService;
        _fileStorageService = fileStorageService;
        _patientRepository = patientRepository;
        _ophthalmologistRepository = ophthalmologistRepository;
        _clinicStaffRepository = clinicStaffRepository;
        _unitOfWork = unitOfWork;
        _userManager = userManager;
        _signInManager = signInManager;
        _googleAuthSettings = googleAuthSettings.Value;
        _logger = logger;
    }

    /// <inheritdoc />
    public async Task<Result<RegisterResponse>> RegisterPatientAsync(
        RegisterPatientRequest request,
        string confirmationUrlBase,
        CancellationToken cancellationToken = default)
    {
        try
        {
            // Check if user already exists
            var existingUser = await _identityService.GetUserByEmailAsync(request.Email, cancellationToken);
            if (existingUser != null)
            {
                return Result<RegisterResponse>.Failure("A user with this email already exists");
            }

            // Begin transaction to ensure atomicity across User + Role + Patient profile
            await _unitOfWork.BeginTransactionAsync(cancellationToken);

            // Create user
            var user = new ApplicationUser
            {
                UserName = request.Email,
                Email = request.Email,
                FullName = request.FullName,
                Address = request.Address,
                DateOfBirth = request.DateOfBirth,
                Gender = request.Gender.HasValue ? (Gender)request.Gender.Value : null,
                PhoneNumber = request.PhoneNumber,
                CitizenId = request.CitizenId
            };

            var createResult = await _userManager.CreateAsync(user, request.Password);
            if (!createResult.Succeeded)
            {
                await _unitOfWork.RollbackTransactionAsync(cancellationToken);
                return Result<RegisterResponse>.Failure(createResult.Errors.Select(e => e.Description));
            }

            // Add to Patient role
            await _identityService.AddToRoleAsync(user.Id, Roles.Patient);

            // Create Patient profile using factory method (ensures IsDeleted = false)
            var patient = Patient.CreateRegistered(user.Id);
            await _patientRepository.AddAsync(patient, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            // All DB operations succeeded — commit transaction
            await _unitOfWork.CommitTransactionAsync(cancellationToken);

            // Send confirmation email (best-effort, after commit — failure must NOT
            // trigger rollback since DB is already committed)
            try
            {
                var confirmationToken = await _identityService.GenerateEmailConfirmationTokenAsync(user.Id);
                var confirmationLink = $"{confirmationUrlBase}?userId={user.Id}&token={Uri.EscapeDataString(confirmationToken)}";
                await _emailService.SendEmailConfirmationAsync(user.Email!, confirmationLink, cancellationToken);
            }
            catch (Exception emailEx)
            {
                _logger.LogWarning(emailEx,
                    "Failed to send confirmation email for {Email}. User is registered but needs manual email confirmation.",
                    request.Email);
            }

            _logger.LogInformation("Patient registered: {Email}", request.Email);

            return Result<RegisterResponse>.Success(new RegisterResponse
            {
                UserId = user.Id,
                Email = user.Email!,
                Message = "Registration successful. Please check your email to confirm your account."
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error registering patient: {Email}", request.Email);

            // Rollback all DB changes (User, Role, Patient)
            try { await _unitOfWork.RollbackTransactionAsync(cancellationToken); }
            catch (Exception rbEx)
            {
                _logger.LogWarning(rbEx, "Failed to rollback transaction for: {Email}", request.Email);
            }

            return Result<RegisterResponse>.Failure("An error occurred during registration");
        }
    }

    /// <inheritdoc />
    public async Task<Result<RegisterResponse>> RegisterOphthalmologistAsync(
        RegisterOphthalmologistRequest request,
        string confirmationUrlBase,
        CancellationToken cancellationToken = default)
    {
        // Track uploaded files for compensating rollback if DB commit fails
        var uploadedFileUrls = new List<string>();

        try
        {
            var normalizedCredentials = NormalizeCredentials(request);

            if (normalizedCredentials.Count == 0)
            {
                return Result<RegisterResponse>.Failure("At least one credential is required");
            }

            if (!normalizedCredentials.Any(c => c.Type == CertificateType.Degree))
            {
                return Result<RegisterResponse>.Failure("At least one degree is required");
            }

            if (!normalizedCredentials.Any(c => c.Type == CertificateType.License))
            {
                return Result<RegisterResponse>.Failure("At least one license/certificate is required");
            }

            foreach (var certificate in normalizedCredentials)
            {
                if (certificate.File is null || certificate.File.Length == 0)
                {
                    return Result<RegisterResponse>.Failure("Credential file is required");
                }

                var issuedDateUtc = EnsureUtc(certificate.IssuedDate);
                var expiryDateUtc = EnsureUtc(certificate.ExpiryDate);

                if (certificate.Type == CertificateType.Degree)
                {
                    if (!certificate.DegreeLevel.HasValue)
                    {
                        return Result<RegisterResponse>.Failure("Degree level is required for degree credentials");
                    }

                    if (expiryDateUtc.HasValue)
                    {
                        return Result<RegisterResponse>.Failure("Expiry date must be empty for degree credentials");
                    }
                }

                if (certificate.Type == CertificateType.License)
                {
                    if (!expiryDateUtc.HasValue)
                    {
                        return Result<RegisterResponse>.Failure("Expiry date is required for license credentials");
                    }

                    if (expiryDateUtc.Value <= issuedDateUtc)
                    {
                        return Result<RegisterResponse>.Failure("Certificate expiry date must be later than issued date");
                    }
                }
            }

            var existingUser = await _identityService.GetUserByEmailAsync(request.Email, cancellationToken);
            if (existingUser != null)
            {
                return Result<RegisterResponse>.Failure("A user with this email already exists");
            }

            // Begin transaction to ensure atomicity across User + Role + Ophthalmologist profile
            await _unitOfWork.BeginTransactionAsync(cancellationToken);

            var user = new ApplicationUser
            {
                UserName = request.Email,
                Email = request.Email,
                FullName = request.FullName
            };

            var createResult = await _userManager.CreateAsync(user, request.Password);
            if (!createResult.Succeeded)
            {
                await _unitOfWork.RollbackTransactionAsync(cancellationToken);
                return Result<RegisterResponse>.Failure(createResult.Errors.Select(e => e.Description));
            }

            await _identityService.AddToRoleAsync(user.Id, Roles.Ophthalmologist);

            // Upload credential files to Supabase S3 and map to Certificate entities
            string? licenseUrl = null;
            string? degreeUrl = null;

            var ophthalmologist = new Ophthalmologist(
                user.Id,
                request.Bio,
                request.Phone,
                licenseUrl,
                degreeUrl,
                request.EmploymentType);

            foreach (var certificate in normalizedCredentials)
            {
                var file = certificate.File;
                if (file is null || file.Length == 0)
                {
                    return Result<RegisterResponse>.Failure("Credential file is required");
                }

                await using var stream = file.OpenReadStream();
                var uploadedUrl = await _fileStorageService.SaveFileAsync(
                    stream,
                    file.FileName,
                    $"ophthalmologists/credentials/{user.Id}",
                    cancellationToken);

                uploadedFileUrls.Add(uploadedUrl);

                if (certificate.Type == CertificateType.Degree)
                {
                    degreeUrl ??= uploadedUrl;
                }
                else if (certificate.Type == CertificateType.License)
                {
                    licenseUrl ??= uploadedUrl;
                }

                var certificateIssuedDateUtc = EnsureUtc(certificate.IssuedDate);
                var certificateExpiryDateUtc = EnsureUtc(certificate.ExpiryDate);

                ophthalmologist.AddCertificate(new Certificate(
                    ophthalmologist.Id,
                    certificate.Type,
                    certificate.Name,
                    certificate.DegreeLevel,
                    certificate.IssuingAuthority,
                    certificateIssuedDateUtc,
                    certificateExpiryDateUtc,
                    uploadedUrl));
            }

            ophthalmologist.UpdateCredentialFiles(licenseUrl, degreeUrl);

            await _ophthalmologistRepository.AddAsync(ophthalmologist, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            // All DB operations succeeded — commit transaction
            await _unitOfWork.CommitTransactionAsync(cancellationToken);

            // Send confirmation email (best-effort, after commit — failure here must NOT
            // trigger rollback or S3 cleanup since DB is already committed)
            try
            {
                var confirmationToken = await _identityService.GenerateEmailConfirmationTokenAsync(user.Id);
                var confirmationLink = $"{confirmationUrlBase}?userId={user.Id}&token={Uri.EscapeDataString(confirmationToken)}";
                await _emailService.SendEmailConfirmationAsync(user.Email!, confirmationLink, cancellationToken);
            }
            catch (Exception emailEx)
            {
                _logger.LogWarning(emailEx,
                    "Failed to send confirmation email for {Email}. User is registered but needs manual email confirmation.",
                    request.Email);
            }

            // Best-effort: Notify System Admin about new ophthalmologist registration
            try
            {
                var adminUsers = await _userManager.GetUsersInRoleAsync(Roles.SystemAdmin);
                foreach (var admin in adminUsers)
                {
                    if (!string.IsNullOrEmpty(admin.Email))
                    {
                        await _emailService.SendAsync(
                            admin.Email,
                            "[AURA] New Ophthalmologist Registration - Credential Review Required",
                            $"""
                            <h2>New Ophthalmologist Registration</h2>
                            <p>A new ophthalmologist has registered on the AURA screening system and requires credential verification.</p>
                            <ul>
                                <li><strong>Name:</strong> {request.FullName}</li>
                                <li><strong>Email:</strong> {request.Email}</li>
                                <li><strong>Loại hình làm việc:</strong> {request.EmploymentType}</li>
                                <li><strong>Giới thiệu:</strong> {request.Bio ?? "Không có"}</li>
                            </ul>
                            <p>Please review their submitted credentials (degrees and licenses/certificates) in the System Admin panel.</p>
                            <p>— AURA System</p>
                            """,
                            isHtml: true,
                            cancellationToken);
                    }
                }
            }
            catch (Exception adminEmailEx)
            {
                _logger.LogWarning(adminEmailEx,
                    "Failed to send admin notification email for new ophthalmologist: {Email}", request.Email);
            }

            _logger.LogInformation("Ophthalmologist registered: {Email}", request.Email);

            return Result<RegisterResponse>.Success(new RegisterResponse
            {
                UserId = user.Id,
                Email = user.Email!,
                Message = "Registration successful. Please check your email to confirm your account."
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error registering ophthalmologist: {Email}", request.Email);

            // Rollback all DB changes (User, Role, Ophthalmologist)
            try { await _unitOfWork.RollbackTransactionAsync(cancellationToken); }
            catch (Exception rbEx)
            {
                _logger.LogWarning(rbEx, "Failed to rollback transaction for: {Email}", request.Email);
            }

            // Compensating action: delete any files already uploaded to S3
            foreach (var url in uploadedFileUrls)
            {
                try { _fileStorageService.DeleteFile(url); }
                catch (Exception delEx)
                {
                    _logger.LogWarning(delEx, "Failed to cleanup uploaded file during rollback: {Url}", url);
                }
            }

            return Result<RegisterResponse>.Failure("An error occurred during registration");
        }
    }

    private static List<CredentialItemDto> NormalizeCredentials(RegisterOphthalmologistRequest request)
    {
        if (request.Degrees.Count > 0)
        {
            var merged = new List<CredentialItemDto>(request.Degrees.Count + request.Certificates.Count);

            merged.AddRange(request.Degrees.Select(d => new CredentialItemDto
            {
                Type = CertificateType.Degree,
                DegreeLevel = d.DegreeLevel,
                Name = d.Name,
                IssuingAuthority = d.IssuingAuthority,
                IssuedDate = d.IssuedDate,
                ExpiryDate = null,
                File = d.File
            }));

            merged.AddRange(request.Certificates.Select(c => new CredentialItemDto
            {
                Type = CertificateType.License,
                DegreeLevel = null,
                Name = c.Name,
                IssuingAuthority = c.IssuingAuthority,
                IssuedDate = c.IssuedDate,
                ExpiryDate = c.ExpiryDate,
                File = c.File
            }));

            return merged;
        }

        // Fallback for partially-updated FE clients that send licenses without type.
        return request.Certificates
            .Select(c =>
            {
                var type = c.Type;
                if (type == CertificateType.Degree && !c.DegreeLevel.HasValue && c.ExpiryDate.HasValue)
                {
                    type = CertificateType.License;
                }

                return new CredentialItemDto
                {
                    Type = type,
                    DegreeLevel = type == CertificateType.Degree ? c.DegreeLevel : null,
                    Name = c.Name,
                    IssuingAuthority = c.IssuingAuthority,
                    IssuedDate = c.IssuedDate,
                    ExpiryDate = type == CertificateType.Degree ? null : c.ExpiryDate,
                    File = c.File
                };
            })
            .ToList();
    }

    private static DateTime EnsureUtc(DateTime value)
    {
        return value.Kind switch
        {
            DateTimeKind.Utc => value,
            DateTimeKind.Local => value.ToUniversalTime(),
            DateTimeKind.Unspecified => DateTime.SpecifyKind(value, DateTimeKind.Utc),
            _ => DateTime.SpecifyKind(value, DateTimeKind.Utc)
        };
    }

    private static DateTime? EnsureUtc(DateTime? value)
    {
        return value.HasValue ? EnsureUtc(value.Value) : null;
    }

    /// <inheritdoc />
    public async Task<Result<LoginResponse>> GoogleLoginAsync(
        GoogleLoginRequest request,
        string? ipAddress,
        CancellationToken cancellationToken = default)
    {
        try
        {
            // Validate Google ID token
            var settings = new GoogleJsonWebSignature.ValidationSettings
            {
                Audience = new[] { _googleAuthSettings.ClientId }
            };

            GoogleJsonWebSignature.Payload payload;
            try
            {
                payload = await GoogleJsonWebSignature.ValidateAsync(request.Credential, settings);
            }
            catch (InvalidJwtException)
            {
                return Result<LoginResponse>.Unauthorized("Invalid Google token");
            }

            if (string.IsNullOrEmpty(payload.Email))
            {
                return Result<LoginResponse>.Failure("Google account does not have an email address");
            }

            var normalizedEmail = payload.Email.Trim();
            var user = await _userManager.FindByEmailAsync(normalizedEmail)
                ?? await _userManager.FindByLoginAsync(GoogleLoginProvider, payload.Subject);

            if (user != null)
            {
                // Existing user
                if (user.IsDeleted)
                {
                    return Result<LoginResponse>.Unauthorized("Account has been deleted");
                }

                if (!user.IsActive)
                {
                    return Result<LoginResponse>.Unauthorized("Account is deactivated. Please contact support.");
                }

                // Auto-confirm email for Google users if not yet confirmed
                if (!user.EmailConfirmed)
                {
                    user.EmailConfirmed = true;
                    await _userManager.UpdateAsync(user);
                }

                var linkResult = await LinkGoogleLoginAsync(user, payload.Subject);
                if (!linkResult.IsSuccess)
                {
                    return Result<LoginResponse>.Failure(
                        string.IsNullOrWhiteSpace(linkResult.ErrorMessage)
                            ? "Unable to link Google account"
                            : linkResult.ErrorMessage);
                }

                var providerAvatarResult = await UpsertProviderAvatarClaimAsync(user, payload.Picture);
                if (!providerAvatarResult.IsSuccess)
                {
                    _logger.LogWarning(
                        "Failed to update provider avatar claim for user {UserId}: {Error}",
                        user.Id,
                        providerAvatarResult.ErrorMessage);
                }

                // Check if 2FA is enabled
                if (await _userManager.GetTwoFactorEnabledAsync(user))
                {
                    _logger.LogInformation("2FA required for Google user: {Email}", payload.Email);
                    return Result<LoginResponse>.Success(LoginResponse.TwoFactorRequired(user.Id));
                }

                var authResponse = await CompleteLoginAsync(user, request.DeviceInfo, ipAddress, cancellationToken);
                return Result<LoginResponse>.Success(LoginResponse.Success(authResponse));
            }

            // New user — create Patient account
            await _unitOfWork.BeginTransactionAsync(cancellationToken);

            var newUser = new ApplicationUser
            {
                UserName = normalizedEmail,
                Email = normalizedEmail,
                FullName = payload.Name ?? normalizedEmail,
                AvatarUrl = null,
                EmailConfirmed = true // Google already verified the email
            };

            var createResult = await _userManager.CreateAsync(newUser);
            if (!createResult.Succeeded)
            {
                await _unitOfWork.RollbackTransactionAsync(cancellationToken);
                return Result<LoginResponse>.Failure(createResult.Errors.Select(e => e.Description));
            }

            // Add Google login provider info
            var addLoginResult = await LinkGoogleLoginAsync(newUser, payload.Subject);
            if (!addLoginResult.IsSuccess)
            {
                await _unitOfWork.RollbackTransactionAsync(cancellationToken);
                return Result<LoginResponse>.Failure(
                    string.IsNullOrWhiteSpace(addLoginResult.ErrorMessage)
                        ? "Unable to link Google account"
                        : addLoginResult.ErrorMessage);
            }

            var newProviderAvatarResult = await UpsertProviderAvatarClaimAsync(newUser, payload.Picture);
            if (!newProviderAvatarResult.IsSuccess)
            {
                await _unitOfWork.RollbackTransactionAsync(cancellationToken);
                return Result<LoginResponse>.Failure(
                    string.IsNullOrWhiteSpace(newProviderAvatarResult.ErrorMessage)
                        ? "Unable to persist provider avatar"
                        : newProviderAvatarResult.ErrorMessage);
            }

            await _identityService.AddToRoleAsync(newUser.Id, Roles.Patient);

            var patient = Patient.CreateRegistered(newUser.Id);
            await _patientRepository.AddAsync(patient, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            await _unitOfWork.CommitTransactionAsync(cancellationToken);

            _logger.LogInformation("New patient registered via Google: {Email}", payload.Email);

            var newAuthResponse = await CompleteLoginAsync(newUser, request.DeviceInfo, ipAddress, cancellationToken);
            return Result<LoginResponse>.Success(LoginResponse.Success(newAuthResponse));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during Google login");

            try { await _unitOfWork.RollbackTransactionAsync(cancellationToken); }
            catch { /* transaction may not have started */ }

            return Result<LoginResponse>.Failure("An error occurred during Google login");
        }
    }

    /// <inheritdoc />
    public async Task<Result<LoginResponse>> LoginAsync(
        LoginRequest request,
        string? ipAddress,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var user = await _userManager.FindByEmailAsync(request.Email);
            if (user == null || user.IsDeleted)
            {
                return Result<LoginResponse>.Unauthorized("Invalid email or password");
            }

            if (!user.IsActive)
            {
                return Result<LoginResponse>.Unauthorized("Account is deactivated. Please contact support.");
            }

            if (await _userManager.IsLockedOutAsync(user))
            {
                return Result<LoginResponse>.Unauthorized("Account is temporarily locked due to multiple failed attempts. Please try again later.");
            }

            var signInResult = await _signInManager.CheckPasswordSignInAsync(user, request.Password, lockoutOnFailure: true);

            if (signInResult.IsLockedOut)
            {
                return Result<LoginResponse>.Unauthorized("Account is temporarily locked due to multiple failed attempts. Please try again later.");
            }

            if (signInResult.IsNotAllowed)
            {
                return Result<LoginResponse>.Unauthorized("Please confirm your email before logging in.");
            }

            if (!signInResult.Succeeded && !signInResult.RequiresTwoFactor)
            {
                return Result<LoginResponse>.Unauthorized("Invalid email or password");
            }

            // 2FA may be requested by SignInManager pre-check or enabled at user level.
            if (signInResult.RequiresTwoFactor || await _userManager.GetTwoFactorEnabledAsync(user))
            {
                _logger.LogInformation("2FA required for user: {Email}", request.Email);
                return Result<LoginResponse>.Success(LoginResponse.TwoFactorRequired(user.Id));
            }

            // Complete login (no 2FA)
            var authResponse = await CompleteLoginAsync(user, request.DeviceInfo, ipAddress, cancellationToken);
            return Result<LoginResponse>.Success(LoginResponse.Success(authResponse));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during login: {Email}", request.Email);
            return Result<LoginResponse>.Failure("An error occurred during login");
        }
    }

    public async Task<Result<LookupAccountByCitizenIdResponse>> LookupAccountByCitizenIdAsync(
        string citizenId,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(citizenId))
        {
            return Result<LookupAccountByCitizenIdResponse>.Failure("Citizen ID is required.");
        }

        var normalizedCitizenId = citizenId.Trim();
        var user = await _userManager.Users
            .FirstOrDefaultAsync(u => u.CitizenId == normalizedCitizenId && !u.IsDeleted, cancellationToken);

        if (user == null)
        {
            return Result<LookupAccountByCitizenIdResponse>.Success(new LookupAccountByCitizenIdResponse
            {
                Exists = false,
                MaskedEmail = null
            });
        }

        return Result<LookupAccountByCitizenIdResponse>.Success(new LookupAccountByCitizenIdResponse
        {
            Exists = true,
            MaskedEmail = MaskEmailForLookup(user.Email)
        });
    }

    /// <inheritdoc />
    public async Task<Result<AuthResponse>> VerifyTwoFactorLoginAsync(
        VerifyTwoFactorRequest request,
        string? ipAddress,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var user = await _userManager.FindByIdAsync(request.UserId.ToString());
            if (user == null || user.IsDeleted || !user.IsActive)
            {
                return Result<AuthResponse>.Unauthorized("User not found or inactive");
            }

            if (!await _userManager.GetTwoFactorEnabledAsync(user))
            {
                return Result<AuthResponse>.Failure("Two-factor authentication is not enabled for this account");
            }

            bool isValidCode;
            if (request.UseRecoveryCode)
            {
                var result = await _identityService.VerifyRecoveryCodeAsync(user.Id, request.Code);
                isValidCode = result.Succeeded;

                if (isValidCode)
                {
                    _logger.LogWarning("Recovery code used for user: {UserId}", user.Id);
                }
            }
            else
            {
                isValidCode = await _identityService.VerifyTwoFactorCodeAsync(user.Id, request.Code);
            }

            if (!isValidCode)
            {
                _logger.LogWarning("Invalid 2FA code for user: {UserId}", user.Id);
                return Result<AuthResponse>.Unauthorized("Invalid verification code");
            }

            var authResponse = await CompleteLoginAsync(user, request.DeviceInfo, ipAddress, cancellationToken);
            return Result<AuthResponse>.Success(authResponse);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during 2FA verification for user: {UserId}", request.UserId);
            return Result<AuthResponse>.Failure("An error occurred during verification");
        }
    }

    private static string? MaskEmailForLookup(string? email)
    {
        if (string.IsNullOrWhiteSpace(email))
        {
            return null;
        }

        var parts = email.Split('@');
        if (parts.Length != 2)
        {
            return null;
        }

        var local = parts[0];
        var domain = parts[1];
        if (local.Length <= 2)
        {
            return $"**@{domain}";
        }

        return $"{local[..2]}***@{domain}";
    }

    /// <summary>
    /// Helper method to complete login and generate tokens.
    /// </summary>
    private async Task<AuthResponse> CompleteLoginAsync(
        ApplicationUser user,
        string? deviceInfo,
        string? ipAddress,
        CancellationToken cancellationToken)
    {
        var roles = await _userManager.GetRolesAsync(user);
        var permissions = await _identityService.GetUserPermissionsAsync(user.Id);

        var additionalClaims = await BuildProfileClaimsAsync(user.Id, roles, permissions, cancellationToken);

        var tokenResult = await _tokenService.GenerateAccessTokenAsync(
            user.Id,
            user.Email!,
            user.FullName,
            roles,
            additionalClaims);

        var refreshToken = _tokenService.GenerateRefreshToken();
        var refreshTokenHash = TokenService.HashToken(refreshToken);

        await _refreshTokenService.CreateRefreshTokenAsync(
            user.Id,
            refreshTokenHash,
            tokenResult.Jti,
            7, // 7 days
            deviceInfo,
            ipAddress,
            cancellationToken);

        await _identityService.UpdateLastLoginAsync(user.Id);

        _logger.LogInformation("User logged in: {Email}", user.Email);

        // Resolve role-specific profile entity
        Guid? roleId = null;
        string? employmentType = null;
        string? staffSubRoles = null;

        if (roles.Contains(Roles.Patient))
        {
            var patients = await _patientRepository.FindAsync(
                p => p.UserId == user.Id, cancellationToken);
            if (patients.Count > 0)
                roleId = patients[0].Id;
        }
        else if (roles.Contains(Roles.Ophthalmologist))
        {
            var doctors = await _ophthalmologistRepository.FindAsync(
                o => o.UserId == user.Id, cancellationToken);
            if (doctors.Count > 0)
            {
                roleId = doctors[0].Id;
                employmentType = doctors[0].EmploymentType.ToString();
            }
        }
        else if (roles.Contains(Roles.ClinicStaff))
        {
            var staff = await _clinicStaffRepository.GetByUserIdAsync(user.Id, cancellationToken);
            if (staff is not null)
            {
                roleId = staff.Id;
                staffSubRoles = staff.SubRoles;
            }
        }


        var providerAvatarUrl = await GetProviderAvatarUrlAsync(user);
        var uploadedAvatarUrl = user.AvatarUrl;

        return new AuthResponse
        {
            Succeeded = true,
            AccessToken = tokenResult.AccessToken,
            RefreshToken = refreshToken,
            ExpiresAt = tokenResult.ExpiresAt,
            User = new UserInfoResponse
            {
                Id = user.Id,
                Email = user.Email!,
                FullName = user.FullName,
                AvatarUrl = uploadedAvatarUrl ?? providerAvatarUrl,
                UploadedAvatarUrl = uploadedAvatarUrl,
                ProviderAvatarUrl = providerAvatarUrl,
                Roles = roles.ToArray(),
                EmailConfirmed = user.EmailConfirmed,
                RoleId = roleId,
                TwoFactorEnabled = await _userManager.GetTwoFactorEnabledAsync(user),
                EmploymentType = employmentType,
                StaffSubRoles = staffSubRoles,
                Permissions = permissions.ToArray()
            }
        };
    }

    /// <inheritdoc />
    public async Task<Result<AuthResponse>> RefreshTokenAsync(
        string accessToken,
        string refreshToken,
        string? ipAddress,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var userId = _tokenService.GetUserIdFromToken(accessToken);
            var jti = _tokenService.GetJtiFromToken(accessToken);

            if (userId == null || jti == null)
            {
                return Result<AuthResponse>.Unauthorized("Invalid access token");
            }

            var refreshTokenHash = TokenService.HashToken(refreshToken);
            var storedToken = await _refreshTokenService.GetByTokenHashAsync(refreshTokenHash, cancellationToken);

            if (storedToken == null)
            {
                return Result<AuthResponse>.Unauthorized("Invalid refresh token");
            }

            if (!storedToken.IsActive)
            {
                _logger.LogWarning("Refresh token reuse detected for user {UserId}", storedToken.UserId);
                await _refreshTokenService.RevokeTokenFamilyAsync(storedToken.Id, "token_reuse_detected", cancellationToken);
                return Result<AuthResponse>.Unauthorized("Token has been revoked. Please login again.");
            }

            if (storedToken.JwtId != jti || storedToken.UserId != userId)
            {
                return Result<AuthResponse>.Unauthorized("Token mismatch");
            }

            var user = await _userManager.FindByIdAsync(userId.Value.ToString());
            if (user == null || !user.IsActive || user.IsDeleted)
            {
                return Result<AuthResponse>.Unauthorized("User not found or inactive");
            }

            var roles = await _userManager.GetRolesAsync(user);
            var permissions = await _identityService.GetUserPermissionsAsync(user.Id);
            var additionalClaims = await BuildProfileClaimsAsync(user.Id, roles, permissions, cancellationToken);

            var tokenResult = await _tokenService.GenerateAccessTokenAsync(
                user.Id,
                user.Email!,
                user.FullName,
                roles,
                additionalClaims);

            var newRefreshToken = _tokenService.GenerateRefreshToken();
            var newRefreshTokenHash = TokenService.HashToken(newRefreshToken);

            await _refreshTokenService.RotateRefreshTokenAsync(
                storedToken.Id,
                newRefreshTokenHash,
                tokenResult.Jti,
                7,
                null,
                ipAddress,
                cancellationToken);

            _logger.LogInformation("Token refreshed for user: {UserId}", user.Id);

            // Resolve role-specific profile entity
            Guid? roleId = null;
            string? employmentType = null;
            string? staffSubRoles = null;

            if (roles.Contains(Roles.Patient))
            {
                var patients = await _patientRepository.FindAsync(
                    p => p.UserId == user.Id, cancellationToken);
                if (patients.Count > 0)
                    roleId = patients[0].Id;
            }
            else if (roles.Contains(Roles.Ophthalmologist))
            {
                var doctors = await _ophthalmologistRepository.FindAsync(
                    o => o.UserId == user.Id, cancellationToken);
                if (doctors.Count > 0)
                {
                    roleId = doctors[0].Id;
                    employmentType = doctors[0].EmploymentType.ToString();
                }
            }
            else if (roles.Contains(Roles.ClinicStaff))
            {
                var staff = await _clinicStaffRepository.GetByUserIdAsync(user.Id, cancellationToken);
                if (staff is not null)
                {
                    roleId = staff.Id;
                    staffSubRoles = staff.SubRoles;
                }
            }


            var providerAvatarUrl = await GetProviderAvatarUrlAsync(user);
            var uploadedAvatarUrl = user.AvatarUrl;

            return Result<AuthResponse>.Success(new AuthResponse
            {
                Succeeded = true,
                AccessToken = tokenResult.AccessToken,
                RefreshToken = newRefreshToken,
                ExpiresAt = tokenResult.ExpiresAt,
                User = new UserInfoResponse
                {
                    Id = user.Id,
                    Email = user.Email!,
                    FullName = user.FullName,
                    AvatarUrl = uploadedAvatarUrl ?? providerAvatarUrl,
                    UploadedAvatarUrl = uploadedAvatarUrl,
                    ProviderAvatarUrl = providerAvatarUrl,
                    Roles = roles.ToArray(),
                    EmailConfirmed = user.EmailConfirmed,
                    RoleId = roleId,
                    TwoFactorEnabled = await _userManager.GetTwoFactorEnabledAsync(user),
                    MustUpdateProfile = user.MustUpdateProfile,
                    EmploymentType = employmentType,
                    StaffSubRoles = staffSubRoles,
                    Permissions = permissions.ToArray()
                }
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error refreshing token");
            return Result<AuthResponse>.Failure("An error occurred while refreshing token");
        }
    }

    /// <inheritdoc />
    public async Task<Result> LogoutAsync(string refreshToken, CancellationToken cancellationToken = default)
    {
        try
        {
            var refreshTokenHash = TokenService.HashToken(refreshToken);
            var storedToken = await _refreshTokenService.GetByTokenHashAsync(refreshTokenHash, cancellationToken);

            if (storedToken != null)
            {
                await _refreshTokenService.RevokeTokenAsync(storedToken.Id, "logout", cancellationToken);
            }

            _logger.LogInformation("User logged out");
            return Result.Success();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during logout");
            return Result.Failure("An error occurred during logout");
        }
    }

    /// <inheritdoc />
    public async Task<Result> LogoutAllAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        try
        {
            await _refreshTokenService.RevokeAllUserTokensAsync(userId, "logout_all", cancellationToken);
            _logger.LogInformation("All tokens revoked for user: {UserId}", userId);
            return Result.Success();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during logout all");
            return Result.Failure("An error occurred during logout");
        }
    }

    /// <inheritdoc />
    public async Task<Result> ConfirmEmailAsync(string userId, string token, CancellationToken cancellationToken = default)
    {
        try
        {
            if (!Guid.TryParse(userId, out var userGuid))
            {
                return Result.Failure("Invalid user ID");
            }

            var user = await _identityService.GetUserByIdAsync(userGuid, cancellationToken);
            if (user == null)
            {
                return Result.NotFound("User not found");
            }

            var (succeeded, errors) = await _identityService.ConfirmEmailAsync(userGuid, token);
            if (!succeeded)
            {
                return Result.Failure(errors);
            }

            var roles = await _identityService.GetUserRolesAsync(userGuid);
            if (roles.Contains(Roles.Ophthalmologist))
            {
                var adminUsers = await _userManager.GetUsersInRoleAsync(Roles.SystemAdmin);
                foreach (var admin in adminUsers)
                {
                    await _notificationService.SendAsync(
                        admin.Id,
                        "Bác sĩ đã xác thực email",
                        $"Bác sĩ {user.FullName} đã xác thực email. Vui lòng kiểm tra hợp đồng.",
                        NotificationType.SystemAlert,
                        payload: new
                        {
                            action = "ophthalmologist_email_confirmed",
                            ophthalmologistUserId = user.Id,
                            emailConfirmed = true
                        },
                        cancellationToken: cancellationToken,
                        referenceId: user.Id);
                }
            }

            _logger.LogInformation("Email confirmed for user: {UserId}", userId);
            return Result.Success();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error confirming email for user: {UserId}", userId);
            return Result.Failure("An error occurred during email confirmation");
        }
    }

    /// <inheritdoc />
    public async Task<Result> ForgotPasswordAsync(string email, string resetUrlBase, CancellationToken cancellationToken = default)
    {
        try
        {
            var userDto = await _identityService.GetUserByEmailAsync(email, cancellationToken);

            if (userDto != null)
            {
                var resetToken = await _identityService.GeneratePasswordResetTokenAsync(userDto.Id);
                var resetLink = $"{resetUrlBase}?userId={userDto.Id}&token={Uri.EscapeDataString(resetToken)}";

                // Delegate to IEmailService
                await _emailService.SendPasswordResetAsync(userDto.Email, resetLink, cancellationToken);
                _logger.LogInformation("Password reset requested for: {Email}", email);
            }
            else
            {
                _logger.LogWarning("Password reset requested for non-existent email: {Email}", email);
            }

            // Always return success to prevent email enumeration
            return Result.Success();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during forgot password: {Email}", email);
            return Result.Failure("An error occurred while processing your request");
        }
    }

    /// <inheritdoc />
    public async Task<Result> ResetPasswordAsync(ResetPasswordRequest request, CancellationToken cancellationToken = default)
    {
        try
        {
            if (!Guid.TryParse(request.UserId, out var userGuid))
            {
                return Result.Failure("Invalid user ID");
            }

            var user = await _identityService.GetUserByIdAsync(userGuid, cancellationToken);
            if (user == null)
            {
                return Result.NotFound("User not found");
            }

            var (succeeded, errors) = await _identityService.ResetPasswordAsync(userGuid, request.Token, request.NewPassword);
            if (!succeeded)
            {
                return Result.Failure(errors);
            }

            // Revoke all refresh tokens for security
            await _refreshTokenService.RevokeAllUserTokensAsync(userGuid, "password_reset", cancellationToken);

            _logger.LogInformation("Password reset for user: {UserId}", request.UserId);
            return Result.Success();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error resetting password for user: {UserId}", request.UserId);
            return Result.Failure("An error occurred while resetting password");
        }
    }

    /// <inheritdoc />
    public async Task<Result<UserInfoResponse>> GetCurrentUserAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        try
        {
            var userDto = await _identityService.GetUserByIdAsync(userId, cancellationToken);
            if (userDto == null)
            {
                return Result<UserInfoResponse>.Unauthorized("User not found");
            }

            var userDetails = await _identityService.GetUserDetailsAsync(userId, cancellationToken);
            var identityUser = await _userManager.FindByIdAsync(userId.ToString());
            var roles = await _identityService.GetUserRolesAsync(userId);
            var permissions = await _identityService.GetUserPermissionsAsync(userId);
            var twoFactorEnabled = await _identityService.IsTwoFactorEnabledAsync(userId);

            // Resolve role-specific profile entity
            Guid? roleId = null;
            string? employmentType = null;
            string? staffSubRoles = null;

            if (roles.Contains(Roles.Patient))
            {
                var patients = await _patientRepository.FindAsync(
                    p => p.UserId == userId, cancellationToken);
                if (patients.Count > 0)
                    roleId = patients[0].Id;
            }
            else if (roles.Contains(Roles.Ophthalmologist))
            {
                var doctors = await _ophthalmologistRepository.FindAsync(
                    o => o.UserId == userId, cancellationToken);
                if (doctors.Count > 0)
                {
                    roleId = doctors[0].Id;
                    employmentType = doctors[0].EmploymentType.ToString();
                }
            }
            else if (roles.Contains(Roles.ClinicStaff))
            {
                var staff = await _clinicStaffRepository.GetByUserIdAsync(userId, cancellationToken);
                if (staff is not null)
                {
                    roleId = staff.Id;
                    staffSubRoles = staff.SubRoles;
                }
            }


            return Result<UserInfoResponse>.Success(new UserInfoResponse
            {
                Id = userDto.Id,
                Email = userDto.Email,
                FullName = userDto.FullName,
                AvatarUrl = userDetails?.AvatarUrl ?? await GetProviderAvatarUrlAsync(identityUser),
                UploadedAvatarUrl = userDetails?.AvatarUrl,
                ProviderAvatarUrl = await GetProviderAvatarUrlAsync(identityUser),
                Roles = roles.ToArray(),
                EmailConfirmed = userDto.EmailConfirmed,
                RoleId = roleId,
                TwoFactorEnabled = twoFactorEnabled,
                MustUpdateProfile = identityUser?.MustUpdateProfile ?? false,
                EmploymentType = employmentType,
                StaffSubRoles = staffSubRoles,
                Permissions = permissions.ToArray()
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting current user: {UserId}", userId);
            return Result<UserInfoResponse>.Failure("An error occurred while retrieving user information");
        }
    }

    /// <inheritdoc />
    public async Task<Result> ResendConfirmationAsync(string email, string confirmationUrlBase, CancellationToken cancellationToken = default)
    {
        try
        {
            var userDto = await _identityService.GetUserByEmailAsync(email, cancellationToken);

            if (userDto != null && !userDto.EmailConfirmed)
            {
                var confirmationToken = await _identityService.GenerateEmailConfirmationTokenAsync(userDto.Id);
                var confirmationLink = $"{confirmationUrlBase}?userId={userDto.Id}&token={Uri.EscapeDataString(confirmationToken)}";

                // Delegate to IEmailService
                await _emailService.SendEmailConfirmationAsync(userDto.Email, confirmationLink, cancellationToken);
                _logger.LogInformation("Confirmation email resent to: {Email}", email);
            }

            return Result.Success();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error resending confirmation email: {Email}", email);
            return Result.Failure("An error occurred while processing your request");
        }
    }
    private async Task<List<Claim>> BuildProfileClaimsAsync(
        Guid userId, IList<string> roles, IEnumerable<string> permissions, CancellationToken cancellationToken)
    {
        var claims = new List<Claim>();

        if (roles.Contains(Roles.Patient))
        {
            var patients = await _patientRepository.FindAsync(
                p => p.UserId == userId, cancellationToken);
            if (patients.Count > 0)
                claims.Add(new Claim("profile_id", patients[0].Id.ToString()));
        }
        else if (roles.Contains(Roles.Ophthalmologist))
        {
            var doctors = await _ophthalmologistRepository.FindAsync(
                o => o.UserId == userId, cancellationToken);
            if (doctors.Count > 0)
            {
                claims.Add(new Claim("profile_id", doctors[0].Id.ToString()));
            }
        }

        // Add permissions as claims
        foreach (var permission in permissions)
        {
            claims.Add(new Claim("permission", permission));
        }

        return claims;
    }

    private async Task<Guid?> ResolveRoleContextAsync(Guid userId, IList<string> roles, CancellationToken cancellationToken)
    {
        Guid? roleId = null;

        if (roles.Contains(Roles.Patient))
        {
            var patients = await _patientRepository.FindAsync(
                p => p.UserId == userId,
                cancellationToken);

            if (patients.Count > 0)
            {
                roleId = patients[0].Id;
            }
        }
        else if (roles.Contains(Roles.Ophthalmologist))
        {
            var doctors = await _ophthalmologistRepository.FindAsync(
                o => o.UserId == userId,
                cancellationToken);

            if (doctors.Count > 0)
            {
                roleId = doctors[0].Id;
            }
        }


        return roleId;
    }

    private async Task<Result> LinkGoogleLoginAsync(ApplicationUser user, string providerKey)
    {
        var linkedLogins = await _userManager.GetLoginsAsync(user);
        var existingGoogleLogin = linkedLogins.FirstOrDefault(login =>
            string.Equals(login.LoginProvider, GoogleLoginProvider, StringComparison.OrdinalIgnoreCase));

        if (existingGoogleLogin is not null &&
            string.Equals(existingGoogleLogin.ProviderKey, providerKey, StringComparison.Ordinal))
        {
            return Result.Success();
        }

        if (existingGoogleLogin is not null)
        {
            var removeResult = await _userManager.RemoveLoginAsync(
                user,
                existingGoogleLogin.LoginProvider,
                existingGoogleLogin.ProviderKey);

            if (!removeResult.Succeeded)
            {
                return Result.Failure(removeResult.Errors.Select(error => error.Description));
            }
        }

        var addResult = await _userManager.AddLoginAsync(
            user,
            new UserLoginInfo(GoogleLoginProvider, providerKey, GoogleLoginProvider));

        return addResult.Succeeded
            ? Result.Success()
            : Result.Failure(addResult.Errors.Select(error => error.Description));
    }

    private async Task<Result> UpsertProviderAvatarClaimAsync(ApplicationUser user, string? providerAvatarUrl)
    {
        if (string.IsNullOrWhiteSpace(providerAvatarUrl))
        {
            return Result.Success();
        }

        var claims = await _userManager.GetClaimsAsync(user);
        var existingClaim = claims.FirstOrDefault(claim =>
            string.Equals(claim.Type, ProviderAvatarClaimType, StringComparison.Ordinal));

        if (existingClaim is not null &&
            string.Equals(existingClaim.Value, providerAvatarUrl, StringComparison.Ordinal))
        {
            return Result.Success();
        }

        if (existingClaim is not null)
        {
            var removeResult = await _userManager.RemoveClaimAsync(user, existingClaim);
            if (!removeResult.Succeeded)
            {
                return Result.Failure(removeResult.Errors.Select(error => error.Description));
            }
        }

        var addResult = await _userManager.AddClaimAsync(
            user,
            new Claim(ProviderAvatarClaimType, providerAvatarUrl));

        return addResult.Succeeded
            ? Result.Success()
            : Result.Failure(addResult.Errors.Select(error => error.Description));
    }

    private async Task<string?> GetProviderAvatarUrlAsync(ApplicationUser? user)
    {
        if (user is null)
        {
            return null;
        }

        var claims = await _userManager.GetClaimsAsync(user);
        var providerAvatarClaim = claims.FirstOrDefault(claim =>
            string.Equals(claim.Type, ProviderAvatarClaimType, StringComparison.Ordinal));

        return string.IsNullOrWhiteSpace(providerAvatarClaim?.Value)
            ? null
            : providerAvatarClaim.Value;
    }
}
