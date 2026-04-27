using Application.Common.Interfaces;
using Application.Common.Models;
using Domain.Common;
using Domain.Repositories;

namespace Application.ClinicStaffs.Commands.UpdateClinicStaff;

/// <summary>
/// Updates sub-roles and profile details of an existing ClinicStaff member.
/// </summary>
public class UpdateClinicStaffCommandHandler : ICommandHandler<UpdateClinicStaffCommand>
{
    private readonly IClinicStaffRepository _clinicStaffRepository;
    private readonly IIdentityService _identityService;
    private readonly IUnitOfWork _unitOfWork;

    public UpdateClinicStaffCommandHandler(
        IClinicStaffRepository clinicStaffRepository,
        IIdentityService identityService,
        IUnitOfWork unitOfWork)
    {
        _clinicStaffRepository = clinicStaffRepository;
        _identityService = identityService;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> Handle(UpdateClinicStaffCommand request, CancellationToken cancellationToken)
    {
        var staff = await _clinicStaffRepository.GetByIdAsync(request.StaffId, cancellationToken);
        if (staff is null)
            return Result.NotFound($"ClinicStaff '{request.StaffId}' was not found.");

        staff.UpdateSubRoles(request.SubRoles);
        staff.UpdateProfile(request.Department, request.EmployeeCode, request.Phone);

        // Synchronize permissions based on new sub-roles
        await _identityService.SynchronizeUserSubRolePermissionsAsync(staff.UserId, request.SubRoles, cancellationToken);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
