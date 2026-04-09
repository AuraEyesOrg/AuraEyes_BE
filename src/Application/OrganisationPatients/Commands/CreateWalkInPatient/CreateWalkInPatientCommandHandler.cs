using System.Security.Cryptography;
using Application.Common.Interfaces;
using Application.Common.Models;
using Domain.Common;
using Domain.Entities.Users;
using Microsoft.Extensions.Logging;

namespace Application.OrganisationPatients.Commands.CreateWalkInPatient;

public class CreateWalkInPatientCommandHandler : ICommandHandler<CreateWalkInPatientCommand, Guid>
{
    private readonly IIdentityService _identityService;
    private readonly IRepository<Patient> _patientRepository;
    private readonly IRepository<Organisation> _orgRepository;
    private readonly ICurrentUserService _currentUserService;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<CreateWalkInPatientCommandHandler> _logger;

    public CreateWalkInPatientCommandHandler(
        IIdentityService identityService,
        IRepository<Patient> patientRepository,
        IRepository<Organisation> orgRepository,
        ICurrentUserService currentUserService,
        IUnitOfWork unitOfWork,
        ILogger<CreateWalkInPatientCommandHandler> logger)
    {
        _identityService = identityService;
        _patientRepository = patientRepository;
        _orgRepository = orgRepository;
        _currentUserService = currentUserService;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<Result<Guid>> Handle(CreateWalkInPatientCommand request, CancellationToken cancellationToken)
    {
        var adminId = _currentUserService.UserId;
        if (adminId is null)
        {
            return Result<Guid>.Unauthorized("User not authenticated");
        }

        var orgs = await _orgRepository.FindAsync(o => o.OwnerId == adminId.Value, cancellationToken);
        var org = orgs.FirstOrDefault();

        if (org is null)
        {
            return Result<Guid>.NotFound("Organisation not found");
        }

        var phoneNumber = string.IsNullOrWhiteSpace(request.PhoneNumber)
            ? null
            : request.PhoneNumber.Trim();

        if (phoneNumber is not null)
        {
            var isPhoneInUse = await _identityService.IsPhoneNumberInUseByOrganizationAsync(
                org.Id,
                phoneNumber,
                cancellationToken);

            if (isPhoneInUse)
            {
                return Result<Guid>.Conflict("Phone number already exists in this organisation");
            }
        }

        var citizenId = string.IsNullOrWhiteSpace(request.CitizenId)
            ? null
            : request.CitizenId.Trim();

        var address = string.IsNullOrWhiteSpace(request.Address)
            ? null
            : request.Address.Trim();

        if (citizenId is not null)
        {
            var isCitizenIdInUse = await _identityService.IsCitizenIdInUseByOrganizationAsync(
                org.Id,
                citizenId,
                cancellationToken);

            if (isCitizenIdInUse)
            {
                return Result<Guid>.Conflict("Citizen ID already exists in this organisation");
            }
        }

        var uniqueSuffix = Guid.NewGuid().ToString("N").Substring(0, 8);
        var email = string.IsNullOrWhiteSpace(request.Email)
            ? $"walkin_{uniqueSuffix}@auraeyes.local"
            : request.Email;

        var password = GenerateStrongRandomPassword();

        int? genderId = string.IsNullOrWhiteSpace(request.Gender) ? null
            : request.Gender.StartsWith("M", StringComparison.OrdinalIgnoreCase) ? 1
            : request.Gender.StartsWith("F", StringComparison.OrdinalIgnoreCase) ? 2 : 3;

        var userProfile = new UserProfileWalkInDto(
            FullName: request.FullName,
            PhoneNumber: phoneNumber,
            DateOfBirth: request.DateOfBirth,
            Gender: genderId,
            Address: address,
            AvatarUrl: null,
            CitizenId: citizenId
        );

        var createResult = await _identityService.CreateUserWalkInPatientAsync(
            email: email,
            password: password,
            fullName: request.FullName,
            role: "Patient",
            organizationId: org.Id,
            userProfile: userProfile,
            cancellationToken: cancellationToken);

        if (!createResult.Succeeded)
        {
            return Result<Guid>.Failure(
                message: createResult.Errors[0]
            );
        }

        try
        {
            var userId = createResult.UserId!.Value;

            var patient = new Patient(userId);
            patient.UpdateProfile(null, null);

            await _patientRepository.AddAsync(patient, cancellationToken);

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("Walk-in patient {PatientId} created by OrgAdmin {AdminId}", userId, adminId);

            return Result<Guid>.Success(patient.Id);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating walk-in patient profile");
            return Result<Guid>.Failure($"Failed to create patient profile: {ex.Message}");
        }
    }

    private static string GenerateStrongRandomPassword(int length = 20)
    {
        const string lowercase = "abcdefghijklmnopqrstuvwxyz";
        const string uppercase = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
        const string digits = "0123456789";
        const string symbols = "!@#$%^&*()-_=+[]{}<>?";

        if (length < 8)
        {
            throw new ArgumentOutOfRangeException(nameof(length), "Password length must be at least 8.");
        }

        var allCharacters = string.Concat(lowercase, uppercase, digits, symbols);
        var passwordChars = new List<char>(length)
        {
            lowercase[RandomNumberGenerator.GetInt32(lowercase.Length)],
            uppercase[RandomNumberGenerator.GetInt32(uppercase.Length)],
            digits[RandomNumberGenerator.GetInt32(digits.Length)],
            symbols[RandomNumberGenerator.GetInt32(symbols.Length)]
        };

        while (passwordChars.Count < length)
        {
            passwordChars.Add(allCharacters[RandomNumberGenerator.GetInt32(allCharacters.Length)]);
        }

        for (var i = passwordChars.Count - 1; i > 0; i--)
        {
            var swapIndex = RandomNumberGenerator.GetInt32(i + 1);
            (passwordChars[i], passwordChars[swapIndex]) = (passwordChars[swapIndex], passwordChars[i]);
        }

        return new string(passwordChars.ToArray());
    }
}
