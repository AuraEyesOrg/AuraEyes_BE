using Application.Common.Constants;
using Application.Common.Interfaces;
using Application.Common.Models;
using Domain.Common;
using Domain.Entities.Users;
using Domain.Repositories;

namespace Application.ClinicStaffs.Commands.CreateClinicStaff;

/// <summary>
/// Creates a ClinicStaff profile for an existing Identity user and assigns the ClinicStaff role.
/// </summary>
public class CreateClinicStaffCommandHandler : ICommandHandler<CreateClinicStaffCommand, Guid>
{
    private readonly IClinicStaffRepository _clinicStaffRepository;
    private readonly IIdentityService _identityService;
    private readonly IUnitOfWork _unitOfWork;

    public CreateClinicStaffCommandHandler(
        IClinicStaffRepository clinicStaffRepository,
        IIdentityService identityService,
        IUnitOfWork unitOfWork)
    {
        _clinicStaffRepository = clinicStaffRepository;
        _identityService = identityService;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<Guid>> Handle(CreateClinicStaffCommand request, CancellationToken cancellationToken)
    {
        // 1. Verify the user exists
        var user = await _identityService.GetUserByIdAsync(request.UserId, cancellationToken);
        if (user is null)
            return Result<Guid>.NotFound($"User '{request.UserId}' was not found.");

        // 2. Ensure no duplicate profile
        var exists = await _clinicStaffRepository.ExistsByUserIdAsync(request.UserId, cancellationToken);
        if (exists)
            return Result<Guid>.Conflict($"A ClinicStaff profile already exists for user '{request.UserId}'.");

        await _unitOfWork.BeginTransactionAsync(cancellationToken);

        try
        {
            // 3. Create the domain entity
            var staff = new ClinicStaff(
                request.UserId,
                request.SubRoles,
                request.Department,
                request.EmployeeCode,
                request.Phone);

            await _clinicStaffRepository.AddAsync(staff, cancellationToken);

            // 4. Assign ClinicStaff role via Identity if not already assigned
            var roles = await _identityService.GetUserRolesAsync(request.UserId);
            if (!roles.Contains(Roles.ClinicStaff))
            {
                await _identityService.AddToRoleAsync(request.UserId, Roles.ClinicStaff);
            }

            // 5. Synchronize sub-role specific permissions
            await _identityService.SynchronizeUserSubRolePermissionsAsync(request.UserId, request.SubRoles, cancellationToken);

            await _unitOfWork.SaveChangesAsync(cancellationToken);
            await _unitOfWork.CommitTransactionAsync(cancellationToken);

            return Result<Guid>.Success(staff.Id);
        }
        catch
        {
            await _unitOfWork.RollbackTransactionAsync(cancellationToken);
            throw;
        }
    }
}
