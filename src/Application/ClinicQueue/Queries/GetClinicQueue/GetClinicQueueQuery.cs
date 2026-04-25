using Application.Common.Interfaces;
using Application.Common.Models;
using Domain.Common;
using Domain.Entities.Consultation;
using Domain.Entities.Scheduling;
using Domain.Entities.Screening;
using Domain.Enums;
using Domain.Repositories;
using Microsoft.EntityFrameworkCore;

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
    public GetClinicQueueQueryHandler(
        IPatientVisitRepository patientVisitRepository,
        IRepository<AiScreening> screeningRepository,
        IConsultationSessionRepository consultationSessionRepository
        )
    {
        _patientVisitRepository = patientVisitRepository;
        _screeningRepository = screeningRepository;
        _consultationSessionRepository = consultationSessionRepository;
    }

    public async Task<Result<IReadOnlyList<ClinicQueueItemDto>>> Handle(
        GetClinicQueueQuery request,
        CancellationToken cancellationToken)
    {
        if (request.RequestedByUserId == Guid.Empty)
            return Result<IReadOnlyList<ClinicQueueItemDto>>.Failure("Invalid requester.");

        var cutoffDate = DateTime.UtcNow.AddDays(-1);

        var visits = await _patientVisitRepository
            .Query()
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
        var screenings = await _screeningRepository
            .Query()
            .Include(s => s.ScreeningResults)
            .Where(s => patientIds.Contains(s.PatientId)
                && s.CreatedAt >= cutoffDate
                && !s.IsDeleted)
            .ToListAsync(cancellationToken);

        var consultations = await _consultationSessionRepository
            .Query()
            .Where(cs => patientIds.Contains(cs.PatientId)
                && cs.CreatedAt >= cutoffDate
                && cs.Status != SessionStatus.Cancelled
                && !cs.IsDeleted)
            .ToListAsync(cancellationToken);

        var queueItems = new List<ClinicQueueItemDto>();

        foreach (var visit in visits)
        {
            // Find most recent screening for this patient
            var screening = screenings
                .Where(s => s.PatientId == visit.PatientId)
                .OrderByDescending(s => s.CreatedAt)
                .FirstOrDefault();

            // Find most recent consultation for this patient
            var consultation = consultations
                .Where(c => c.PatientId == visit.PatientId)
                .OrderByDescending(c => c.CreatedAt)
                .FirstOrDefault();
            var latestResult = screening?.ScreeningResults
                .OrderByDescending(r => r.CreatedAt)
                .FirstOrDefault();

            var item = new ClinicQueueItemDto
            {
                VisitId = visit.Id,
                PatientId = visit.PatientId,
                PatientName = visit.Patient?.FullName ?? "Unknown Patient",
                AppointmentId = visit.AppointmentId,
                VisitStatus = visit.Status.ToString(),
                CheckedInAt = visit.CheckedInAt ?? DateTime.UtcNow,
                ScreeningId = screening?.Id,
                ScreeningStatus = latestResult is not null ? "completed" : screening is not null ? "pending" : null,
                ScreeningRiskLevel = latestResult?.RiskLevel.ToString(),
                ConsultationSessionId = consultation?.Id,
                ConsultationStatus = consultation?.Status.ToString(),
                AssignedDoctorId = visit.AssignedDoctorId ?? consultation?.OphthalmologistId,
                AssignedDoctorName = null, // TODO: Load doctor name from ApplicationUser via UserId
                MedicalRecordId = visit.MedicalRecord?.Id,
                FlowState = DetermineFlowState(visit, screening, consultation)
            };

            queueItems.Add(item);
        }

        return Result<IReadOnlyList<ClinicQueueItemDto>>.Success(queueItems);
    }

    private static string DetermineFlowState(
        PatientVisit visit,
        AiScreening? screening,
        ConsultationSession? consultation)
    {
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
}
