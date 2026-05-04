using Application.ClinicQueue.Common;
using Application.Common.Interfaces;
using Application.Common.Models;
using Domain.Common;
using Domain.Entities.Consultation;
using Domain.Entities.MedicalRecords;
using Domain.Entities.Scheduling;
using Domain.Entities.Screening;
using Domain.Enums;
using Domain.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;

namespace Application.ClinicQueue.Queries.GetClinicQueue;

public record GetClinicQueueQuery : IQuery<IReadOnlyList<ClinicQueueItemDto>>
{
    public Guid RequestedByUserId { get; init; }
}

public class GetClinicQueueQueryHandler
    : IQueryHandler<GetClinicQueueQuery, IReadOnlyList<ClinicQueueItemDto>>
{
    private readonly IPatientVisitRepository _patientVisitRepository;
    private readonly IRepository<AiScreening> _screeningRepository;
    private readonly IConsultationSessionRepository _consultationSessionRepository;
    private readonly IIdentityService _identityService;
    private readonly IMemoryCache _cache;

    public GetClinicQueueQueryHandler(
        IPatientVisitRepository patientVisitRepository,
        IRepository<AiScreening> screeningRepository,
        IConsultationSessionRepository consultationSessionRepository,
        IIdentityService identityService,
        IMemoryCache cache
        )
    {
        _patientVisitRepository = patientVisitRepository;
        _screeningRepository = screeningRepository;
        _consultationSessionRepository = consultationSessionRepository;
        _identityService = identityService;
        _cache = cache;
    }

    public async Task<Result<IReadOnlyList<ClinicQueueItemDto>>> Handle(
        GetClinicQueueQuery request,
        CancellationToken cancellationToken)
    {
        if (request.RequestedByUserId == Guid.Empty)
            return Result<IReadOnlyList<ClinicQueueItemDto>>.Failure("Invalid requester.");

        string cacheKey = $"clinic_queue_all";

        // 1. Fast path: Check cache
        if (_cache.TryGetValue(cacheKey, out List<ClinicQueueItemDto>? cachedQueue))
        {
            return Result<IReadOnlyList<ClinicQueueItemDto>>.Success(cachedQueue!);
        }

        try
        {
            var cutoffDate = DateTime.UtcNow.AddDays(-1);

            var visits = await _patientVisitRepository
                .Query()
                .AsNoTracking()
                .Include(v => v.Patient)
                .Include(v => v.Appointment)
                    .ThenInclude(a => a!.AppointmentSlot)
                        .ThenInclude(s => s!.ScheduleTemplate)
                .Include(v => v.AssignedDoctor)
                .Include(v => v.MedicalRecord)
                .Where(v =>
                    v.Appointment != null &&
                    v.Appointment.AppointmentSlot != null &&
                    v.CheckedInAt >= cutoffDate &&
                    v.Status != PatientVisitStatus.Completed)
                .OrderBy(v => v.CheckedInAt)
                .ToListAsync(cancellationToken);

            if (visits.Count == 0)
                return Result<IReadOnlyList<ClinicQueueItemDto>>.Success(Array.Empty<ClinicQueueItemDto>());

            var patientIds = visits.Select(v => v.PatientId).Distinct().ToList();
            
            // Use the earliest CheckedInAt among active visits as the cutoff — avoids missing data
            // for visits that started before the rolling 24h window.
            var screeningCutoff = visits
                .Where(v => v.CheckedInAt.HasValue)
                .Select(v => v.CheckedInAt!.Value)
                .DefaultIfEmpty(DateTime.UtcNow.AddDays(-1))
                .Min();

            var screenings = await _screeningRepository
                .Query()
                .AsNoTracking()
                .Include(s => s.ScreeningResults)
                .Where(s => patientIds.Contains(s.PatientId)
                    && s.CreatedAt >= screeningCutoff
                    && !s.IsDeleted)
                .ToListAsync(cancellationToken);

            var consultations = await _consultationSessionRepository
                .Query()
                .AsNoTracking()
                .Where(cs => patientIds.Contains(cs.PatientId)
                    && cs.CreatedAt >= screeningCutoff
                    && cs.Status != SessionStatus.Cancelled
                    && !cs.IsDeleted)
                .ToListAsync(cancellationToken);

            // --- Optimized User Lookup (Batch) ---
            var doctorUserIds = visits
                .Where(v => v.AssignedDoctor != null)
                .Select(v => v.AssignedDoctor!.UserId)
                .Distinct()
                .ToList();

            var patientUserIds = visits
                .Where(v => v.Patient != null && v.Patient.UserId.HasValue)
                .Select(v => v.Patient!.UserId!.Value)
                .Distinct()
                .ToList();

            var allUserIds = doctorUserIds.Concat(patientUserIds).Distinct().ToList();
            var allUsers = allUserIds.Count > 0
                ? await _identityService.GetUsersByIdsAsync(allUserIds, cancellationToken)
                : Array.Empty<UserDto>();
            
            var userNameLookup = allUsers.ToDictionary(u => u.Id, u => u.FullName?.Trim());

            var doctorNameByUserId = new Dictionary<Guid, string>();
            foreach (var id in doctorUserIds)
            {
                if (userNameLookup.TryGetValue(id, out var name))
                    doctorNameByUserId[id] = name ?? string.Empty;
            }

            var patientNameByUserId = new Dictionary<Guid, string>();
            foreach (var id in patientUserIds)
            {
                if (userNameLookup.TryGetValue(id, out var name))
                    patientNameByUserId[id] = name ?? "Unknown Patient";
            }

            var queueItems = new List<ClinicQueueItemDto>();

            var sortedVisits = visits.OrderBy(v => v.CheckedInAt).ThenBy(v => v.Id).ToList();

            foreach (var visit in visits)
            {
                var visitCheckedInAt = visit.CheckedInAt ?? DateTime.UtcNow;

                // Find the next visit of the same patient in the sorted list to define the time boundary
                var nextVisitOfSamePatient = sortedVisits
                    .Skip(sortedVisits.IndexOf(visit) + 1)
                    .FirstOrDefault(v => v.PatientId == visit.PatientId);
                
                var nextVisitTime = nextVisitOfSamePatient?.CheckedInAt;

                var screening = screenings
                    .FirstOrDefault(s => s.PatientVisitId == visit.Id);

                if (screening is null)
                {
                    screening = screenings
                        .Where(s => s.PatientId == visit.PatientId &&
                                    s.PatientVisitId == null &&
                                    s.CreatedAt >= visitCheckedInAt &&
                                    (nextVisitTime == null || s.CreatedAt < nextVisitTime))
                        .OrderByDescending(s => s.CreatedAt)
                        .FirstOrDefault();
                }

                ConsultationSession? consultation = null;
                var linkedConsultationId = visit.MedicalRecord?.ConsultationSessionId;
                if (linkedConsultationId.HasValue)
                {
                    consultation = consultations.FirstOrDefault(c => c.Id == linkedConsultationId.Value);
                }

                if (consultation is null)
                {
                    consultation = consultations
                        .Where(c => c.PatientId == visit.PatientId &&
                                    c.CreatedAt >= visitCheckedInAt &&
                                    (nextVisitTime == null || c.CreatedAt < nextVisitTime))
                        .OrderByDescending(c => c.CreatedAt)
                        .FirstOrDefault();
                }
                    
                var latestResult = screening?.ScreeningResults
                    .OrderByDescending(r => r.CreatedAt)
                    .FirstOrDefault();
                
                var assignedDoctorName = visit.AssignedDoctor is not null &&
                                         doctorNameByUserId.TryGetValue(visit.AssignedDoctor.UserId, out var dName)
                    ? dName
                    : null;

                string patientName = "Unknown Patient";
                if (visit.Patient != null)
                {
                    if (visit.Patient.UserId.HasValue && patientNameByUserId.TryGetValue(visit.Patient.UserId.Value, out var pName))
                    {
                        patientName = pName;
                    }
                    else if (visit.Patient.IsWalkIn && !string.IsNullOrWhiteSpace(visit.Patient.FullName))
                    {
                        patientName = visit.Patient.FullName;
                    }
                }

                var item = new ClinicQueueItemDto
                {
                    VisitId = visit.Id,
                    PatientId = visit.PatientId,
                    PatientName = patientName,
                    PatientAge = visit.Patient != null ? CalculateAge(visit.Patient.DateOfBirth) : null,
                    PatientGender = visit.Patient != null ? ((Gender?)visit.Patient.GenderId)?.ToString() : null,
                    CitizenId = visit.Patient?.CitizenId,
                    AppointmentId = visit.AppointmentId,
                    VisitStatus = visit.Status.ToString(),
                    CheckedInAt = visit.CheckedInAt ?? DateTime.UtcNow,
                    ScreeningId = screening?.Id,
                    ScreeningStatus = latestResult is not null ? "completed" : screening is not null ? "pending" : null,
                    ScreeningRiskLevel = latestResult?.RiskLevel.ToString(),
                    ConsultationSessionId = consultation?.Id,
                    ConsultationStatus = consultation?.Status.ToString(),
                    AssignedDoctorId = visit.AssignedDoctorId ?? consultation?.OphthalmologistId,
                    AssignedDoctorName = assignedDoctorName,
                    MedicalRecordId = visit.MedicalRecord?.Id,
                    IsAdminCompleted = ClinicAdministrativeErmGate.IsSatisfied(visit.MedicalRecord),

                    // Integration with the new business logic resolver
                    FlowState = ClinicFlowStateResolver.Resolve(visit, screening, consultation, visit.MedicalRecord)
                };

                queueItems.Add(item);
            }

            // Cache for 30 seconds to survive load tests
            _cache.Set(cacheKey, queueItems, TimeSpan.FromSeconds(30));

            return Result<IReadOnlyList<ClinicQueueItemDto>>.Success(queueItems);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[CRITICAL] Error in GetClinicQueueQueryHandler: {ex.Message} \n {ex.StackTrace}");
            return Result<IReadOnlyList<ClinicQueueItemDto>>.Failure("Internal error occurred while processing queue.");
        }
    }

    private static int? CalculateAge(DateTime? dob)
    {
        if (!dob.HasValue) return null;
        var today = DateTime.UtcNow;
        var age = today.Year - dob.Value.Year;
        if (dob.Value.Date > today.AddYears(-age)) age--;
        return age;
    }
}

public class ClinicQueueItemDto
{
    public Guid VisitId { get; set; }
    public Guid PatientId { get; set; }
    public string PatientName { get; set; } = string.Empty;
    public int? PatientAge { get; set; }
    public string? PatientGender { get; set; }
    public string? CitizenId { get; set; }
    public Guid? AppointmentId { get; set; }
    public string VisitStatus { get; set; } = string.Empty;
    public DateTime CheckedInAt { get; set; }
    public Guid? ScreeningId { get; set; }
    public string? ScreeningStatus { get; set; }
    public string? ScreeningRiskLevel { get; set; }
    public Guid? ConsultationSessionId { get; set; }
    public string? ConsultationStatus { get; set; }
    public Guid? AssignedDoctorId { get; set; }
    public string? AssignedDoctorName { get; set; }
    public Guid? MedicalRecordId { get; set; }
    public string FlowState { get; set; } = string.Empty;
    public bool IsAdminCompleted { get; set; }
}
