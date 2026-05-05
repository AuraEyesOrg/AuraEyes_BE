using Application.Common.Constants;
using Application.Common.Interfaces;
using Application.Common.Models;
using Domain.Common;
using Domain.Entities.Users;
using Domain.Repositories;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Application.Patients.Commands.CreateWalkInPatient;

public record CreateWalkInPatientCommand : IRequest<Result<CreateWalkInPatientResult>>
{
    public string FullName { get; init; } = string.Empty;
    public string? Email { get; init; }
    public string? PhoneNumber { get; init; }
    public string? CitizenId { get; init; }
    public DateTime? DateOfBirth { get; init; }
    public int? Gender { get; init; }
    public string? Address { get; init; }
}

public record CreateWalkInPatientResult
{
    public Guid PatientId { get; init; }
    public Guid UserId { get; init; }
    public string FullName { get; init; } = string.Empty;
    public string? LoginEmail { get; init; }
    public bool IsGeneratedEmail { get; init; }
    public bool EmailSent { get; init; }
    public string? TemporaryPassword { get; init; }
}

public class CreateWalkInPatientCommandHandler : IRequestHandler<CreateWalkInPatientCommand, Result<CreateWalkInPatientResult>>
{
    private readonly IIdentityService _identityService;
    private readonly IRepository<Patient> _patientRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<CreateWalkInPatientCommandHandler> _logger;
    private readonly IEmailService _emailService;

    public CreateWalkInPatientCommandHandler(
        IIdentityService identityService,
        IRepository<Patient> patientRepository,
        IUnitOfWork unitOfWork,
        ILogger<CreateWalkInPatientCommandHandler> logger,
        IEmailService emailService)
    {
        _identityService = identityService;
        _patientRepository = patientRepository;
        _unitOfWork = unitOfWork;
        _logger = logger;
        _emailService = emailService;
    }

    public async Task<Result<CreateWalkInPatientResult>> Handle(CreateWalkInPatientCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Creating walk-in patient: {FullName}", request.FullName);

        // 1. Validation: Check if email is provided and already exists
        if (string.IsNullOrWhiteSpace(request.Email))
        {
            return Result<CreateWalkInPatientResult>.Failure("Email is required for walk-in patient registration.");
        }

        var existingUser = await _identityService.GetUserByEmailAsync(request.Email, cancellationToken);
        if (existingUser != null)
        {
            return Result<CreateWalkInPatientResult>.Failure("Email is already in use.");
        }

        // 2. Validation: Check if CitizenId already exists
        if (!string.IsNullOrWhiteSpace(request.CitizenId))
        {
            var userByCitizenId = await _identityService.GetUserByCitizenIdAsync(request.CitizenId, cancellationToken);
            if (userByCitizenId != null)
            {
                return Result<CreateWalkInPatientResult>.Failure("Citizen ID is already in use.");
            }
        }

        // 3. Create Identity User
        // For walk-ins, we generate a random password or a default one
        // Since they will likely need to login later, we use a predictable pattern or let them reset it
        var defaultPassword = "Patient@WalkIn123!"; 
        var email = !string.IsNullOrWhiteSpace(request.Email) 
            ? request.Email 
            : $"{Guid.NewGuid():N}@auraeyes.com"; // Fallback email if not provided

        var profileDto = new UserProfileWalkInDto(
            FullName: request.FullName,
            PhoneNumber: request.PhoneNumber,
            DateOfBirth: request.DateOfBirth,
            Gender: request.Gender,
            Address: request.Address,
            AvatarUrl: null,
            CitizenId: request.CitizenId
        );

        var (succeeded, userId, errors) = await _identityService.CreateUserWalkInPatientAsync(
            request.Email,
            defaultPassword,
            request.FullName,
            Roles.Patient,
            profileDto,
            cancellationToken);

        if (!succeeded)
        {
            return Result<CreateWalkInPatientResult>.Failure("Failed to create user account: " + string.Join(", ", errors));
        }

        // 4. Create Patient Entity
        var patient = Patient.CreateRegistered(userId!.Value, isWalkIn: true);
        
        await _patientRepository.AddAsync(patient, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Walk-in patient created successfully. PatientId: {PatientId}, UserId: {UserId}", patient.Id, userId);

        // 5. Send credentials via email
        try
        {
            await _emailService.SendPatientWalkInCredentialsAsync(request.Email, request.FullName, defaultPassword, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to send credentials email to walk-in patient {Email}", request.Email);
            // We don't fail the whole command if email fails, but we should log it
        }

        return Result<CreateWalkInPatientResult>.Success(new CreateWalkInPatientResult
        {
            PatientId = patient.Id,
            UserId = userId.Value,
            FullName = request.FullName,
            LoginEmail = request.Email,
            IsGeneratedEmail = false,
            EmailSent = true
        });
    }
}
