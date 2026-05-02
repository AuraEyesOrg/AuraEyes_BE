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

    private static readonly System.Collections.Concurrent.ConcurrentDictionary<string, System.Threading.SemaphoreSlim> _semaphores = new();
    
    public async Task<Result<IReadOnlyList<ClinicQueueItemDto>>> Handle(
        GetClinicQueueQuery request,
        CancellationToken cancellationToken)
    {
        if (request.RequestedByUserId == Guid.Empty)
            return Result<IReadOnlyList<ClinicQueueItemDto>>.Failure("Invalid requester.");

        string cacheKey = $"clinic_queue_{request.RequestedByUserId}";
        
        // 1. Fast path: check cache
        if (_cache.TryGetValue(cacheKey, out IReadOnlyList<ClinicQueueItemDto>? cachedQueue))
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
            {
                var emptyResult = Array.Empty<ClinicQueueItemDto>();
                _cache.Set(cacheKey, emptyResult, TimeSpan.FromSeconds(10));
                return Result<IReadOnlyList<ClinicQueueItemDto>>.Success(emptyResult);
            }

            var patientIds = visits.Select(v => v.PatientId).Distinct().ToList();
            var screenings = await _screeningRepository
                .Query()
                .AsNoTracking()
                .Include(s => s.ScreeningResults)
                .Where(s => patientIds.Contains(s.PatientId)
                    && s.CreatedAt >= cutoffDate
                    && !s.IsDeleted)
                .ToListAsync(cancellationToken);

            var consultations = await _consultationSessionRepository
                .Query()
                .AsNoTracking()
                .Where(cs => patientIds.Contains(cs.PatientId)
                    && cs.CreatedAt >= cutoffDate
                    && cs.Status != SessionStatus.Cancelled
                    && !cs.IsDeleted)
                .ToListAsync(cancellationToken);

            var doctorUserIds = visits
                .Where(v => v.AssignedDoctor != null)
                .Select(v => v.AssignedDoctor!.UserId)
                .Distinct()
                .ToList();
            var doctorUsers = doctorUserIds.Count > 0
                ? await _identityService.GetUsersByIdsAsync(doctorUserIds, cancellationToken)
                : Array.Empty<UserDto>();
            
            // Safer way to build dictionary (handles potential duplicates from service gracefully)
            var doctorNameByUserId = new Dictionary<Guid, string>();
            foreach (var u in doctorUsers)
            {
                doctorNameByUserId[u.Id] = u.FullName?.Trim() ?? string.Empty;
            }

            var patientUserIds = visits
                .Where(v => v.Patient != null && v.Patient.UserId.HasValue)
                .Select(v => v.Patient!.UserId!.Value)
                .Distinct()
                .ToList();

            var patientUsers = patientUserIds.Count > 0
                ? await _identityService.GetUsersByIdsAsync(patientUserIds, cancellationToken)
                : Array.Empty<UserDto>();
            
            var patientNameByUserId = new Dictionary<Guid, string>();
            foreach (var u in patientUsers)
            {
                patientNameByUserId[u.Id] = u.FullName?.Trim() ?? "Unknown Patient";
            }

            var queueItems = new List<ClinicQueueItemDto>();

            foreach (var visit in visits)
            {
                var screening = screenings
                    .Where(s => s.PatientId == visit.PatientId)
                    .OrderByDescending(s => s.CreatedAt)
                    .FirstOrDefault();

                var consultation = consultations
                    .Where(c => c.PatientId == visit.PatientId)
                    .OrderByDescending(c => c.CreatedAt)
                    .FirstOrDefault();
                    
                var latestResult = screening?.ScreeningResults
                    .OrderByDescending(r => r.CreatedAt)
                    .FirstOrDefault();
                
                var assignedDoctorName = visit.AssignedDoctor is not null &&
                                         doctorNameByUserId.TryGetValue(visit.AssignedDoctor.UserId, out var doctorName)
                    ? doctorName
                    : null;

                string patientName = "Unknown Patient";
                if (visit.Patient != null)
                {
                    if (visit.Patient.UserId.HasValue && patientNameByUserId.TryGetValue(visit.Patient.UserId.Value, out var name))
                    {
                        patientName = name;
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
                    IsAdminCompleted = visit.MedicalRecord != null && visit.MedicalRecord.Status != MedicalRecordStatus.DraftAdmin,

                    FlowState = DetermineFlowState(visit, screening, consultation)
                };

                queueItems.Add(item);
            }

            // Cache the results for 30 seconds to survive heavy load
            _cache.Set(cacheKey, queueItems, TimeSpan.FromSeconds(30));

            return Result<IReadOnlyList<ClinicQueueItemDto>>.Success(queueItems);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[CRITICAL] Error in GetClinicQueueQueryHandler: {ex.Message} \n {ex.StackTrace}");
            return Result<IReadOnlyList<ClinicQueueItemDto>>.Failure("An internal error occurred while fetching the clinic queue.");
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

    private static string DetermineFlowState(
        PatientVisit visit,
        AiScreening? screening,
        ConsultationSession? consultation)
    {
        // Doctor finalized and handed off to cashier.
        // This is the canonical state for cashier intake.
        if (visit.Status == PatientVisitStatus.WaitingForPayment)
            return "Finalized";

        // If visit completed, flow is finalized
        if (visit.Status == PatientVisitStatus.Completed)
            return "Finalized";

        // If consultation session exists and is active
        if (consultation != null)
        {
            if (consultation.Status == SessionStatus.Confirmed)
                return "ConsultationInProgress";
            if (consultation.Status == SessionStatus.Completed)
                return "Finalized";
            return "SentToDoctor"; // Pending consultation
        }

        // If screening exists
        if (screening != null)
        {
            // Check if screening has results
            if (screening.ScreeningResults.Count > 0)
                return "AICompleted";
            return "ScreeningPending";
        }

        // Default: just checked in
        return "CheckedIn";
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
