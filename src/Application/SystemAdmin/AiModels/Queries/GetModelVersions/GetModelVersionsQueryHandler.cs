using Application.Common.Interfaces;
using Application.Common.Models;

namespace Application.SystemAdmin.AiModels.Queries.GetModelVersions;

/// <summary>
/// Handler for GetModelVersionsQuery - Returns mock data
/// </summary>
public class GetModelVersionsQueryHandler : IQueryHandler<GetModelVersionsQuery, ModelVersionsDto>
{
    public Task<Result<ModelVersionsDto>> Handle(GetModelVersionsQuery request, CancellationToken cancellationToken)
    {
        // Mock model versions
        var versions = new List<ModelVersionItemDto>
        {
            new() { Id = Guid.NewGuid(), Version = "2.3.1", Name = "DR Detection v2.3.1", Description = "Production stable release", Status = "Active", Accuracy = 96.5m, Sensitivity = 97.2m, Specificity = 95.8m, AverageInferenceTimeMs = 145.5, TotalInferences = 125000, CreatedAt = DateTime.UtcNow.AddMonths(-2), DeployedAt = DateTime.UtcNow.AddMonths(-2), ReleaseNotes = "Improved DR grade detection", CanPromote = false },
            new() { Id = Guid.NewGuid(), Version = "2.4.0-beta", Name = "DR Detection v2.4.0", Description = "Beta testing phase", Status = "Staging", Accuracy = 97.1m, Sensitivity = 97.8m, Specificity = 96.4m, AverageInferenceTimeMs = 138.2, TotalInferences = 15000, CreatedAt = DateTime.UtcNow.AddDays(-14), ReleaseNotes = "New architecture with improved speed", CanPromote = true },
            new() { Id = Guid.NewGuid(), Version = "2.5.0-dev", Name = "DR Detection v2.5.0", Description = "Development version", Status = "Development", Accuracy = 94.2m, Sensitivity = 95.1m, Specificity = 93.3m, AverageInferenceTimeMs = 160.0, TotalInferences = 500, CreatedAt = DateTime.UtcNow.AddDays(-5), ReleaseNotes = "Experimental multi-disease detection", CanPromote = true },
            new() { Id = Guid.NewGuid(), Version = "2.2.0", Name = "DR Detection v2.2.0", Description = "Previous production version", Status = "Deprecated", Accuracy = 95.8m, Sensitivity = 96.5m, Specificity = 95.1m, AverageInferenceTimeMs = 155.0, TotalInferences = 250000, CreatedAt = DateTime.UtcNow.AddMonths(-6), DeployedAt = DateTime.UtcNow.AddMonths(-6), CanPromote = false },
            new() { Id = Guid.NewGuid(), Version = "2.1.0", Name = "DR Detection v2.1.0", Description = "Archived version", Status = "Archived", Accuracy = 94.5m, Sensitivity = 95.2m, Specificity = 93.8m, AverageInferenceTimeMs = 165.0, TotalInferences = 180000, CreatedAt = DateTime.UtcNow.AddYears(-1), CanPromote = false }
        };

        var dto = new ModelVersionsDto
        {
            Versions = versions.Take(request.Limit).ToList()
        };

        return Task.FromResult(Result<ModelVersionsDto>.Success(dto));
    }
}
