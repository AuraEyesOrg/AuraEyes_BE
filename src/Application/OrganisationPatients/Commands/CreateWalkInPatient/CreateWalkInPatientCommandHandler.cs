using Application.Common.Interfaces;
using Application.Common.Models;
using Application.Common.Constants;
using Domain.Common;
using Domain.Entities.Users;
using Microsoft.Extensions.Logging;

namespace Application.OrganisationPatients.Commands.CreateWalkInPatient;

/// <summary>
/// Creates a walk-in patient:
///   1. Always creates an ApplicationUser (Identity) + a Patient profile.
///   2. If the receptionist provides a real email  → use it, send credentials by email.
///   3. If no email is provided                   → generate a fake local email, return credentials in the response.
/// Result: exactly 1 User + 1 Patient per call.
/// </summary>
public class CreateWalkInPatientCommandHandler : ICommandHandler<CreateWalkInPatientCommand, CreateWalkInPatientResponse>
{
    private readonly IRepository<Patient> _patientRepository;
    private readonly IIdentityService _identityService;
    private readonly IEmailService _emailService;
    private readonly ICurrentUserService _currentUserService;
    private readonly ICurrentUserOrganisationService _currentUserOrganisationService;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<CreateWalkInPatientCommandHandler> _logger;

    public CreateWalkInPatientCommandHandler(
        IRepository<Patient> patientRepository,
        IIdentityService identityService,
        IEmailService emailService,
        ICurrentUserService currentUserService,
        ICurrentUserOrganisationService currentUserOrganisationService,
        IUnitOfWork unitOfWork,
        ILogger<CreateWalkInPatientCommandHandler> logger)
    {
        _patientRepository = patientRepository;
        _identityService = identityService;
        _emailService = emailService;
        _currentUserService = currentUserService;
        _currentUserOrganisationService = currentUserOrganisationService;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<Result<CreateWalkInPatientResponse>> Handle(CreateWalkInPatientCommand request, CancellationToken cancellationToken)
    {
        var adminId = _currentUserService.UserId;
        if (adminId is null)
        {
            return Result<CreateWalkInPatientResponse>.Unauthorized("User not authenticated");
        }

        // ── Resolve the organisation the receptionist belongs to ──
        var organisationId = await _currentUserOrganisationService.GetOrganisationIdAsync(adminId.Value, cancellationToken);

        // ── Normalise optional string fields ──

        var phoneNumber = string.IsNullOrWhiteSpace(request.PhoneNumber)
            ? null
            : request.PhoneNumber.Trim();

        var citizenId = string.IsNullOrWhiteSpace(request.CitizenId)
            ? null
            : request.CitizenId.Trim();

        var address = string.IsNullOrWhiteSpace(request.Address)
            ? null
            : request.Address.Trim();

        var email = string.IsNullOrWhiteSpace(request.Email)
            ? null
            : request.Email.Trim().ToLowerInvariant();

        // ── Parse gender (1=Male, 2=Female, 3=Other) ──

        int? genderId = string.IsNullOrWhiteSpace(request.Gender) ? null
            : request.Gender.StartsWith("M", StringComparison.OrdinalIgnoreCase) ? 1
            : request.Gender.StartsWith("F", StringComparison.OrdinalIgnoreCase) ? 2 : 3;

        // ── Duplicate checks ──

        if (!string.IsNullOrWhiteSpace(citizenId))
        {
            var existingUserByCitizenId = await _identityService.GetUserByCitizenIdAsync(citizenId!, cancellationToken);
            if (existingUserByCitizenId != null)
            {
                return Result<CreateWalkInPatientResponse>.Failure("Citizen ID already linked to an existing account.");
            }
        }

        if (!string.IsNullOrWhiteSpace(email))
        {
            var existingUserByEmail = await _identityService.GetUserByEmailAsync(email, cancellationToken);
            if (existingUserByEmail != null)
            {
                return Result<CreateWalkInPatientResponse>.Failure("Email already linked to an existing account.");
            }
        }

        // ── Decide login email ──
        // Real email provided by receptionist → use it (credentials will be emailed).
        // No email → generate a disposable fake local address; credentials returned in response only.

        var isGeneratedEmail = string.IsNullOrWhiteSpace(email);
        var loginEmail = isGeneratedEmail
            ? await GenerateUniqueWalkInEmailAsync(citizenId, cancellationToken)
            : email!;
        var temporaryPassword = GenerateTemporaryPassword();

        await _unitOfWork.BeginTransactionAsync(cancellationToken);

        try
        {
            // ── Step 1: Create ApplicationUser (Identity) + assign Patient role ──

            var createUserResult = await _identityService.CreateUserWalkInPatientAsync(
                email: loginEmail,
                password: temporaryPassword,
                fullName: request.FullName.Trim(),
                role: Roles.Patient,
                organizationId: null,          // walk-in patients are not tied to an org at user level
                userProfile: new UserProfileWalkInDto(
                    FullName: request.FullName.Trim(),
                    PhoneNumber: phoneNumber,
                    DateOfBirth: request.DateOfBirth,
                    Gender: genderId,
                    Address: address,
                    AvatarUrl: null,
                    CitizenId: citizenId),
                cancellationToken: cancellationToken);

            if (!createUserResult.Succeeded || createUserResult.UserId is null)
            {
                await _unitOfWork.RollbackTransactionAsync(cancellationToken);
                return Result<CreateWalkInPatientResponse>.Failure(
                    createUserResult.Errors.Length > 0
                        ? createUserResult.Errors
                        : ["Failed to create patient account."]);
            }

            var userId = createUserResult.UserId.Value;

            // ── Step 2: Email confirmation strategy ──
            // • Generated/fake email → auto-confirm immediately (not deliverable; staff hands credentials in person).
            // • Real email provided  → leave unconfirmed; patient must click the link sent to their inbox.

            if (isGeneratedEmail)
            {
                var autoConfirmResult = await _identityService.ConfirmEmailAsync(
                    userId,
                    await _identityService.GenerateEmailConfirmationTokenAsync(userId));

                if (!autoConfirmResult.Succeeded)
                {
                    await _unitOfWork.RollbackTransactionAsync(cancellationToken);
                    return Result<CreateWalkInPatientResponse>.Failure(autoConfirmResult.Errors);
                }
            }

            // ── Step 3: Create Patient profile (1 User → 1 Patient) ──

            var patient = Patient.CreateRegistered(userId);
            await _patientRepository.AddAsync(patient, cancellationToken);

            await _unitOfWork.SaveChangesAsync(cancellationToken);
            await _unitOfWork.CommitTransactionAsync(cancellationToken);

            // ── Step 4 (best-effort): Send emails AFTER commit — failures do not roll back ──

            var emailSent = false;
            if (!isGeneratedEmail)
            {
                try
                {
                    // 4a. Credentials email — so patient knows how to log in
                    var credentialsSubject = "[AURA] Tài khoản bệnh nhân AuraEyes";
                    var credentialsBody =
$"""
Xin chào {request.FullName.Trim()},

Tài khoản bệnh nhân của bạn đã được tiếp tân tạo tại AuraEyes.

Email đăng nhập : {loginEmail}
Mật khẩu tạm   : {temporaryPassword}

Vui lòng đổi mật khẩu ngay sau lần đầu đăng nhập.

Trân trọng,
AuraEyes
""";
                    await _emailService.SendAsync(loginEmail, credentialsSubject, credentialsBody, isHtml: false, cancellationToken);

                    // 4b. Email confirmation link — patient must verify before logging in
                    if (!string.IsNullOrWhiteSpace(request.ConfirmationUrlBase))
                    {
                        var confirmToken = await _identityService.GenerateEmailConfirmationTokenAsync(userId);
                        var confirmLink = $"{request.ConfirmationUrlBase}?userId={userId}&token={Uri.EscapeDataString(confirmToken)}";
                        await _emailService.SendEmailConfirmationAsync(loginEmail, confirmLink, cancellationToken);
                    }

                    emailSent = true;
                }
                catch (Exception emailEx)
                {
                    _logger.LogWarning(
                        emailEx,
                        "Failed to send walk-in account email to {Email}. Credentials must be delivered manually.",
                        loginEmail);
                }
            }

            _logger.LogInformation(
                "Walk-in patient created. PatientId={PatientId}, UserId={UserId}, CreatedBy={AdminId}, " +
                "OrganisationId={OrgId}, IsGeneratedEmail={IsGeneratedEmail}",
                patient.Id, userId, adminId, organisationId, isGeneratedEmail);

            return Result<CreateWalkInPatientResponse>.Success(new CreateWalkInPatientResponse
            {
                PatientId = patient.Id,
                UserId = userId,
                LoginEmail = loginEmail,
                IsGeneratedEmail = isGeneratedEmail,
                EmailSent = emailSent,
                // Return temp password only when: no real email was provided, OR email send failed
                TemporaryPassword = isGeneratedEmail || !emailSent ? temporaryPassword : null
            });
        }
        catch (Exception ex)
        {
            await _unitOfWork.RollbackTransactionAsync(cancellationToken);
            _logger.LogError(ex, "Error creating walk-in patient profile");
            return Result<CreateWalkInPatientResponse>.Failure($"Failed to create patient profile: {ex.Message}");
        }
    }

    // ── Helpers ──

    private async Task<string> GenerateUniqueWalkInEmailAsync(string? citizenId, CancellationToken cancellationToken)
    {
        var seed = string.IsNullOrWhiteSpace(citizenId)
            ? Guid.NewGuid().ToString("N")[..12]
            : citizenId;

        var sanitizedSeed = new string(seed.Where(char.IsLetterOrDigit).ToArray());
        if (string.IsNullOrWhiteSpace(sanitizedSeed))
        {
            sanitizedSeed = Guid.NewGuid().ToString("N")[..12];
        }

        var candidate = $"walkin.{sanitizedSeed}@patient.aura.local";
        var existing = await _identityService.GetUserByEmailAsync(candidate, cancellationToken);
        if (existing == null)
        {
            return candidate;
        }

        // Append random suffix to avoid collision
        var suffix = Guid.NewGuid().ToString("N")[..6];
        return $"walkin.{sanitizedSeed}.{suffix}@patient.aura.local";
    }

    private static string GenerateTemporaryPassword()
    {
        // Satisfies typical Identity password rules: upper, lower, digit, special
        return $"Aura@{Guid.NewGuid():N}"[..14] + "!";
    }
}
