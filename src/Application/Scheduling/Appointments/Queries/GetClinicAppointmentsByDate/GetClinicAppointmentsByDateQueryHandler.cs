using Application.ClinicQueue.Common;
using Application.Common.Interfaces;
using Application.Common.Models;
using Application.Scheduling.Appointments.Common;
using Domain.Common;
using Domain.Entities.Consultation;
using Domain.Entities.Screening;
using Domain.Enums;
using Domain.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Application.Scheduling.Appointments.Queries.GetClinicAppointmentsByDate;

public class GetClinicAppointmentsByDateQueryHandler
    : IQueryHandler<GetClinicAppointmentsByDateQuery, IReadOnlyList<ClinicAppointmentDto>>
{
    private readonly IAppointmentRepository _appointmentRepository;
    private readonly IOrderRepository _orderRepository;
    private readonly IPatientVisitRepository _patientVisitRepository;
    private readonly IRepository<AiScreening> _screeningRepository;
    private readonly IConsultationSessionRepository _consultationSessionRepository;
    private readonly IOphthalmologistRepository _ophthalmologistRepository;
    private readonly IIdentityService _identityService;

    public GetClinicAppointmentsByDateQueryHandler(
        IAppointmentRepository appointmentRepository,
        IOrderRepository orderRepository,
        IPatientVisitRepository patientVisitRepository,
        IRepository<AiScreening> screeningRepository,
        IConsultationSessionRepository consultationSessionRepository,
        IOphthalmologistRepository ophthalmologistRepository,
        IIdentityService identityService)
    {
        _appointmentRepository = appointmentRepository;
        _orderRepository = orderRepository;
        _patientVisitRepository = patientVisitRepository;
        _screeningRepository = screeningRepository;
        _consultationSessionRepository = consultationSessionRepository;
        _ophthalmologistRepository = ophthalmologistRepository;
        _identityService = identityService;
    }

    public async Task<Result<IReadOnlyList<ClinicAppointmentDto>>> Handle(
        GetClinicAppointmentsByDateQuery request,
        CancellationToken cancellationToken)
    {
        var (appointments, _) = await _appointmentRepository.GetPagedAsync(
            fromDate: request.Date,
            toDate: request.Date,
            pageNumber: 1,
            pageSize: 500,
            cancellationToken: cancellationToken);

        var appointmentIds = appointments.Select(a => a.Id).ToList();

        var orders = await _orderRepository.GetByAppointmentIdsAsync(appointmentIds, cancellationToken);
        
        var visits = await _patientVisitRepository.Query().AsNoTracking()
            .Where(v => v.AppointmentId.HasValue && appointmentIds.Contains(v.AppointmentId.Value))
            .Include(v => v.MedicalRecord)
            .ToListAsync(cancellationToken);
        
        var registeredUserIds = appointments
            .Where(a => a.Patient?.UserId != null)
            .Select(a => a.Patient!.UserId!.Value)
            .Distinct()
            .ToList();

        var userMap = (await _identityService.GetUsersByIdsAsync(registeredUserIds, cancellationToken))
            .ToDictionary(u => u.Id);

        var orderLookup = orders.ToLookup(o => o.AppointmentId!.Value);
        var visitMap = visits.ToDictionary(v => v.AppointmentId!.Value);
        var visitPatientIds = visits.Select(v => v.PatientId).Distinct().ToList();

        // Use the earliest CheckedInAt among active visits as the cutoff to avoid missing
        // screenings/consultations for visits that started before the rolling 24h window.
        var screeningCutoff = visits
            .Where(v => v.CheckedInAt.HasValue)
            .Select(v => v.CheckedInAt!.Value)
            .DefaultIfEmpty(DateTime.UtcNow.AddDays(-1))
            .Min();

        var screenings = visitPatientIds.Count == 0
            ? new List<AiScreening>()
            : await _screeningRepository
                .Query().AsNoTracking()
                .Include(s => s.ScreeningResults)
                .Where(s =>
                    visitPatientIds.Contains(s.PatientId)
                    && s.CreatedAt >= screeningCutoff
                    && !s.IsDeleted)
                .ToListAsync(cancellationToken);

        var consultations = visitPatientIds.Count == 0
            ? new List<ConsultationSession>()
            : await _consultationSessionRepository
                .Query().AsNoTracking()
                .Where(cs =>
                    visitPatientIds.Contains(cs.PatientId)
                    && cs.CreatedAt >= screeningCutoff
                    && cs.Status != SessionStatus.Cancelled
                    && !cs.IsDeleted)
                .ToListAsync(cancellationToken);

        var doctorIds = appointments
            .Select(a => a.RequestedDoctorId ?? a.AppointmentSlot?.OphthalId)
            .Where(id => id.HasValue)
            .Select(id => id!.Value)
            .Union(visits.Where(v => v.AssignedDoctorId.HasValue).Select(v => v.AssignedDoctorId!.Value))
            .Distinct()
            .ToList();

        var doctorMap = await _ophthalmologistRepository.GetEnhancedDoctorDetailsByIdsAsync(doctorIds, cancellationToken);

        var sortedVisits = visits.OrderBy(v => v.CheckedInAt).ThenBy(v => v.Id).ToList();
        var items = new List<ClinicAppointmentDto>();

        foreach (var a in appointments.Where(x => x.AppointmentSlot is not null && x.Status != AppointmentStatus.Cancelled && x.Status != AppointmentStatus.NoShow))
        {
            var appointmentOrders = orderLookup[a.Id].ToList();
            var primaryOrder = appointmentOrders.OrderByDescending(o => o.CreatedAt).FirstOrDefault();

            decimal totalAmount = appointmentOrders.Sum(o => o.TotalAmount);
            decimal paidAmount = appointmentOrders.SelectMany(o => o.Payments)
                .Where(p => p.Status == PaymentStatus.Completed)
                .Sum(p => p.Amount);
            decimal remaining = totalAmount - paidAmount;

            bool isPaidDeposit = appointmentOrders.Count == 0
                || appointmentOrders.All(o => o.IsClinicDepositSatisfiedForCheckIn());

            visitMap.TryGetValue(a.Id, out var visit);

            string? flowState = null;
            string? visitStatus = null;
            if (visit != null)
            {
                visitStatus = visit.Status.ToString();
                var visitCheckedInAt = visit.CheckedInAt ?? DateTime.UtcNow;

                // Find the next visit of the same patient in the sorted list to define the time boundary
                var nextVisitOfSamePatient = sortedVisits
                    .Skip(sortedVisits.IndexOf(visit) + 1)
                    .FirstOrDefault(v => v.PatientId == visit.PatientId);
                
                var nextVisitTime = nextVisitOfSamePatient?.CheckedInAt;

                var screening = screenings.FirstOrDefault(s => s.PatientVisitId == visit.Id);
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
                if (visit.MedicalRecord?.ConsultationSessionId is { } linkedCsId)
                {
                    consultation = consultations.FirstOrDefault(c => c.Id == linkedCsId);
                }

                if (consultation is null &&
                    screening is not null &&
                    screening.PatientVisitId == visit.Id)
                {
                    consultation = consultations.FirstOrDefault(c => c.AiScreeningId == screening.Id);
                }

                flowState = ClinicFlowStateResolver.Resolve(visit, screening, consultation, visit.MedicalRecord);
            }

            var finalDocId = visit?.AssignedDoctorId ?? a.RequestedDoctorId ?? a.AppointmentSlot?.OphthalId;
            doctorMap.TryGetValue(finalDocId ?? Guid.Empty, out var doc);

            string patientName = "Patient";
            if (a.Patient != null)
            {
                if (a.Patient.IsWalkIn)
                {
                    patientName = a.Patient.FullName ?? "Patient";
                }
                else if (a.Patient.UserId.HasValue && userMap.TryGetValue(a.Patient.UserId.Value, out var user))
                {
                    patientName = !string.IsNullOrWhiteSpace(user.FullName) ? user.FullName : (user.Email ?? "Patient");
                }
            }

            items.Add(new ClinicAppointmentDto
            {
                Id = a.Id,
                PatientId = a.PatientId,
                PatientName = patientName,
                PatientAvatarUrl = null,
                SlotId = a.AppointmentSlotId,
                Date = a.AppointmentSlot!.Date,
                StartTime = a.AppointmentSlot.StartTime,
                EndTime = a.AppointmentSlot.EndTime,
                VisitReason = a.VisitReason,
                Status = a.Status.ToString(),
                VisitStatus = visitStatus,
                FlowState = flowState,
                CreatedAt = a.CreatedAt,
                // SubmittedFeedbackTargets defaults to empty; HasFeedback is computed from it

                OphthalId = a.AppointmentSlot.OphthalId,
                OphthalFullName = doc.FullName ?? "Clinic Doctor",
                OphthalAvatarUrl = doc.AvatarUrl,

                OrderId = primaryOrder?.Id,
                TotalAmount = totalAmount > 0 ? totalAmount : null,
                DepositAmount = primaryOrder?.DepositAmount,
                IsPaidDeposit = isPaidDeposit,
                PaidAmount = paidAmount,
                RemainingAmount = totalAmount > 0 ? (remaining > 0 ? remaining : 0) : null,
                    OrderStatus = primaryOrder?.Status switch
                {
                    OrderStatus.Confirmed => "PartiallyPaid",
                    OrderStatus.Completed => "FullyPaid",
                    _ => primaryOrder?.Status.ToString()
                },

                // Refund info
                RefundBankNumber = a.RefundBankNumber,
                RefundAccountName = a.RefundAccountName,
                RefundBankName = a.RefundBankName,
                CancellationReason = a.CancellationReason
            });
        }

        items = items
            .OrderBy(x => x.Date)
            .ThenBy(x => x.StartTime)
            .ToList();

        return Result<IReadOnlyList<ClinicAppointmentDto>>.Success(items);
    }
}

