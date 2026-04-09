using Application.Common.Interfaces;
using Application.Common.Models;
using Domain.Common;
using Domain.Entities.Users;
using Domain.Repositories;
using Microsoft.Extensions.Logging;

namespace Application.OrganisationPatients.Commands.UpdateOrganisationPatientContact;

public class UpdateOrganisationPatientContactCommandHandler
    : ICommandHandler<UpdateOrganisationPatientContactCommand, Guid>
{
    private readonly IIdentityService _identityService;
    private readonly IRepository<Patient> _patientRepository;
    private readonly IRepository<Organisation> _organisationRepository;
    private readonly IOrganisationPatientsRepository _organisationPatientsRepository;
    private readonly ILogger<UpdateOrganisationPatientContactCommandHandler> _logger;

    public UpdateOrganisationPatientContactCommandHandler(
        IIdentityService identityService,
        IRepository<Patient> patientRepository,
        IRepository<Organisation> organisationRepository,
        IOrganisationPatientsRepository organisationPatientsRepository,
        ILogger<UpdateOrganisationPatientContactCommandHandler> logger)
    {
        _identityService = identityService;
        _patientRepository = patientRepository;
        _organisationRepository = organisationRepository;
        _organisationPatientsRepository = organisationPatientsRepository;
        _logger = logger;
    }

    public async Task<Result<Guid>> Handle(
        UpdateOrganisationPatientContactCommand request,
        CancellationToken cancellationToken)
    {
        var isManagedByOrg = await _organisationPatientsRepository.IsPatientManagedByOrganisationAdminAsync(
            request.OrgAdminUserId,
            request.PatientId,
            cancellationToken);

        if (!isManagedByOrg)
        {
            return Result<Guid>.Forbidden("You are not allowed to update this patient profile");
        }

        var patient = await _patientRepository.GetByIdAsync(request.PatientId, cancellationToken);
        if (patient is null)
        {
            return Result<Guid>.NotFound("Patient profile not found");
        }

        var userDetails = await _identityService.GetUserDetailsAsync(patient.UserId, cancellationToken);
        if (userDetails is null)
        {
            return Result<Guid>.NotFound("Patient user not found");
        }

        var requestedPhoneUpdate = request.PhoneNumber is not null;
        var requestedEmailUpdate = request.Email is not null;
        var requestedAddressUpdate = request.Address is not null;

        var phoneNumber = requestedPhoneUpdate
            ? TrimToNull(request.PhoneNumber)
            : userDetails.PhoneNumber;

        var email = requestedEmailUpdate
            ? TrimToNull(request.Email)
            : userDetails.Email;

        var address = requestedAddressUpdate
            ? TrimToNull(request.Address)
            : userDetails.Address;

        if (requestedEmailUpdate && email is null)
        {
            return Result<Guid>.Failure("Email cannot be empty");
        }

        if (phoneNumber is not null && !AreEquivalentPhoneNumber(phoneNumber, userDetails.PhoneNumber))
        {
            var organisation = (await _organisationRepository.FindAsync(
                    o => o.OwnerId == request.OrgAdminUserId,
                    cancellationToken))
                .FirstOrDefault();

            if (organisation is null)
            {
                return Result<Guid>.NotFound("Organisation not found");
            }

            var isPhoneInUse = await _identityService.IsPhoneNumberInUseByOrganizationAsync(
                organisation.Id,
                phoneNumber,
                cancellationToken);

            if (isPhoneInUse)
            {
                return Result<Guid>.Conflict("Phone number already exists in this organisation");
            }
        }

        if (requestedEmailUpdate
            && email is not null
            && !string.Equals(email, userDetails.Email, StringComparison.OrdinalIgnoreCase))
        {
            var existingUser = await _identityService.GetUserByEmailAsync(email, cancellationToken);
            if (existingUser is not null && existingUser.Id != patient.UserId)
            {
                return Result<Guid>.Conflict("Email already exists");
            }

            var (emailUpdated, emailErrors) = await _identityService.UpdateUserEmailAsync(
                patient.UserId,
                email,
                cancellationToken);

            if (!emailUpdated)
            {
                return Result<Guid>.Failure(emailErrors);
            }
        }

        int? currentGender = userDetails.Gender.HasValue
            ? (int)userDetails.Gender.Value
            : null;

        var (profileUpdated, profileErrors) = await _identityService.UpdateUserProfileAsync(
            patient.UserId,
            userDetails.FullName,
            phoneNumber,
            userDetails.DateOfBirth,
            currentGender,
            address,
            userDetails.CitizenId,
            cancellationToken);

        if (!profileUpdated)
        {
            return Result<Guid>.Failure(profileErrors);
        }

        _logger.LogInformation(
            "OrgAdmin {OrgAdminId} updated contact for patient {PatientId}",
            request.OrgAdminUserId,
            request.PatientId);

        return Result<Guid>.Success(request.PatientId);
    }

    private static bool AreEquivalentPhoneNumber(string incomingPhone, string? existingPhone)
    {
        return NormalizePhone(incomingPhone) == NormalizePhone(existingPhone);
    }

    private static string NormalizePhone(string? phoneNumber)
    {
        if (string.IsNullOrWhiteSpace(phoneNumber))
        {
            return string.Empty;
        }

        return new string(phoneNumber.Where(char.IsDigit).ToArray());
    }

    private static string? TrimToNull(string? value)
    {
        return string.IsNullOrWhiteSpace(value)
            ? null
            : value.Trim();
    }
}