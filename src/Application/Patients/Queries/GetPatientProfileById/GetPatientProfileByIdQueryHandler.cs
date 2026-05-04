using Application.Common.Interfaces;
using Application.Common.Models;
using Application.Patients.Common;
using Domain.Common;
using Domain.Entities.Users;
using Microsoft.Extensions.Logging;

namespace Application.Patients.Queries.GetPatientProfileById;

public class GetPatientProfileByIdQueryHandler : IQueryHandler<GetPatientProfileByIdQuery, PatientProfileDto>
{
    private readonly IIdentityService _identityService;
    private readonly IRepository<Patient> _patientRepository;

    public GetPatientProfileByIdQueryHandler(
        IIdentityService identityService,
        IRepository<Patient> patientRepository)
    {
        _identityService = identityService;
        _patientRepository = patientRepository;
    }

    public async Task<Result<PatientProfileDto>> Handle(
        GetPatientProfileByIdQuery request,
        CancellationToken cancellationToken)
    {
        var patient = await _patientRepository.GetByIdAsNoTrackingAsync(request.PatientId, cancellationToken);
        if (patient is null)
            return Result<PatientProfileDto>.NotFound("Patient not found");

        if (patient.UserId == null)
        {
            // Walk-in patient handling
            return Result<PatientProfileDto>.Success(new PatientProfileDto
            {
                Id = patient.Id,
                FullName = patient.FullName ?? "Unknown",
                Phone = patient.PhoneNumber,
                DateOfBirth = patient.DateOfBirth,
                Gender = patient.GenderId.HasValue ? ((Domain.Enums.Gender)patient.GenderId.Value).ToString().ToLower() : null,
                Address = patient.Address,
                MedicalRecordNumber = patient.MedicalRecordNumber,
                CreatedAt = patient.CreatedAt,
                UpdatedAt = patient.UpdatedAt,
            });
        }

        var user = await _identityService.GetUserByIdAsync(patient.UserId.Value, cancellationToken);
        var userDetails = await _identityService.GetUserDetailsAsync(patient.UserId.Value, cancellationToken);

        var dto = new PatientProfileDto
        {
            Id = patient.Id,
            Email = user?.Email,
            FullName = user?.FullName ?? patient.FullName,
            Phone = userDetails?.PhoneNumber ?? patient.PhoneNumber,
            DateOfBirth = userDetails?.DateOfBirth ?? patient.DateOfBirth,
            Gender = (userDetails?.Gender?.ToString() ?? (patient.GenderId.HasValue ? ((Domain.Enums.Gender)patient.GenderId.Value).ToString() : null))?.ToLower(),
            Address = userDetails?.Address ?? patient.Address,
            CitizenId = userDetails?.CitizenId,
            AvatarUrl = userDetails?.AvatarUrl,
            MedicalRecordNumber = patient.MedicalRecordNumber,
            CreatedAt = patient.CreatedAt,
            UpdatedAt = patient.UpdatedAt,
            IsEmailVerified = user?.EmailConfirmed ?? false,
        };

        return Result<PatientProfileDto>.Success(dto);
    }
}

