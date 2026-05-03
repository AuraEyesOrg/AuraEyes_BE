using Application.Common.Interfaces;
using Application.Common.Models;
using Domain.Common;
using Domain.Entities.Screening;
using Domain.Entities.Users;
using Microsoft.EntityFrameworkCore;

namespace Application.ClinicScreenings.Queries.GetClinicScreeningHistory;

public record GetClinicScreeningHistoryQuery : IQuery<IReadOnlyList<ClinicScreeningHistoryDto>>
{
    public int Take { get; init; } = 50;
}

public record ClinicScreeningHistoryDto
{
    public Guid ScreeningId { get; init; }
    public Guid PatientId { get; init; }
    public string PatientName { get; init; } = string.Empty;
    public DateTime CreatedAt { get; init; }
    public DateTime? ProcessedAt { get; init; }
    public int ImagesCount { get; init; }
    public string? LatestRiskLevel { get; init; }
    public decimal? ConfidenceScore { get; init; }
    public string Status { get; init; } = "pending";
}

public class GetClinicScreeningHistoryQueryHandler
    : IQueryHandler<GetClinicScreeningHistoryQuery, IReadOnlyList<ClinicScreeningHistoryDto>>
{
    private readonly IRepository<AiScreening> _screeningRepository;
    private readonly IRepository<Patient> _patientRepository;
    private readonly IIdentityService _identityService;

    public GetClinicScreeningHistoryQueryHandler(
        IRepository<AiScreening> screeningRepository,
        IRepository<Patient> patientRepository,
        IIdentityService identityService)
    {
        _screeningRepository = screeningRepository;
        _patientRepository = patientRepository;
        _identityService = identityService;
    }

    public async Task<Result<IReadOnlyList<ClinicScreeningHistoryDto>>> Handle(
        GetClinicScreeningHistoryQuery request,
        CancellationToken cancellationToken)
    {
        var take = Math.Clamp(request.Take, 1, 200);

        var sessions = await _screeningRepository
            .Query().AsNoTracking()
            .Where(s => !s.IsDeleted)
            .OrderByDescending(s => s.CreatedAt)
            .Take(take)
            .Select(s => new
            {
                s.Id,
                s.PatientId,
                s.CreatedAt,
                s.ProcessedAt,
                ImagesCount = s.RetinalImages.Count,
                LatestResult = s.ScreeningResults
                    .OrderByDescending(r => r.CreatedAt)
                    .Select(r => new { r.RiskLevel, r.ConfidenceScore })
                    .FirstOrDefault()
            })
            .ToListAsync(cancellationToken);

        var patientIds = sessions.Select(s => s.PatientId).Distinct().ToList();
        var patients = await _patientRepository
            .Query().AsNoTracking()
            .Where(p => patientIds.Contains(p.Id))
            .ToListAsync(cancellationToken);

        var patientUserIds = patients
            .Where(p => p.UserId.HasValue)
            .Select(p => p.UserId!.Value)
            .Distinct()
            .ToList();
            
        var userNames = new Dictionary<Guid, string>();
        
        foreach (var userId in patientUserIds)
        {
            var user = await _identityService.GetUserByIdAsync(userId, cancellationToken);
            if (user != null)
            {
                userNames[userId] = user.FullName ?? "Unknown Patient";
            }
        }

        var results = sessions.Select(s =>
        {
            var patient = patients.FirstOrDefault(p => p.Id == s.PatientId);
            string patientName = "Unknown Patient";

            if (patient != null)
            {
                if (patient.UserId.HasValue && userNames.TryGetValue(patient.UserId.Value, out var name))
                {
                    patientName = name;
                }
                else if (patient.IsWalkIn && !string.IsNullOrWhiteSpace(patient.FullName))
                {
                    patientName = patient.FullName;
                }
            }

            return new ClinicScreeningHistoryDto
            {
                ScreeningId = s.Id,
                PatientId = s.PatientId,
                PatientName = patientName,
                CreatedAt = s.CreatedAt,
                ProcessedAt = s.ProcessedAt,
                ImagesCount = s.ImagesCount,
                LatestRiskLevel = s.LatestResult?.RiskLevel.ToString(),
                ConfidenceScore = s.LatestResult?.ConfidenceScore,
                Status = s.ProcessedAt.HasValue ? "completed" : "pending"
            };
        }).ToList();

        return Result<IReadOnlyList<ClinicScreeningHistoryDto>>.Success(results);
    }
}

