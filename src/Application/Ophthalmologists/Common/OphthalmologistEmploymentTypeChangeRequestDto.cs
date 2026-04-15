using Domain.Enums;

namespace Application.Ophthalmologists.Common;

public record OphthalmologistEmploymentTypeChangeRequestDto
{
    public Guid Id { get; init; }
    public Guid OphthalmologistId { get; init; }
    public OphthalmologistEmploymentType CurrentEmploymentType { get; init; }
    public OphthalmologistEmploymentType TargetEmploymentType { get; init; }
    public string Reason { get; init; } = string.Empty;
    public OphthalmologistEmploymentTypeChangeRequestStatus Status { get; init; }
    public string? AdminNote { get; init; }
    public Guid? ReviewedByAdminUserId { get; init; }
    public DateTime? ReviewedAt { get; init; }
    public DateTime CreatedAt { get; init; }
}
