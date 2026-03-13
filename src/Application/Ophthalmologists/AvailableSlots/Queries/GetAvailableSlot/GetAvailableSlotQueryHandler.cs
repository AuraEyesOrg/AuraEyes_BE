using Application.Common.Interfaces;
using Application.Common.Models;
using Application.Ophthalmologists.AvailableSlots.Common;
using Domain.Enums;
using Domain.Repositories;

namespace Application.Ophthalmologists.AvailableSlots.Queries.GetAvailableSlot;

/// <summary>
/// Handler for GetAvailableSlotQuery.
/// </summary>
public class GetAvailableSlotQueryHandler : IQueryHandler<GetAvailableSlotQuery, AvailableSlotDto>
{
    private readonly IAvailableSlotRepository _availableSlotRepository;

    public GetAvailableSlotQueryHandler(IAvailableSlotRepository availableSlotRepository)
    {
        _availableSlotRepository = availableSlotRepository;
    }

    public async Task<Result<AvailableSlotDto>> Handle(
        GetAvailableSlotQuery request,
        CancellationToken cancellationToken)
    {
        var availableSlot = await _availableSlotRepository.GetByIdWithSchedulesAsync(
            request.AvailableSlotId, cancellationToken);

        if (availableSlot is null)
        {
            return Result<AvailableSlotDto>.NotFound(
                $"Available slot with ID '{request.AvailableSlotId}' was not found.");
        }

        var bookedCount = availableSlot.Schedules.Count(s => s.Status != ScheduleStatus.Cancelled);

        var dto = new AvailableSlotDto
        {
            Id = availableSlot.Id,
            OrganisationId = availableSlot.OrganisationId,
            OphthalmologistId = availableSlot.OphthalmologistId,
            StartTime = availableSlot.StartTime,
            EndTime = availableSlot.EndTime,
            MaxCapacity = availableSlot.MaxCapacity,
            BookedCount = bookedCount,
            CreatedAt = availableSlot.CreatedAt,
            UpdatedAt = availableSlot.UpdatedAt,
            Schedules = availableSlot.Schedules.Select(s => new AvailableSlotScheduleDto
            {
                Id = s.Id,
                PatientId = s.PatientId,
                Date = s.Date,
                StartTime = s.StartTime,
                EndTime = s.EndTime,
                Status = s.Status.ToString()
            }).ToList()
        };

        return Result<AvailableSlotDto>.Success(dto);
    }
}
