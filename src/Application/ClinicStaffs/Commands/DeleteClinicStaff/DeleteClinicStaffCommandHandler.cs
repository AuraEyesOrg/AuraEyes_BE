using Application.Common.Interfaces;
using Application.Common.Models;
using Domain.Common;
using Domain.Repositories;

namespace Application.ClinicStaffs.Commands.DeleteClinicStaff;

/// <summary>
/// Soft-deletes a ClinicStaff profile (deactivates account).
/// The underlying Identity user is NOT deleted by this command.
/// </summary>
public class DeleteClinicStaffCommandHandler : ICommandHandler<DeleteClinicStaffCommand>
{
    private readonly IClinicStaffRepository _clinicStaffRepository;
    private readonly IUnitOfWork _unitOfWork;

    public DeleteClinicStaffCommandHandler(
        IClinicStaffRepository clinicStaffRepository,
        IUnitOfWork unitOfWork)
    {
        _clinicStaffRepository = clinicStaffRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> Handle(DeleteClinicStaffCommand request, CancellationToken cancellationToken)
    {
        var staff = await _clinicStaffRepository.GetByIdAsync(request.StaffId, cancellationToken);
        if (staff is null)
            return Result.NotFound($"ClinicStaff '{request.StaffId}' was not found.");

        staff.Deactivate();
        _clinicStaffRepository.Remove(staff);   // triggers soft-delete via DbContext interceptor

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
