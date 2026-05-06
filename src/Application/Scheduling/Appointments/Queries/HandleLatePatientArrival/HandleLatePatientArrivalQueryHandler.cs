using Application.Common.Helpers;
using Application.Common.Interfaces;
using Application.Common.Models;
using Domain.Common;
using Domain.Entities.Scheduling;
using Domain.Enums;
using Domain.Repositories;

namespace Application.Scheduling.Appointments.Queries.HandleLatePatientArrival;

public class HandleLatePatientArrivalQueryHandler
    : IQueryHandler<HandleLatePatientArrivalQuery, LateArrivalCheckResult>
{
    private readonly IAppointmentRepository _appointmentRepository;
    private readonly IAppointmentSlotRepository _appointmentSlotRepository;

    public HandleLatePatientArrivalQueryHandler(
        IAppointmentRepository appointmentRepository,
        IAppointmentSlotRepository appointmentSlotRepository)
    {
        _appointmentRepository = appointmentRepository;
        _appointmentSlotRepository = appointmentSlotRepository;
    }

    public async Task<Result<LateArrivalCheckResult>> Handle(
        HandleLatePatientArrivalQuery request,
        CancellationToken cancellationToken)
    {
        var appointment = await _appointmentRepository.GetByIdWithDetailsAsync(
            request.AppointmentId, cancellationToken);

        if (appointment is null)
            return Result<LateArrivalCheckResult>.NotFound(
                $"Appointment '{request.AppointmentId}' not found.");

        if (appointment.AppointmentSlot is null)
            return Result<LateArrivalCheckResult>.Failure(
                "Appointment is not linked to a slot.");

        var slot = appointment.AppointmentSlot;
        var utcNow = DateTime.UtcNow;
        var localNow = TimeZoneInfo.ConvertTimeFromUtc(utcNow, VietnamTimeZoneResolver.TimeZone);
        var slotStart = slot.Date.ToDateTime(slot.StartTime);
        var slotEnd = slot.Date.ToDateTime(slot.EndTime);
        var duration = slotEnd - slotStart;
        var threshold = TimeSpan.FromTicks(duration.Ticks / 3);
        var thresholdTime = slotStart.Add(threshold);

        var isLate = localNow > thresholdTime;
        var lateMinutes = isLate ? (int)(localNow - thresholdTime).TotalMinutes : 0;
        var thresholdMinutes = (int)threshold.TotalMinutes;

        var availableSlots = new List<AvailableSlotOption>();
        var adHocDefaults = new AdHocDefaults
        {
            SuggestedStartTime = slot.EndTime,
            DurationMinutes = (int)duration.TotalMinutes,
            DefaultCapacity = slot.MaxCapacity,
            DefaultCost = slot.Cost
        };

        if (isLate)
        {
            // Find available slots on the same day with remaining capacity
            var allSlots = await _appointmentSlotRepository.GetByDateRangeAsync(
                slot.Date, slot.Date, cancellationToken);

            availableSlots = allSlots
                .Where(s =>
                    s.Id != slot.Id &&
                    s.Status == ScheduleStatus.Available &&
                    s.RemainingCapacity > 0 &&
                    s.Date == slot.Date &&
                    s.StartTime >= slot.StartTime)
                .OrderBy(s => s.StartTime)
                .Select(s => new AvailableSlotOption
                {
                    SlotId = s.Id,
                    StartTime = s.StartTime,
                    EndTime = s.EndTime,
                    RemainingCapacity = s.RemainingCapacity,
                    Cost = s.Cost
                })
                .ToList();
        }

        return Result<LateArrivalCheckResult>.Success(new LateArrivalCheckResult
        {
            IsLate = isLate,
            LateMinutes = lateMinutes,
            ThresholdMinutes = thresholdMinutes,
            CurrentSlotId = slot.Id,
            SlotDate = slot.Date,
            SlotStartTime = slot.StartTime,
            SlotEndTime = slot.EndTime,
            SlotDurationMinutes = (int)duration.TotalMinutes,
            AvailableSlots = availableSlots,
            AdHocDefaults = adHocDefaults
        });
    }
}
