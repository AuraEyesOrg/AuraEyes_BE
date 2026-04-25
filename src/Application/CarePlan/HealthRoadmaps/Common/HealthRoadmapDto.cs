namespace Application.CarePlan.HealthRoadmaps.Common;

/// <summary>
/// DTO returned by the <c>GET /api/roadmap/{patientId}</c> endpoint.
/// </summary>
public class HealthRoadmapDto
{
    public Guid? Id { get; set; }
    public Guid PatientId { get; set; }
    public DateTime? CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public IReadOnlyList<HealthRoadmapStepDto> Steps { get; set; } = Array.Empty<HealthRoadmapStepDto>();
}
