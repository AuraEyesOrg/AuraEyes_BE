using Application.Common.Interfaces;
using Application.Common.Models;
using Application.Patients.Common;
using Domain.Common;
using Domain.Entities;
using Domain.Entities.Users;
using Microsoft.Extensions.Logging;

namespace Application.Patients.Queries.GetPatientProfile;

public class GetPatientProfileQueryHandler : IQueryHandler<GetPatientProfileQuery, PatientProfileDto>
{
    private readonly IIdentityService _identityService;
    private readonly IRepository<Patient> _patientRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<GetPatientProfileQueryHandler> _logger;

    public GetPatientProfileQueryHandler(
        IIdentityService identityService,
        IRepository<Patient> patientRepository,
        IUnitOfWork unitOfWork,
        ILogger<GetPatientProfileQueryHandler> logger)
    {
        _identityService = identityService;
        _patientRepository = patientRepository;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<Result<PatientProfileDto>> Handle(
        GetPatientProfileQuery request,
        CancellationToken cancellationToken)
    {
        var user = await _identityService.GetUserByIdAsync(request.UserId, cancellationToken);
        if (user is null)
            return Result<PatientProfileDto>.NotFound("User not found");

        var patients = await _patientRepository.FindAsync(
            p => p.UserId == request.UserId, cancellationToken);
        var patient = patients.FirstOrDefault();

        if (patient is null)
        {
            _logger.LogInformation("Patient profile not found for user {UserId}. Creating self-healing profile.", request.UserId);
            
            // Self-healing: create missing patient record
            patient = Patient.CreateRegistered(request.UserId);
            await _patientRepository.AddAsync(patient, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
        }

        var twoFactorEnabled = await _identityService.IsTwoFactorEnabledAsync(request.UserId);
        var userDetails = await _identityService.GetUserDetailsAsync(request.UserId, cancellationToken);

        var dto = new PatientProfileDto
        {
            Id = patient.Id,
            Email = user.Email,
            FullName = user.FullName,
            Phone = userDetails?.PhoneNumber,
            DateOfBirth = userDetails?.DateOfBirth,
            Gender = userDetails?.Gender?.ToString().ToLower(),
            Address = userDetails?.Address,
            CitizenId = userDetails?.CitizenId,
            AvatarUrl = userDetails?.AvatarUrl,
            CreatedAt = patient.CreatedAt,
            UpdatedAt = patient.UpdatedAt,
            IsEmailVerified = user.EmailConfirmed,
            IsTwoFactorEnabled = twoFactorEnabled,
        };

        return Result<PatientProfileDto>.Success(dto);
    }
}
