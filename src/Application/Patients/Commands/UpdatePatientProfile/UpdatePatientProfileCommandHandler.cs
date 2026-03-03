using Application.Common.Interfaces;
using Application.Common.Models;
using Application.Patients.Common;
using Domain.Common;
using Domain.Entities;
using Domain.Enums;
using Microsoft.Extensions.Logging;

namespace Application.Patients.Commands.UpdatePatientProfile;

public class UpdatePatientProfileCommandHandler : ICommandHandler<UpdatePatientProfileCommand, PatientProfileDto>
{
    private readonly IIdentityService _identityService;
    private readonly IRepository<Patient> _patientRepository;
    private readonly ILogger<UpdatePatientProfileCommandHandler> _logger;

    public UpdatePatientProfileCommandHandler(
        IIdentityService identityService,
        IRepository<Patient> patientRepository,
        ILogger<UpdatePatientProfileCommandHandler> logger)
    {
        _identityService = identityService;
        _patientRepository = patientRepository;
        _logger = logger;
    }

    public async Task<Result<PatientProfileDto>> Handle(
        UpdatePatientProfileCommand request,
        CancellationToken cancellationToken)
    {
        // Verify patient exists
        var patients = await _patientRepository.FindAsync(
            p => p.UserId == request.UserId, cancellationToken);
        var patient = patients.FirstOrDefault();

        if (patient is null)
            return Result<PatientProfileDto>.NotFound("Patient profile not found");

        // Parse gender
        int? genderValue = request.Gender?.ToLowerInvariant() switch
        {
            "male" => (int)Gender.Male,
            "female" => (int)Gender.Female,
            "other" => (int)Gender.Other,
            "prefernottotsay" => (int)Gender.PreferNotToSay,
            _ => null
        };

        // Parse date of birth
        DateTime? dateOfBirth = null;
        if (!string.IsNullOrWhiteSpace(request.DateOfBirth) &&
            DateTime.TryParse(request.DateOfBirth, out var dob))
        {
            dateOfBirth = DateTime.SpecifyKind(dob, DateTimeKind.Utc);
        }

        // Update ApplicationUser via identity service
        var (succeeded, errors) = await _identityService.UpdateUserProfileAsync(
            request.UserId,
            request.FullName,
            request.Phone,
            dateOfBirth,
            genderValue,
            request.Address,
            cancellationToken);

        if (!succeeded)
            return Result<PatientProfileDto>.Failure(errors);

        _logger.LogInformation("Patient profile updated for user {UserId}", request.UserId);

        // Fetch updated profile to return
        var userDetails = await _identityService.GetUserDetailsAsync(request.UserId, cancellationToken);
        var twoFactorEnabled = await _identityService.IsTwoFactorEnabledAsync(request.UserId);

        var dto = new PatientProfileDto
        {
            Id = patient.Id,
            Email = userDetails!.Email,
            FullName = userDetails.FullName,
            Phone = userDetails.PhoneNumber,
            DateOfBirth = userDetails.DateOfBirth,
            Gender = userDetails.Gender?.ToString().ToLower(),
            Address = userDetails.Address,
            AvatarUrl = userDetails.AvatarUrl,
            CreatedAt = patient.CreatedAt,
            UpdatedAt = patient.UpdatedAt,
            IsEmailVerified = userDetails.EmailConfirmed,
            IsTwoFactorEnabled = twoFactorEnabled,
        };

        return Result<PatientProfileDto>.Success(dto);
    }
}
