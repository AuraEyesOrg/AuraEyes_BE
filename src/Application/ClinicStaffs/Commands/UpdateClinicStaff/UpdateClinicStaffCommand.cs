using Application.Common.Interfaces;
using Domain.Enums;

namespace Application.ClinicStaffs.Commands.UpdateClinicStaff;

/// <summary>
/// Command to update a ClinicStaff profile (sub-roles and profile details).
/// </summary>
public record UpdateClinicStaffCommand : ICommand
{
    public Guid StaffId { get; init; }
    public IReadOnlyList<ClinicStaffRole> SubRoles { get; init; } = [];
    public string? Department { get; init; }
    public string? EmployeeCode { get; init; }
    public string? Phone { get; init; }
}
