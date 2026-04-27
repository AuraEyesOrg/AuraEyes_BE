using Application.Common.Interfaces;

namespace Application.ClinicStaffs.Commands.DeleteClinicStaff;

/// <summary>
/// Command to soft-delete (deactivate) a ClinicStaff profile.
/// </summary>
public record DeleteClinicStaffCommand : ICommand
{
    public Guid StaffId { get; init; }
}
