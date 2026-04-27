using Application.Common.Interfaces;
using Application.Common.Models;
using Domain.Enums;
using MediatR;

namespace Application.Screenings.Queries.GetClinicScreeningHistory;

public class ClinicScreeningHistoryDto
{
    public Guid ScreeningId { get; init; }
    public Guid PatientId { get; init; }
    public string PatientName { get; init; } = string.Empty;
    public DateTime CreatedAt { get; init; }
    public DateTime? ProcessedAt { get; init; }
    public int ImagesCount { get; init; }
    public string? LatestRiskLevel { get; init; }
    public decimal? ConfidenceScore { get; init; }
    public string? AiPrimaryLabel { get; init; }
    public string Status { get; init; } = "pending";
}

public class GetClinicScreeningHistoryQuery : IRequest<Result<IReadOnlyList<ClinicScreeningHistoryDto>>>
{
    public int Take { get; init; } = 50;
}
