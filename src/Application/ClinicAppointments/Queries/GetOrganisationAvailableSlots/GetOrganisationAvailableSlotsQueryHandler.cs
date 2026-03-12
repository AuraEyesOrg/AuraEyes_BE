using Application.ClinicAppointments.Common;
using Application.Common.Interfaces;
using Application.Common.Models;
using Domain.Repositories;

namespace Application.ClinicAppointments.Queries.GetOrganisationAvailableSlots;

/// <summary>
/// Handler for GetOrganisationAvailableSlotsQuery.
/// Returns available slots with remaining capacity for patient booking.
/// </summary>
public class GetOrganisationAvailableSlotsQueryHandler : IQueryHandler<GetOrganisationAvailableSlotsQuery, IReadOnlyList<OrganisationAvailableSlotDto>>
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
        // Default to today and 30 days ahead if not specified
        var fromDate = request.FromDate ?? DateOnly.FromDateTime(DateTime.UtcNow);
        var toDate = request.ToDate ?? fromDate.AddDays(30);

        var slots = await _appointmentSlotRepository.GetAvailableByOrganisationWithCapacityAsync(
            request.OrganisationId,
            fromDate,
            toDate,
            cancellationToken);

        var result = slots.Select(s => new OrganisationAvailableSlotDto
        {
            SlotId = s.Id,
            Date = s.Date,
            StartTime = s.StartTime,
            EndTime = s.EndTime,
            MaxCapacity = s.MaxCapacity,
            BookedCount = s.BookedCount,
            RemainingCapacity = s.RemainingCapacity,
            Cost = s.Cost
        }).ToList();

        return Result<IReadOnlyList<OrganisationAvailableSlotDto>>.Success(result);
    }
}
