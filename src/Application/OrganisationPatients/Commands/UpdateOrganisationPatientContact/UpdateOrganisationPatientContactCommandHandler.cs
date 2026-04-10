using Application.Common.Interfaces;
using Application.Common.Models;
using Domain.Common;
using Domain.Entities.Users;
using Domain.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Application.OrganisationPatients.Commands.UpdateOrganisationPatientContact;

public class UpdateOrganisationPatientContactCommandHandler
    : ICommandHandler<UpdateOrganisationPatientContactCommand, Guid>
{
    private readonly IRepository<Patient> _patientRepository;
    private readonly IRepository<Organisation> _organisationRepository;
    private readonly IRepository<OrganisationPatientLink> _organisationPatientLinkRepository;
    private readonly IOrganisationPatientsRepository _organisationPatientsRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<UpdateOrganisationPatientContactCommandHandler> _logger;

    public UpdateOrganisationPatientContactCommandHandler(
        IRepository<Patient> patientRepository,
        IRepository<Organisation> organisationRepository,
        IRepository<OrganisationPatientLink> organisationPatientLinkRepository,
        IOrganisationPatientsRepository organisationPatientsRepository,
        IUnitOfWork unitOfWork,
        ILogger<UpdateOrganisationPatientContactCommandHandler> logger)
    {
        _patientRepository = patientRepository;
        _organisationRepository = organisationRepository;
        _organisationPatientLinkRepository = organisationPatientLinkRepository;
        _organisationPatientsRepository = organisationPatientsRepository;
        _unitOfWork = unitOfWork;
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

        // ── Walk-in patient: org owns full profile ──
        if (patient.IsWalkIn)
        {
            return await HandleWalkInUpdateAsync(patient, request, cancellationToken);
        }

        // ── Registered patient: org can only update medical fields ──
        return await HandleRegisteredUpdateAsync(patient, request, cancellationToken);
    }

    /// <summary>
    /// Walk-in: update ALL fields (admin + medical) on Patient entity.
    /// </summary>
    private async Task<Result<Guid>> HandleWalkInUpdateAsync(
        Patient patient,
        UpdateOrganisationPatientContactCommand request,
        CancellationToken cancellationToken)
    {
        // Resolve final values (use request if provided, else keep current)
        var fullName = request.FullName is not null
            ? request.FullName.Trim()
            : patient.FullName ?? "Unknown";

        var phoneNumber = request.PhoneNumber is not null
            ? TrimToNull(request.PhoneNumber)
            : patient.PhoneNumber;

        var citizenId = request.CitizenId is not null
            ? TrimToNull(request.CitizenId)
            : patient.CitizenId;

        var dateOfBirth = request.DateOfBirth ?? patient.DateOfBirth;

        int? genderId = request.Gender is not null
            ? ParseGenderId(request.Gender)
            : patient.GenderId;

        var address = request.Address is not null
            ? TrimToNull(request.Address)
            : patient.Address;

        var bmi = request.Bmi ?? patient.BMI;
        var diseaseHistory = request.DiseaseHistory is not null
            ? TrimToNull(request.DiseaseHistory)
            : patient.DiseaseHistory;

        // Duplicate phone check scoped to organisation
        if (phoneNumber is not null && !AreEquivalentPhoneNumber(phoneNumber, patient.PhoneNumber))
        {
            var isDuplicate = await IsPhoneDuplicateInOrgAsync(
                request.OrgAdminUserId, patient.Id, phoneNumber, cancellationToken);

            if (isDuplicate)
            {
                return Result<Guid>.Conflict("Phone number already exists in this organisation");
            }
        }

        // Duplicate citizenId check scoped to organisation
        if (citizenId is not null && citizenId != patient.CitizenId)
        {
            var isDuplicate = await IsCitizenIdDuplicateInOrgAsync(
                request.OrgAdminUserId, patient.Id, citizenId, cancellationToken);

            if (isDuplicate)
            {
                return Result<Guid>.Conflict("Citizen ID already exists in this organisation");
            }
        }

        patient.UpdateWalkInProfile(fullName, phoneNumber, citizenId, dateOfBirth, genderId, address);
        patient.UpdateProfile(bmi, diseaseHistory);

        await _patientRepository.UpdateAsync(patient, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        _logger.LogInformation(
            "OrgAdmin {OrgAdminId} updated walk-in patient {PatientId}",
            request.OrgAdminUserId, request.PatientId);

        return Result<Guid>.Success(request.PatientId);
    }

    /// <summary>
    /// Registered: ONLY update medical fields (BMI, DiseaseHistory).
    /// Admin fields belong to the patient via Identity — org CANNOT modify them.
    /// </summary>
    private async Task<Result<Guid>> HandleRegisteredUpdateAsync(
        Patient patient,
        UpdateOrganisationPatientContactCommand request,
        CancellationToken cancellationToken)
    {
        var bmi = request.Bmi ?? patient.BMI;
        var diseaseHistory = request.DiseaseHistory is not null
            ? TrimToNull(request.DiseaseHistory)
            : patient.DiseaseHistory;

        patient.UpdateProfile(bmi, diseaseHistory);

        await _patientRepository.UpdateAsync(patient, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        _logger.LogInformation(
            "OrgAdmin {OrgAdminId} updated medical info for registered patient {PatientId}",
            request.OrgAdminUserId, request.PatientId);

        return Result<Guid>.Success(request.PatientId);
    }

    // ── Helpers ──

    private async Task<bool> IsPhoneDuplicateInOrgAsync(
        Guid orgAdminUserId, Guid excludePatientId, string phoneNumber, CancellationToken ct)
    {
        var organisation = (await _organisationRepository.FindAsync(
                o => o.OwnerId == orgAdminUserId, ct))
            .FirstOrDefault();

        if (organisation is null) return false;

        return await _organisationPatientLinkRepository.Query()
            .Where(link => link.OrganisationId == organisation.Id && !link.IsDeleted)
            .Join(
                _patientRepository.Query()
                    .Where(p => p.Id != excludePatientId && p.PhoneNumber == phoneNumber && !p.IsDeleted),
                link => link.PatientId,
                p => p.Id,
                (link, p) => p)
            .AnyAsync(ct);
    }

    private async Task<bool> IsCitizenIdDuplicateInOrgAsync(
        Guid orgAdminUserId, Guid excludePatientId, string citizenId, CancellationToken ct)
    {
        var organisation = (await _organisationRepository.FindAsync(
                o => o.OwnerId == orgAdminUserId, ct))
            .FirstOrDefault();

        if (organisation is null) return false;

        return await _organisationPatientLinkRepository.Query()
            .Where(link => link.OrganisationId == organisation.Id && !link.IsDeleted)
            .Join(
                _patientRepository.Query()
                    .Where(p => p.Id != excludePatientId && p.CitizenId == citizenId && !p.IsDeleted),
                link => link.PatientId,
                p => p.Id,
                (link, p) => p)
            .AnyAsync(ct);
    }

    private static int? ParseGenderId(string? gender)
    {
        if (string.IsNullOrWhiteSpace(gender)) return null;
        if (gender.StartsWith("M", StringComparison.OrdinalIgnoreCase)) return 1;
        if (gender.StartsWith("F", StringComparison.OrdinalIgnoreCase)) return 2;
        return 3;
    }

    private static bool AreEquivalentPhoneNumber(string incomingPhone, string? existingPhone)
    {
        return NormalizePhone(incomingPhone) == NormalizePhone(existingPhone);
    }

    private static string NormalizePhone(string? phoneNumber)
    {
        if (string.IsNullOrWhiteSpace(phoneNumber)) return string.Empty;
        return new string(phoneNumber.Where(char.IsDigit).ToArray());
    }

    private static string? TrimToNull(string? value)
    {
        return string.IsNullOrWhiteSpace(value) ? null : value.Trim();
    }
}