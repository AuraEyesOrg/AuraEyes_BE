using Domain.Enums;

namespace Application.SystemAdmin.Ophthalmologists.Queries.GetLeaveRequests;

public record AdminOphthalmologistLeaveRequestDto
{
    public Guid Id { get; init; }
    public Guid OphthalmologistId { get; init; }
    public Guid DoctorUserId { get; init; }
    public string DoctorFullName { get; init; } = string.Empty;
    public string DoctorEmail { get; init; } = string.Empty;
    public DateOnly StartDate { get; init; }
    public DateOnly EndDate { get; init; }
    public string Reason { get; init; } = string.Empty;
    public OphthalmologistLeaveRequestStatus Status { get; init; }
    public string? AdminNote { get; init; }
    public Guid? ReviewedByAdminUserId { get; init; }
    public DateTime? ReviewedAt { get; init; }
    public DateTime CreatedAt { get; init; }
}
