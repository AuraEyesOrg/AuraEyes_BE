namespace Application.SystemAdmin.AiModels.Queries.GetModelVersions;

/// <summary>
/// DTO for model versions
/// </summary>
public class ModelVersionsDto
{
    public List<ModelVersionItemDto> Versions { get; set; } = new();
}

public class ModelVersionItemDto
{
    public Guid Id { get; set; }
    public string Version { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string Status { get; set; } = string.Empty;
    public decimal Accuracy { get; set; }
    public decimal Sensitivity { get; set; }
    public decimal Specificity { get; set; }
    public double AverageInferenceTimeMs { get; set; }
    public long TotalInferences { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? DeployedAt { get; set; }
    public string? ReleaseNotes { get; set; }
    public bool CanPromote { get; set; }
}
