using Application.Common.Constants;
using Application.Common.Interfaces;
using Application.Common.Models;
using Domain.Common;
using Domain.Entities.Users;
using Domain.Enums;
using Domain.Repositories;
using Microsoft.Extensions.Logging;

namespace Application.SystemAdmin.Users.Commands.OnboardStaff;

public class OnboardStaffCommandHandler : ICommandHandler<OnboardStaffCommand, Guid>
{
    private readonly IIdentityService _identityService;
    private readonly IOphthalmologistRepository _ophthalmologistRepository;
    private readonly IClinicStaffRepository _clinicStaffRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IEmailService _emailService;
    private readonly ILogger<OnboardStaffCommandHandler> _logger;

    public OnboardStaffCommandHandler(
        IIdentityService identityService,
        IOphthalmologistRepository ophthalmologistRepository,
        IClinicStaffRepository clinicStaffRepository,
        IUnitOfWork unitOfWork,
        IEmailService emailService,
        ILogger<OnboardStaffCommandHandler> logger)
    {
        _identityService = identityService;
        _ophthalmologistRepository = ophthalmologistRepository;
        _clinicStaffRepository = clinicStaffRepository;
        _unitOfWork = unitOfWork;
        _emailService = emailService;
        _logger = logger;
    }

    public async Task<Result<Guid>> Handle(OnboardStaffCommand request, CancellationToken cancellationToken)
    {
        // 1. Check if user already exists
        var existingUser = await _identityService.GetUserByEmailAsync(request.Email, cancellationToken);
        if (existingUser != null)
        {
            return Result<Guid>.Failure("A user with this email already exists.");
        }

        // 2. Generate a temporary password
        var temporaryPassword = GenerateTemporaryPassword();

        await _unitOfWork.BeginTransactionAsync(cancellationToken);

        try
        {
            // 3. Create Identity User with Role
            var (succeeded, errors) = await _identityService.CreateUserWithRoleAsync(
                request.Email,
                temporaryPassword,
                request.FullName,
                request.Role,
                cancellationToken);

            if (!succeeded)
            {
                await _unitOfWork.RollbackTransactionAsync(cancellationToken);
                return Result<Guid>.Failure(errors);
            }

            var user = await _identityService.GetUserByEmailAsync(request.Email, cancellationToken);
            if (user == null)
            {
                await _unitOfWork.RollbackTransactionAsync(cancellationToken);
                return Result<Guid>.Failure("Failed to retrieve created user.");
            }

            // 4. Create Profile Entity based on Role
            if (request.Role == Roles.Ophthalmologist)
            {
                var ophthalmologist = new Ophthalmologist(
                    user.Id,
                    bio: "New staff member",
                    yearsOfExperience: 0,
                    phone: request.Phone);
                
                await _ophthalmologistRepository.AddAsync(ophthalmologist, cancellationToken);
            }
            else if (request.Role == Roles.ClinicStaff)
            {
                var clinicStaff = new ClinicStaff(
                    user.Id,
                    subRoles: [ClinicStaffRole.Receptionist],
                    department: "General",
                    employeeCode: $"STAFF-{DateTime.UtcNow:yyyyMMddHHmm}",
                    phone: request.Phone);
                
                await _clinicStaffRepository.AddAsync(clinicStaff, cancellationToken);
            }

            await _unitOfWork.SaveChangesAsync(cancellationToken);
            await _unitOfWork.CommitTransactionAsync(cancellationToken);

            // 5. Finalize onboarding (Confirm email + Set MustChangePassword)
            await _identityService.SetStaffOnboardingStatusAsync(user.Id);

            // 6. Send beautiful onboarding email with temporary password
            await SendOnboardingEmail(request.Email, request.FullName, temporaryPassword, cancellationToken);

            return Result<Guid>.Success(user.Id);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error onboarding staff: {Email}", request.Email);
            await _unitOfWork.RollbackTransactionAsync(cancellationToken);
            throw;
        }
    }

    private string GenerateTemporaryPassword()
    {
        // Simple temporary password generation
        return $"Aura@{Guid.NewGuid().ToString("N")[..8]}!";
    }

    private async Task SendOnboardingEmail(string email, string fullName, string temporaryPassword, CancellationToken cancellationToken)
    {
        try
        {
            await _emailService.SendStaffOnboardingEmailAsync(email, fullName, temporaryPassword, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Failed to send onboarding email to {Email}", email);
        }
    }
}
