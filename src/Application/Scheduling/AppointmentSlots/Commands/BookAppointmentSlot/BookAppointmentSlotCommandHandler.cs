using Application.Common.Interfaces;
using Application.Common.Models;
using Domain.Common;
using Domain.Enums;
using Domain.Repositories;
using Microsoft.Extensions.Logging;

namespace Application.Scheduling.AppointmentSlots.Commands.BookAppointmentSlot;

/// <summary>
/// Handler for BookAppointmentSlotCommand.
/// Loads the appointment slot with its ScheduleTemplate and validates capacity before booking.
/// </summary>
public class BookAppointmentSlotCommandHandler : ICommandHandler<BookAppointmentSlotCommand>
{
    private readonly IAppointmentSlotRepository _appointmentSlotRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<BookAppointmentSlotCommandHandler> _logger;

    public BookAppointmentSlotCommandHandler(
        IAppointmentSlotRepository appointmentSlotRepository,
        IUnitOfWork unitOfWork,
        ILogger<BookAppointmentSlotCommandHandler> logger)
    {
        _appointmentSlotRepository = appointmentSlotRepository;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<Result> Handle(BookAppointmentSlotCommand request, CancellationToken cancellationToken)
    {
        // Load appointment slot with its ScheduleTemplate to perform capacity check
        var slot = await _appointmentSlotRepository.GetByIdWithTemplateAsync(
            request.AppointmentSlotId, cancellationToken);

        if (slot is null)
        {
            return Result.NotFound($"Appointment slot '{request.AppointmentSlotId}' not found.");
        }

        if (slot.Status != ScheduleStatus.Available)
        {
            return Result.Failure($"Appointment slot is not available for booking. Current status: {slot.Status}.");
        }

        if (slot.ScheduleTemplate is null)
        {
            return Result.NotFound($"Schedule template for appointment slot '{request.AppointmentSlotId}' not found.");
        }

        // Check if there's capacity remaining
        if (slot.BookedCount >= slot.ScheduleTemplate.MaxCapacity)
        {
            return Result.Conflict("This appointment slot is fully booked. No capacity remaining.");
        }

        try
        {
            slot.Book();

            // Update status to Booked if this is the first booking
            // or if we've reached capacity
            if (slot.BookedCount >= slot.ScheduleTemplate.MaxCapacity)
            {
                slot.UpdateStatus(ScheduleStatus.Booked);
            }
        }
        catch (InvalidOperationException ex)
        {
            return Result.Failure(ex.Message);
        }

        await _appointmentSlotRepository.UpdateAsync(slot, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        _logger.LogInformation(
            "Appointment slot {SlotId} booked by Patient {PatientId}. BookedCount: {BookedCount}/{MaxCapacity}",
            request.AppointmentSlotId, request.PatientId, slot.BookedCount, slot.ScheduleTemplate.MaxCapacity);

        return Result.Success();
    }
}
