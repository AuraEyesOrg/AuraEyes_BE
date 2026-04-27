using Application.Common.Interfaces;
using Application.Common.Models;
using Application.Scheduling.AppointmentSlots.Common;
using Domain.Repositories;

namespace Application.Scheduling.AppointmentSlots.Queries.GetAppointmentSlot;

public class GetAppointmentSlotQueryHandler : IQueryHandler<GetAppointmentSlotQuery, AppointmentSlotDto>
{
    private readonly IAppointmentSlotRepository _repository;

    public GetAppointmentSlotQueryHandler(IAppointmentSlotRepository repository)
    {
        _repository = repository;
    }

    public async Task<Result<AppointmentSlotDto>> Handle(GetAppointmentSlotQuery request, CancellationToken cancellationToken)
    {
        var slot = await _repository.GetByIdWithTemplateAsync(request.SlotId, cancellationToken);
        if (slot is null)
            return Result<AppointmentSlotDto>.NotFound($"Appointment slot with ID '{request.SlotId}' was not found.");

        var availableCapacity = slot.MaxCapacity - slot.BookedCount;

        var dto = new AppointmentSlotDto
        {
            Id = slot.Id,
            OphthalId = slot.OphthalId ?? Guid.Empty,
            ScheduleTemplateId = slot.ScheduleTemplateId,
            Date = slot.Date,
            StartTime = slot.StartTime,
            EndTime = slot.EndTime,
            Status = slot.Status.ToString(),
            MaxCapacity = slot.MaxCapacity,
            BookedCount = slot.BookedCount,
            AvailableCapacity = availableCapacity,
            Cost = slot.Cost,
            ReservationExpireAt = slot.ReservationExpireAt,
            CreatedAt = slot.CreatedAt,
            UpdatedAt = slot.UpdatedAt
        };

        return Result<AppointmentSlotDto>.Success(dto);
    }
}
