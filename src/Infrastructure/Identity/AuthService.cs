using Application.Common.Constants;
using Application.Common.Interfaces;
using Application.Common.Models;
using Application.Common.Models.Auth;
using Domain.Common;
using Domain.Entities;
using Domain.Enums;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;

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
    private readonly IIdentityService _identityService;
    private readonly ITokenService _tokenService;
    private readonly IRefreshTokenService _refreshTokenService;
    private readonly IEmailService _emailService;
    private readonly IFileStorageService _fileStorageService;
    private readonly IRepository<Patient> _patientRepository;
    private readonly IRepository<Ophthalmologist> _ophthalmologistRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly ILogger<AuthService> _logger;

    public AuthService(
        IIdentityService identityService,
        ITokenService tokenService,
        IRefreshTokenService refreshTokenService,
        IEmailService emailService,
        IFileStorageService fileStorageService,
        IRepository<Patient> patientRepository,
        IRepository<Ophthalmologist> ophthalmologistRepository,
        IUnitOfWork unitOfWork,
        UserManager<ApplicationUser> userManager,
        ILogger<AuthService> logger)
    {
        _identityService = identityService;
        _tokenService = tokenService;
        _refreshTokenService = refreshTokenService;
        _emailService = emailService;
        _fileStorageService = fileStorageService;
        _patientRepository = patientRepository;
        _ophthalmologistRepository = ophthalmologistRepository;
        _unitOfWork = unitOfWork;
        _userManager = userManager;
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
                Gender = request.Gender.HasValue ? (Gender)request.Gender.Value : null
            };

            var createResult = await _userManager.CreateAsync(user, request.Password);
            if (!createResult.Succeeded)
            {
                await _unitOfWork.RollbackTransactionAsync(cancellationToken);
                return Result<RegisterResponse>.Failure(createResult.Errors.Select(e => e.Description));
            }

            // Add to Patient role
            await _identityService.AddToRoleAsync(user.Id, Roles.Patient);

            // Create Patient profile using Repository pattern
            var patient = new Patient(user.Id, null);
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
                FullName = request.FullName,
                OrganizationId = request.OrganizationId
            };

            var createResult = await _userManager.CreateAsync(user, request.Password);
            if (!createResult.Succeeded)
            {
                await _unitOfWork.RollbackTransactionAsync(cancellationToken);
                return Result<RegisterResponse>.Failure(createResult.Errors.Select(e => e.Description));
            }

            await _identityService.AddToRoleAsync(user.Id, Roles.Ophthalmologist);

            // Upload credential files to Supabase S3 if provided
            string? licenseUrl = null;
            string? degreeUrl = null;

            if (request.LicenseImage is { Length: > 0 })
            {
                await using var stream = request.LicenseImage.OpenReadStream();
                licenseUrl = await _fileStorageService.SaveFileAsync(
                    stream, request.LicenseImage.FileName, $"credentials/{user.Id}", cancellationToken);
                uploadedFileUrls.Add(licenseUrl);
            }

            if (request.DegreeImage is { Length: > 0 })
            {
                await using var stream = request.DegreeImage.OpenReadStream();
                degreeUrl = await _fileStorageService.SaveFileAsync(
                    stream, request.DegreeImage.FileName, $"credentials/{user.Id}", cancellationToken);
                uploadedFileUrls.Add(degreeUrl);
            }

            // Create Ophthalmologist profile with uploaded file URLs
            var ophthalmologist = new Ophthalmologist(
                user.Id, request.Bio, request.YearsOfExperience,
                request.Phone, licenseUrl, degreeUrl);
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

            if (!await _userManager.CheckPasswordAsync(user, request.Password))
            {
                return Result<LoginResponse>.Unauthorized("Invalid email or password");
            }

            if (!user.EmailConfirmed)
            {
                return Result<LoginResponse>.Unauthorized("Please confirm your email before logging in.");
            }

            // Check if 2FA is enabled
            if (await _userManager.GetTwoFactorEnabledAsync(user))
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

        var tokenResult = await _tokenService.GenerateAccessTokenAsync(
            user.Id,
            user.Email!,
            user.FullName,
            roles);

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
                Roles = roles.ToArray(),
                EmailConfirmed = user.EmailConfirmed,
                OrganizationId = user.OrganizationId,
                TwoFactorEnabled = await _userManager.GetTwoFactorEnabledAsync(user)
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
            var tokenResult = await _tokenService.GenerateAccessTokenAsync(
                user.Id,
                user.Email!,
                user.FullName,
                roles);

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
                    Roles = roles.ToArray(),
                    EmailConfirmed = user.EmailConfirmed,
                    OrganizationId = user.OrganizationId,
                    TwoFactorEnabled = await _userManager.GetTwoFactorEnabledAsync(user)
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

            var roles = await _identityService.GetUserRolesAsync(userId);
            var twoFactorEnabled = await _identityService.IsTwoFactorEnabledAsync(userId);

            return Result<UserInfoResponse>.Success(new UserInfoResponse
            {
                Id = userDto.Id,
                Email = userDto.Email,
                FullName = userDto.FullName,
                Roles = roles.ToArray(),
                EmailConfirmed = userDto.EmailConfirmed,
                OrganizationId = userDto.OrganizationId,
                TwoFactorEnabled = twoFactorEnabled
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
}
