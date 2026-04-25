using Application.ClinicStaffs.Common;
using Application.Common.Interfaces;

namespace Application.ClinicStaffs.Commands.UpdateClinicStaffProfile;

/// <summary>
/// Update command for current clinic staff profile.
/// </summary>
public record UpdateClinicStaffProfileCommand : ICommand<ClinicStaffProfileDto>
{
    public Guid UserId { get; init; }
    public string FullName { get; init; } = string.Empty;
    public string? Phone { get; init; }
    public string? DateOfBirth { get; init; }
    public string? Gender { get; init; }
    public string? Address { get; init; }
    public string? CitizenId { get; init; }
    public string? Department { get; init; }
    public string? EmployeeCode { get; init; }
}

