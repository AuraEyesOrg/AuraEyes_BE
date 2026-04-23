using Domain.Enums;

namespace Application.ClinicStaffs.Common;

/// <summary>
/// Response DTO returned for any ClinicStaff read operation.
/// </summary>
public record ClinicStaffResponse(
    Guid Id,
    Guid UserId,
    string FullName,
    string Email,
    string? AvatarUrl,
    IReadOnlyList<string> SubRoles,
    string? Department,
    string? EmployeeCode,
    string? Phone,
    bool IsActive,
    DateTime CreatedAt,
    DateTime? UpdatedAt);
