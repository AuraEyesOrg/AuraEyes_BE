using Application.Common.Models;
using Domain.Common;
using Domain.Entities.Consultation;
using Domain.Entities.Scheduling;
using Domain.Repositories;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Application.SystemAdmin.Ophthalmologists.Commands.DeleteFutureOphthalmologistSlots;

/// <summary>
/// Deletes all slots from today onward for one ophthalmologist while preserving historical slots.
/// Slots linked to appointments or consultation sessions are protected.
/// </summary>
public class DeleteFutureOphthalmologistSlotsCommandHandler
    : IRequestHandler<DeleteFutureOphthalmologistSlotsCommand, Result<DeleteFutureOphthalmologistSlotsResultDto>>
{
    private readonly IOphthalmologistRepository _ophthalmologistRepository;
    private readonly IAppointmentSlotRepository _appointmentSlotRepository;
    private readonly IRepository<Appointment> _appointmentRepository;
    private readonly IRepository<ConsultationSession> _consultationSessionRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<DeleteFutureOphthalmologistSlotsCommandHandler> _logger;

    public DeleteFutureOphthalmologistSlotsCommandHandler(
        IOphthalmologistRepository ophthalmologistRepository,
        IAppointmentSlotRepository appointmentSlotRepository,
        IRepository<Appointment> appointmentRepository,
        IRepository<ConsultationSession> consultationSessionRepository,
        IUnitOfWork unitOfWork,
        ILogger<DeleteFutureOphthalmologistSlotsCommandHandler> logger)
    {
        _ophthalmologistRepository = ophthalmologistRepository;
        _appointmentSlotRepository = appointmentSlotRepository;
        _appointmentRepository = appointmentRepository;
        _consultationSessionRepository = consultationSessionRepository;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<Result<DeleteFutureOphthalmologistSlotsResultDto>> Handle(
        DeleteFutureOphthalmologistSlotsCommand request,
        CancellationToken cancellationToken)
    {
        if (request.OphthalmologistId == Guid.Empty)
        {
            return Result<DeleteFutureOphthalmologistSlotsResultDto>.Failure("Ophthalmologist ID is required.");
        }

        var ophthalmologist = await _ophthalmologistRepository.GetByIdAsync(request.OphthalmologistId, cancellationToken);
        if (ophthalmologist is null)
        {
            return Result<DeleteFutureOphthalmologistSlotsResultDto>.NotFound(
                $"Ophthalmologist '{request.OphthalmologistId}' was not found.");
        }

        var vietnamNow = GetVietnamNow();
        var today = DateOnly.FromDateTime(vietnamNow);
        var currentTimeUtc = TimeOnly.FromDateTime(DateTime.UtcNow);

        var futureSlots = await _appointmentSlotRepository
            .Query()
            .Where(slot => slot.ScheduleTemplate != null && slot.ScheduleTemplate.OphthalId == request.OphthalmologistId)
            .Where(slot => slot.Date >= today)
            .OrderBy(slot => slot.Date)
            .ThenBy(slot => slot.StartTime)
            .ToListAsync(cancellationToken);

        if (futureSlots.Count == 0)
        {
            return Result<DeleteFutureOphthalmologistSlotsResultDto>.Success(new DeleteFutureOphthalmologistSlotsResultDto
            {
                OphthalmologistId = request.OphthalmologistId,
                Today = today,
                CurrentTimeUtc = currentTimeUtc,
                MatchedFutureSlots = 0,
                DeletedSlots = 0,
                ProtectedSlots = 0
            });
        }

        var futureSlotIds = futureSlots
            .Select(slot => slot.Id)
            .ToHashSet();

        var slotIdsWithAppointments = await _appointmentRepository
            .Query()
            .Where(appointment => futureSlotIds.Contains(appointment.AppointmentSlotId))
            .Select(appointment => appointment.AppointmentSlotId)
            .Distinct()
            .ToListAsync(cancellationToken);

        var slotIdsWithConsultationSessions = await _consultationSessionRepository
            .Query()
            .Where(session => session.AppointmentSlotId.HasValue && futureSlotIds.Contains(session.AppointmentSlotId.Value))
            .Select(session => session.AppointmentSlotId!.Value)
            .Distinct()
            .ToListAsync(cancellationToken);

        var protectedSlotIds = slotIdsWithAppointments
            .Concat(slotIdsWithConsultationSessions)
            .ToHashSet();

        var deletableSlots = futureSlots
            .Where(slot => !protectedSlotIds.Contains(slot.Id))
            .ToList();

        foreach (var slot in deletableSlots)
        {
            await _appointmentSlotRepository.DeleteAsync(slot, cancellationToken);
        }

        if (deletableSlots.Count > 0)
        {
            await _unitOfWork.SaveChangesAsync(cancellationToken);
        }

        var result = new DeleteFutureOphthalmologistSlotsResultDto
        {
            OphthalmologistId = request.OphthalmologistId,
            Today = today,
            CurrentTimeUtc = currentTimeUtc,
            MatchedFutureSlots = futureSlots.Count,
            DeletedSlots = deletableSlots.Count,
            ProtectedSlots = protectedSlotIds.Count
        };

        _logger.LogInformation(
            "Deleted slots from today onward for ophthalmologist {OphthalmologistId}. Matched={Matched}, Deleted={Deleted}, Protected={Protected}, Today={Today}, CurrentTimeUtc={CurrentTimeUtc}",
            request.OphthalmologistId,
            result.MatchedFutureSlots,
            result.DeletedSlots,
            result.ProtectedSlots,
            result.Today,
            result.CurrentTimeUtc);

        return Result<DeleteFutureOphthalmologistSlotsResultDto>.Success(result);
    }

    private static DateTime GetVietnamNow()
    {
        var utcNow = DateTime.UtcNow;
        foreach (var timeZoneId in new[] { "SE Asia Standard Time", "Asia/Ho_Chi_Minh" })
        {
            try
            {
                var timeZone = TimeZoneInfo.FindSystemTimeZoneById(timeZoneId);
                return TimeZoneInfo.ConvertTimeFromUtc(utcNow, timeZone);
            }
            catch (TimeZoneNotFoundException)
            {
            }
            catch (InvalidTimeZoneException)
            {
            }
        }

        // UTC+7 fallback in case timezone metadata is unavailable.
        return utcNow + TimeSpan.FromHours(7);
    }
}
