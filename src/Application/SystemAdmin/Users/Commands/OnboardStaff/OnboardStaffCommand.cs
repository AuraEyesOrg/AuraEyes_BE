using Application.Common.Interfaces;
using Domain.Enums;

namespace Application.SystemAdmin.Users.Commands.OnboardStaff;

/// <summary>
/// Command to onboard a new staff member (Ophthalmologist, or ClinicStaff).
/// Creates both the Identity user and the corresponding profile entity.
/// </summary>
public record OnboardStaffCommand : ICommand<Guid>
{
    public string FullName { get; init; } = string.Empty;
    public string Email { get; init; } = string.Empty;
    public string Phone { get; init; } = string.Empty;
    public string Role { get; init; } = string.Empty; // Ophthalmologist, ClinicStaff
    
    /// <summary>
    /// Initial consultation fee for ophthalmologists.
    /// </summary>
    public decimal ConsultationFee { get; init; }

    /// <summary>
    /// Sub-roles for clinic staff (Receptionist, Coordinator, Cashier).
    /// </summary>
    public List<string> SubRoles { get; init; } = new();
}
