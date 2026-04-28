using Application.Common.Interfaces;
using Application.Common.Models;
using Domain.Common;
using Domain.Entities.Users;
using Domain.Enums;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Application.Patients.Commands.UpdateClinicPatient;

/// <summary>
/// Clinic staff updates a patient's demographic and medical info using PatientId.
///
/// Walk-in patients (created by clinic staff) have a system-generated Identity
/// account — their profile data lives in ApplicationUser, not directly on Patient.
/// We update via IIdentityService just like UpdatePatientProfileCommand.
///
/// Medical fields (BMI, DiseaseHistory) are stored on the Patient entity itself
/// and are editable for ALL patient types.
/// </summary>
public record UpdateClinicPatientCommand : IRequest<Result<string>>
{
    public Guid PatientId { get; init; }

    // Demographics – forwarded to IIdentityService
    public string? FullName { get; init; }
    public string? PhoneNumber { get; init; }
    public string? CitizenId { get; init; }
    public string? DateOfBirth { get; init; }
    public string? Gender { get; init; }
    public string? Address { get; init; }

    // Medical fields – stored on Patient entity
    public decimal? Bmi { get; init; }
    public string? DiseaseHistory { get; init; }
}

public class UpdateClinicPatientCommandHandler
    : IRequestHandler<UpdateClinicPatientCommand, Result<string>>
{
    private readonly IIdentityService _identityService;
    private readonly IRepository<Patient> _patientRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<UpdateClinicPatientCommandHandler> _logger;

    public UpdateClinicPatientCommandHandler(
        IIdentityService identityService,
        IRepository<Patient> patientRepository,
        IUnitOfWork unitOfWork,
        ILogger<UpdateClinicPatientCommandHandler> logger)
    {
        _identityService = identityService;
        _patientRepository = patientRepository;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<Result<string>> Handle(
        UpdateClinicPatientCommand request,
        CancellationToken cancellationToken)
    {
        var patient = await _patientRepository.GetByIdAsync(request.PatientId, cancellationToken);
        if (patient is null)
            return Result<string>.NotFound("Patient not found.");

        // ── Update demographics via Identity ──────────────────────────────
        bool hasDemographicUpdate =
            request.FullName is not null ||
            request.PhoneNumber is not null ||
            request.CitizenId is not null ||
            request.DateOfBirth is not null ||
            request.Gender is not null ||
            request.Address is not null;

        if (hasDemographicUpdate && patient.UserId.HasValue)
        {
            // Fetch current values so we don't wipe fields with null
            var current = await _identityService.GetUserDetailsAsync(
                patient.UserId.Value, cancellationToken);

            if (current is null)
                return Result<string>.Failure("Identity user not found for this patient.");

            int? genderValue = request.Gender?.ToLowerInvariant() switch
            {
                "male" => (int)Gender.Male,
                "female" => (int)Gender.Female,
                "other" => (int)Gender.Other,
                _ => null
            };

            DateTime? dateOfBirth = null;
            if (!string.IsNullOrWhiteSpace(request.DateOfBirth) &&
                DateTime.TryParse(request.DateOfBirth, out var dob))
            {
                dateOfBirth = DateTime.SpecifyKind(dob, DateTimeKind.Utc);
            }

            var (succeeded, errors) = await _identityService.UpdateUserProfileAsync(
                patient.UserId.Value,
                request.FullName ?? current.FullName,
                request.PhoneNumber ?? current.PhoneNumber,
                dateOfBirth ?? current.DateOfBirth,
                genderValue ?? (current.Gender.HasValue ? (int)current.Gender.Value : (int?)null),
                request.Address ?? current.Address,
                request.CitizenId ?? current.CitizenId,
                cancellationToken);

            if (!succeeded)
                return Result<string>.Failure("Failed to update patient profile: " +
                                              string.Join(", ", errors));
        }

        // ── Update medical fields on Patient entity ──────────────────────
        bool hasMedicalUpdate = request.Bmi is not null || request.DiseaseHistory is not null;
        if (hasMedicalUpdate)
        {
            var newBmi = request.Bmi ?? patient.BMI;
            var newDiseaseHistory = request.DiseaseHistory ?? patient.DiseaseHistory;
            patient.UpdateProfile(newBmi, newDiseaseHistory);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
        }

        _logger.LogInformation(
            "Clinic staff updated patient {PatientId}",
            request.PatientId);

        return Result<string>.Success("Patient updated successfully.");
    }
}
