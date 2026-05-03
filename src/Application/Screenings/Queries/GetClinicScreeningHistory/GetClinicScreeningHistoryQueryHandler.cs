using Application.Common.Interfaces;
using Application.Common.Models;
using Domain.Common;
using Domain.Entities.Screening;
using Domain.Entities.Users;
using Microsoft.EntityFrameworkCore;

namespace Application.Screenings.Queries.GetClinicScreeningHistory;

public class GetClinicScreeningHistoryQueryHandler
    : IQueryHandler<GetClinicScreeningHistoryQuery, IReadOnlyList<ClinicScreeningHistoryDto>>
{
    private readonly IRepository<AiScreening> _screeningRepository;
    private readonly IRepository<Patient> _patientRepository;
    private readonly IIdentityService _identityService;
    private readonly ICurrentUserService _currentUserService;

    public GetClinicScreeningHistoryQueryHandler(
        IRepository<AiScreening> screeningRepository,
        IRepository<Patient> patientRepository,
        IIdentityService identityService,
        ICurrentUserService currentUserService)
    {
        _screeningRepository = screeningRepository;
        _patientRepository = patientRepository;
        _identityService = identityService;
        _currentUserService = currentUserService;
    }

    public async Task<Result<IReadOnlyList<ClinicScreeningHistoryDto>>> Handle(
        GetClinicScreeningHistoryQuery request,
        CancellationToken cancellationToken)
    {
        // Clinic staff only
        if (!_currentUserService.IsAuthenticated)
            return Result<IReadOnlyList<ClinicScreeningHistoryDto>>.Unauthorized("User not authenticated");

        var take = Math.Clamp(request.Take, 1, 100);

        // Fetch screenings with related data
        var screenings = await _screeningRepository
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
                s.RetinalImages.Count,
                LatestResult = s.ScreeningResults
                    .OrderByDescending(r => r.CreatedAt)
                    .Select(r => new { r.RiskLevel, r.ConfidenceScore })
                    .FirstOrDefault()
            })
            .ToListAsync(cancellationToken);

        if (!screenings.Any())
            return Result<IReadOnlyList<ClinicScreeningHistoryDto>>.Success(new List<ClinicScreeningHistoryDto>());

        // Get unique patient IDs
        var patientIds = screenings.Select(s => s.PatientId).Distinct().ToList();

        // Fetch patient profiles
        var patients = await _patientRepository
            .Query().AsNoTracking()
            .Where(p => patientIds.Contains(p.Id))
            .Select(p => new { p.Id, p.UserId, p.FullName })
            .ToListAsync(cancellationToken);

        // Map patient IDs to user IDs for registered patients
        var userIdMap = patients
            .Where(p => p.UserId.HasValue)
            .ToDictionary(p => p.UserId!.Value, p => p.Id);

        // Fetch user names from Identity
        var identityUsers = await _identityService.GetUsersByIdsAsync(userIdMap.Keys, cancellationToken);
        var userNameMap = identityUsers.ToDictionary(u => u.Id, u => u.FullName);

        // Combine data
        var result = screenings.Select(s =>
        {
            var patient = patients.FirstOrDefault(p => p.Id == s.PatientId);
            string patientName = "Unknown";

            if (patient != null)
            {
                if (patient.UserId.HasValue && userNameMap.TryGetValue(patient.UserId.Value, out var name))
                {
                    patientName = name;
                }
                else if (!string.IsNullOrEmpty(patient.FullName))
                {
                    patientName = patient.FullName;
                }
            }

            var status = s.ProcessedAt.HasValue ? (s.LatestResult != null ? "completed" : "pending") : "pending";
            // Map status correctly based on frontend expectations
            if (s.ProcessedAt.HasValue && s.LatestResult != null)
            {
                status = "completed";
            }

            return new ClinicScreeningHistoryDto
            {
                ScreeningId = s.Id,
                PatientId = s.PatientId,
                PatientName = patientName,
                CreatedAt = s.CreatedAt,
                ProcessedAt = s.ProcessedAt,
                ImagesCount = s.Count,
                LatestRiskLevel = s.LatestResult?.RiskLevel.ToString(),
                ConfidenceScore = s.LatestResult?.ConfidenceScore,
                AiPrimaryLabel = s.LatestResult?.RiskLevel.ToString(),
                Status = status
            };
        }).ToList();

        return Result<IReadOnlyList<ClinicScreeningHistoryDto>>.Success(result);
    }
}

