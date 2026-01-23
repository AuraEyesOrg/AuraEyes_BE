namespace Application.SystemAdmin.Dashboard.Queries.GetPopulationRiskAnalysis;

/// <summary>
/// DTO for population risk distribution analysis
/// </summary>
public class PopulationRiskAnalysisDto
{
    public int TotalPatients { get; set; }
    public List<RiskCategoryDto> RiskCategories { get; set; } = new();
}

public class RiskCategoryDto
{
    public string RiskLevel { get; set; } = string.Empty;
    public int Count { get; set; }
    public decimal Percentage { get; set; }
}
