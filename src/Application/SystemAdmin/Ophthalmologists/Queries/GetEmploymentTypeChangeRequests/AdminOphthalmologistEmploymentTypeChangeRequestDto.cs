using Domain.Enums;

namespace Application.SystemAdmin.Ophthalmologists.Queries.GetEmploymentTypeChangeRequests;

public record AdminOphthalmologistEmploymentTypeChangeRequestDto
{
    public Guid Id { get; init; }
    public Guid OphthalmologistId { get; init; }
    public Guid DoctorUserId { get; init; }
    public string DoctorFullName { get; init; } = string.Empty;
    public string DoctorEmail { get; init; } = string.Empty;
    public OphthalmologistEmploymentType CurrentEmploymentType { get; init; }
    public OphthalmologistEmploymentType TargetEmploymentType { get; init; }
    public string Reason { get; init; } = string.Empty;
    public OphthalmologistEmploymentTypeChangeRequestStatus Status { get; init; }
    public string? AdminNote { get; init; }
    public Guid? ReviewedByAdminUserId { get; init; }
    public DateTime? ReviewedAt { get; init; }
    public DateTime CreatedAt { get; init; }
}
