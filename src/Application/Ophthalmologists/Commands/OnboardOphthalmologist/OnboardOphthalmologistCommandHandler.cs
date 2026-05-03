using Application.Common.Interfaces;
using Application.Common.Models;
using Domain.Common;
using Domain.Entities.Users;
using Domain.Enums;
using Domain.Repositories;
using Microsoft.Extensions.Logging;
using System.Globalization;

namespace Application.Ophthalmologists.Commands.OnboardOphthalmologist;

public class OnboardOphthalmologistCommandHandler : ICommandHandler<OnboardOphthalmologistCommand, bool>
{
    private readonly IIdentityService _identityService;
    private readonly IOphthalmologistRepository _ophthalmologistRepository;
    private readonly IFileStorageService _fileStorageService;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUserService _currentUserService;
    private readonly ILogger<OnboardOphthalmologistCommandHandler> _logger;

    public OnboardOphthalmologistCommandHandler(
        IIdentityService identityService,
        IOphthalmologistRepository ophthalmologistRepository,
        IFileStorageService fileStorageService,
        IUnitOfWork unitOfWork,
        ICurrentUserService currentUserService,
        ILogger<OnboardOphthalmologistCommandHandler> logger)
    {
        _identityService = identityService;
        _ophthalmologistRepository = ophthalmologistRepository;
        _fileStorageService = fileStorageService;
        _unitOfWork = unitOfWork;
        _currentUserService = currentUserService;
        _logger = logger;
    }

    public async Task<Result<bool>> Handle(OnboardOphthalmologistCommand request, CancellationToken cancellationToken)
    {
        var userId = _currentUserService.UserId;
        Console.WriteLine($"[DEBUG] Handler Started: UserId={userId}");
        if (!userId.HasValue)
            return Result<bool>.Failure("Unauthorized access.");

        var ophthalmologist = await _ophthalmologistRepository.GetByUserIdAsync(userId.Value, cancellationToken);
        if (ophthalmologist == null)
        {
            Console.WriteLine("[DEBUG] Ophthalmologist profile not found in DB.");
            return Result<bool>.NotFound("Ophthalmologist profile not found.");
        }

        await _unitOfWork.BeginTransactionAsync(cancellationToken);

        try
        {
            // Parse DOB
            DateTime? dob = null;
            if (!string.IsNullOrWhiteSpace(request.DateOfBirth))
            {
                if (DateTime.TryParse(request.DateOfBirth, out var parsedDob))
                    dob = DateTime.SpecifyKind(parsedDob, DateTimeKind.Utc);
            }

            Console.WriteLine($"[DEBUG] Updating Profile: FullName={request.FullName}, CitizenId={request.CitizenId}");
            var (userSucceeded, userErrors) = await _identityService.UpdateUserProfileAsync(
                userId.Value,
                request.FullName,
                request.Phone,
                dob,
                request.Gender,
                request.Address,
                request.CitizenId,
                cancellationToken);

            if (!userSucceeded)
            {
                Console.WriteLine($"[DEBUG] UpdateUserProfileAsync Failed: {string.Join(", ", userErrors)}");
                await _unitOfWork.RollbackTransactionAsync(cancellationToken);
                return Result<bool>.Failure(userErrors);
            }

            // 2. Change Password if provided
            if (!string.IsNullOrWhiteSpace(request.NewPassword) && !string.IsNullOrWhiteSpace(request.CurrentPassword))
            {
                var (pwdSucceeded, pwdErrors) = await _identityService.ChangePasswordAsync(
                    userId.Value,
                    request.CurrentPassword,
                    request.NewPassword,
                    cancellationToken);

                if (!pwdSucceeded)
                {
                    await _unitOfWork.RollbackTransactionAsync(cancellationToken);
                    return Result<bool>.Failure(pwdErrors);
                }
            }

            // 3. Update Avatar if provided
            if (!string.IsNullOrWhiteSpace(request.AvatarUrl))
            {
                await _identityService.UpdateAvatarUrlAsync(userId.Value, request.AvatarUrl, cancellationToken);
            }

            // 4. Update Ophthalmologist Bio
            ophthalmologist.UpdateProfile(request.Bio);

            // 5. Handle Degrees
            foreach (var deg in request.Degrees)
            {
                string? fileUrl = null;
                if (deg.File != null && deg.File.Length > 0)
                {
                    await using var stream = deg.File.OpenReadStream();
                    fileUrl = await _fileStorageService.SaveFileAsync(
                        stream,
                        deg.File.FileName,
                        $"ophthalmologists/credentials/{userId.Value}",
                        cancellationToken);
                }

                DateTime issuedDate = DateTime.UtcNow;
                if (DateTime.TryParse(deg.IssuedDate, out var parsedDate))
                    issuedDate = DateTime.SpecifyKind(parsedDate, DateTimeKind.Utc);

                var degreeCert = new Certificate(
                    ophthalmologist.Id,
                    CertificateType.Degree,
                    deg.Name,
                    deg.DegreeLevel,
                    null,
                    DateTime.SpecifyKind(issuedDate, DateTimeKind.Utc),
                    null,
                    fileUrl,
                    null,
                    null,
                    deg.IssuingInstitution
                );
                ophthalmologist.AddCertificate(degreeCert);
            }

            // 6. Handle Licenses
            foreach (var lic in request.Licenses)
            {
                string? fileUrl = null;
                if (lic.File != null && lic.File.Length > 0)
                {
                    await using var stream = lic.File.OpenReadStream();
                    fileUrl = await _fileStorageService.SaveFileAsync(
                        stream,
                        lic.File.FileName,
                        $"ophthalmologists/credentials/{userId.Value}",
                        cancellationToken);
                }

                DateTime issuedDate = DateTime.UtcNow;
                if (DateTime.TryParse(lic.IssuedDate, out var parsedId))
                    issuedDate = DateTime.SpecifyKind(parsedId, DateTimeKind.Utc);

                DateTime? expDate = null;
                if (!string.IsNullOrWhiteSpace(lic.ExpirationDate) && DateTime.TryParse(lic.ExpirationDate, out var parsedEd))
                    expDate = DateTime.SpecifyKind(parsedEd, DateTimeKind.Utc);

                var licenseCert = new Certificate(
                    ophthalmologist.Id,
                    CertificateType.License,
                    lic.Name,
                    null,
                    lic.IssuingAuthority,
                    DateTime.SpecifyKind(issuedDate, DateTimeKind.Utc),
                    expDate.HasValue ? DateTime.SpecifyKind(expDate.Value, DateTimeKind.Utc) : null,
                    fileUrl,
                    lic.LicenseNumber,
                    lic.ScopeOfPractice,
                    null
                );
                licenseCert.GetType().GetProperty("IssuingInstitution")?.SetValue(licenseCert, null); // Just to be clean
                ophthalmologist.AddCertificate(licenseCert);
            }

            // 7. Save role-specific changes
            await _ophthalmologistRepository.UpdateAsync(ophthalmologist, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            // 8. Clear MustUpdateProfile flag
            var (clearSucceeded, clearErrors) = await _identityService.ClearMustUpdateProfileFlagAsync(userId.Value);
            if (!clearSucceeded)
            {
                await _unitOfWork.RollbackTransactionAsync(cancellationToken);
                return Result<bool>.Failure(clearErrors);
            }

            await _unitOfWork.CommitTransactionAsync(cancellationToken);
            return Result<bool>.Success(true);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[DEBUG] Exception in Handler: {ex.Message}");
            Console.WriteLine(ex.StackTrace);
            _logger.LogError(ex, "Error during ophthalmologist onboarding for user {UserId}", userId);
            await _unitOfWork.RollbackTransactionAsync(cancellationToken);
            return Result<bool>.Failure("An internal error occurred during onboarding.");
        }
    }
}
