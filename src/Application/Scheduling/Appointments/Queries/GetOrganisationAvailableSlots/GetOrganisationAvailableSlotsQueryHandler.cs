using Application.Common.Interfaces;
using Application.Common.Models;
using Application.Scheduling.Appointments.Common;
using Domain.Repositories;

namespace Application.Scheduling.Appointments.Queries.GetOrganisationAvailableSlots;

public class GetOrganisationAvailableSlotsQueryHandler
    : IQueryHandler<GetOrganisationAvailableSlotsQuery, IReadOnlyList<OrganisationAvailableSlotDto>>
{
    private readonly IAppointmentSlotRepository _appointmentSlotRepository;

    public GetOrganisationAvailableSlotsQueryHandler(IAppointmentSlotRepository appointmentSlotRepository)
    {
        _appointmentSlotRepository = appointmentSlotRepository;
    }

    public async Task<Result<IReadOnlyList<OrganisationAvailableSlotDto>>> Handle(
        GetOrganisationAvailableSlotsQuery request,
        CancellationToken cancellationToken)
    {
        var fromDate = request.Date ?? request.FromDate;
        var toDate = request.Date ?? request.ToDate;

        var slots = await _appointmentSlotRepository.GetAvailableByOrganisationWithCapacityAsync(
            request.OrganisationId,
            fromDate,
            toDate,
            cancellationToken);

        var data = slots
            .Select(slot => new OrganisationAvailableSlotDto
            {
                SlotId = slot.Id,
                Date = slot.Date,
                StartTime = slot.StartTime,
                EndTime = slot.EndTime,
                MaxCapacity = slot.MaxCapacity,
                BookedCount = slot.BookedCount,
                Remaining = slot.RemainingCapacity,
                Cost = slot.Cost
            })
            .ToList();

        return Result<IReadOnlyList<OrganisationAvailableSlotDto>>.Success(data);
    }
}
