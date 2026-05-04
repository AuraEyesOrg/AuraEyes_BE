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

        const string cacheKey = "clinic_queue_all";

        if (_cache.TryGetValue(cacheKey, out List<ClinicQueueItemDto>? cachedQueue))
        {
            return Result<IReadOnlyList<ClinicQueueItemDto>>.Success(cachedQueue!);
        }

        try
        {
            var visits = await FetchActiveVisitsAsync(cancellationToken);
            if (visits.Count == 0)
                return Result<IReadOnlyList<ClinicQueueItemDto>>.Success(Array.Empty<ClinicQueueItemDto>());

            var patientIds = visits.Select(v => v.PatientId).Distinct().ToList();
            var screeningCutoff = GetScreeningCutoff(visits);

            var (screenings, consultations) = await FetchRelatedDataAsync(patientIds, screeningCutoff, cancellationToken);
            var (doctorNames, patientNames) = await GetUserNamesAsync(visits, cancellationToken);

            var queueItems = MapVisitsToQueueItems(visits, screenings, consultations, doctorNames, patientNames);

            _cache.Set(cacheKey, queueItems, TimeSpan.FromSeconds(30));

            return Result<IReadOnlyList<ClinicQueueItemDto>>.Success(queueItems);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[CRITICAL] Error in GetClinicQueueQueryHandler: {ex.Message} \n {ex.StackTrace}");
            return Result<IReadOnlyList<ClinicQueueItemDto>>.Failure("Internal error occurred while processing queue.");
        }
    }

    private async Task<List<PatientVisit>> FetchActiveVisitsAsync(CancellationToken cancellationToken)
    {
        var cutoffDate = DateTime.UtcNow.AddDays(-1);

        return await _patientVisitRepository
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
    }

    private static DateTime GetScreeningCutoff(IEnumerable<PatientVisit> visits)
    {
        return visits
            .Where(v => v.CheckedInAt.HasValue)
            .Select(v => v.CheckedInAt!.Value)
            .DefaultIfEmpty(DateTime.UtcNow.AddDays(-1))
            .Min();
    }

    private async Task<(List<AiScreening> Screenings, List<ConsultationSession> Consultations)> FetchRelatedDataAsync(
        List<Guid> patientIds,
        DateTime screeningCutoff,
        CancellationToken cancellationToken)
    {
        // Sequential awaits are required because EF Core DbContext is not thread-safe
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

        return (screenings, consultations);
    }

    private async Task<(Dictionary<Guid, string> DoctorNames, Dictionary<Guid, string> PatientNames)> GetUserNamesAsync(
        List<PatientVisit> visits,
        CancellationToken cancellationToken)
    {
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
        if (allUserIds.Count == 0)
            return (new Dictionary<Guid, string>(), new Dictionary<Guid, string>());

        var allUsers = await _identityService.GetUsersByIdsAsync(allUserIds, cancellationToken);
        var userNameLookup = allUsers.ToDictionary(u => u.Id, u => u.FullName?.Trim());

        var doctorNames = doctorUserIds
            .Where(id => userNameLookup.ContainsKey(id))
            .ToDictionary(id => id, id => userNameLookup[id] ?? string.Empty);

        var patientNames = patientUserIds
            .Where(id => userNameLookup.ContainsKey(id))
            .ToDictionary(id => id, id => userNameLookup[id] ?? "Unknown Patient");

        return (doctorNames, patientNames);
    }

    private List<ClinicQueueItemDto> MapVisitsToQueueItems(
        List<PatientVisit> visits,
        List<AiScreening> screenings,
        List<ConsultationSession> consultations,
        Dictionary<Guid, string> doctorNames,
        Dictionary<Guid, string> patientNames)
    {
        var queueItems = new List<ClinicQueueItemDto>();
        var sortedVisits = visits.OrderBy(v => v.CheckedInAt).ThenBy(v => v.Id).ToList();

        foreach (var visit in sortedVisits)
        {
            var visitIdx = sortedVisits.IndexOf(visit);
            var nextVisitTime = sortedVisits
                .Skip(visitIdx + 1)
                .FirstOrDefault(v => v.PatientId == visit.PatientId)?.CheckedInAt;

            var screening = ResolveScreening(visit, screenings, nextVisitTime);
            var consultation = ResolveConsultation(visit, screening, consultations);
            
            queueItems.Add(MapToDto(visit, screening, consultation, doctorNames, patientNames));
        }

        return queueItems;
    }

    private static AiScreening? ResolveScreening(PatientVisit visit, List<AiScreening> screenings, DateTime? nextVisitTime)
    {
        var screening = screenings.FirstOrDefault(s => s.PatientVisitId == visit.Id);
        if (screening != null) return screening;

        var visitCheckedInAt = visit.CheckedInAt ?? DateTime.UtcNow;
        return screenings
            .Where(s => s.PatientId == visit.PatientId &&
                        s.PatientVisitId == null &&
                        s.CreatedAt >= visitCheckedInAt &&
                        (nextVisitTime == null || s.CreatedAt < nextVisitTime))
            .OrderByDescending(s => s.CreatedAt)
            .FirstOrDefault();
    }

    private static ConsultationSession? ResolveConsultation(PatientVisit visit, AiScreening? screening, List<ConsultationSession> consultations)
    {
        if (visit.MedicalRecord?.ConsultationSessionId is { } linkedCsId)
        {
            return consultations.FirstOrDefault(c => c.Id == linkedCsId);
        }

        if (screening != null && screening.PatientVisitId == visit.Id)
        {
            return consultations.FirstOrDefault(c => c.AiScreeningId == screening.Id);
        }

        return null;
    }

    private static ClinicQueueItemDto MapToDto(
        PatientVisit visit,
        AiScreening? screening,
        ConsultationSession? consultation,
        Dictionary<Guid, string> doctorNames,
        Dictionary<Guid, string> patientNames)
    {
        var latestResult = screening?.ScreeningResults
            .OrderByDescending(r => r.CreatedAt)
            .FirstOrDefault();

        var assignedDoctorName = visit.AssignedDoctor != null &&
                                 doctorNames.TryGetValue(visit.AssignedDoctor.UserId, out var dName)
            ? dName : null;

        string patientName = ResolvePatientName(visit, patientNames);

        return new ClinicQueueItemDto
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
            ScreeningStatus = latestResult != null ? "completed" : screening != null ? "pending" : null,
            ScreeningRiskLevel = latestResult?.RiskLevel.ToString(),
            ConsultationSessionId = consultation?.Id,
            ConsultationStatus = consultation?.Status.ToString(),
            AssignedDoctorId = visit.AssignedDoctorId ?? consultation?.OphthalmologistId,
            AssignedDoctorName = assignedDoctorName,
            MedicalRecordId = visit.MedicalRecord?.Id,
            IsAdminCompleted = ClinicAdministrativeErmGate.IsSatisfied(visit.MedicalRecord),
            FlowState = ClinicFlowStateResolver.Resolve(visit, screening, consultation, visit.MedicalRecord)
        };
    }

    private static string ResolvePatientName(PatientVisit visit, Dictionary<Guid, string> patientNames)
    {
        if (visit.Patient == null) return "Unknown Patient";
        
        if (visit.Patient.UserId.HasValue && patientNames.TryGetValue(visit.Patient.UserId.Value, out var pName))
            return pName;
        
        return (visit.Patient.IsWalkIn && !string.IsNullOrWhiteSpace(visit.Patient.FullName))
            ? visit.Patient.FullName
            : "Unknown Patient";
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
