using Application.Common.Interfaces;
using Domain.Enums;

namespace Application.ClinicStaffs.Commands.CreateClinicStaff;

/// <summary>
/// Command to create a ClinicStaff profile for an existing Identity user.
/// </summary>
public record CreateClinicStaffCommand : ICommand<Guid>
{
    public Guid UserId { get; init; }
    public IReadOnlyList<ClinicStaffRole> SubRoles { get; init; } = [];
    public string? Department { get; init; }
    public string? EmployeeCode { get; init; }
    public string? Phone { get; init; }
}
