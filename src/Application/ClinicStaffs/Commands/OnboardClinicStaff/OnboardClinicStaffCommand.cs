using Application.Common.Interfaces;

namespace Application.ClinicStaffs.Commands.OnboardClinicStaff;

public record OnboardClinicStaffCommand : ICommand<bool>
{
    public string FullName { get; init; } = string.Empty;
    public string? Phone { get; init; }
    public string? Address { get; init; }
    public DateTime? DateOfBirth { get; init; }
    public int? Gender { get; init; }
    public string? CitizenId { get; init; }
    public string? Department { get; init; }
    public string? EmployeeCode { get; init; }
    public string? AvatarUrl { get; init; }
}
