using Application.Common.Interfaces;

namespace Application.SystemAdmin.AiModels.Queries.GetModelVersions;

/// <summary>
/// Query to get model version history
/// Screen: 3.9.7 View Model Version History
/// </summary>
public record GetModelVersionsQuery : IQuery<ModelVersionsDto>
{
    public int Limit { get; init; } = 10;
}
