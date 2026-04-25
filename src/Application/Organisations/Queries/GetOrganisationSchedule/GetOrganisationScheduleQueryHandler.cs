using Application.Common.Interfaces;
using Application.Common.Models;
using Application.Scheduling.AppointmentSlots.Common;
using AutoMapper;
using Domain.Common;
using Domain.Entities.Users;
using Domain.Repositories;
using MediatR;

namespace Application.Organisations.Queries.GetOrganisationSchedule;

public class GetOrganisationScheduleQueryHandler : IRequestHandler<GetOrganisationScheduleQuery, Result<OrganisationScheduleDto>>
{
    private readonly IAppointmentSlotRepository _slotRepository;
    private readonly IRepository<Organisation> _organisationRepository;
    private readonly IOphthalmologistRepository _ophthalmologistRepository;

    public GetOrganisationScheduleQueryHandler(
        IAppointmentSlotRepository slotRepository,
        IRepository<Organisation> organisationRepository,
        IOphthalmologistRepository ophthalmologistRepository)
    {
        _slotRepository = slotRepository;
        _organisationRepository = organisationRepository;
        _ophthalmologistRepository = ophthalmologistRepository;
    }

    public async Task<Result<OrganisationScheduleDto>> Handle(GetOrganisationScheduleQuery request, CancellationToken cancellationToken)
    {
        Organisation? organisation = null;
        if (request.OrganisationId != Guid.Empty)
        {
            organisation = await _organisationRepository.GetByIdAsync(request.OrganisationId, cancellationToken);
            if (organisation == null)
            {
                return Result<OrganisationScheduleDto>.NotFound("Organisation not found.");
            }
        }

        var fromDate = request.FromDate ?? DateOnly.FromDateTime(DateTime.UtcNow);
        var toDate = request.ToDate ?? fromDate.AddDays(7); // Default to 7 days for schedule view

        // 1. Fetch all slots for this organisation in the date range (including booked ones)
        var slots = await _slotRepository.GetByOrganisationAndDateRangeAsync(
            request.OrganisationId,
            fromDate,
            toDate,
            cancellationToken);

        // 2. Collect unique doctor IDs and fetch their details
        var doctorIds = slots.Where(s => s.OphthalId.HasValue).Select(s => s.OphthalId!.Value).Distinct().ToList();
        var doctorDetails = await _ophthalmologistRepository.GetDoctorDetailsByIdsAsync(doctorIds, cancellationToken);

        // 3. Group slots by (Date, StartTime, EndTime)
        var aggregatedSlots = slots
            .GroupBy(s => new { s.Date, s.StartTime, s.EndTime })
            .Select(g => new AggregatedSlotDto
            {
                Date = g.Key.Date,
                StartTime = g.Key.StartTime,
                EndTime = g.Key.EndTime,
                Doctors = g.Select(s => new DoctorSlotDetailDto
                {
                    SlotId = s.Id,
                    DoctorId = s.OphthalId ?? Guid.Empty,
                    DoctorName = s.OphthalId.HasValue && doctorDetails.TryGetValue(s.OphthalId.Value, out var details) ? details.FullName : "Aura Clinic",
                    DoctorAvatar = s.OphthalId.HasValue && doctorDetails.TryGetValue(s.OphthalId.Value, out var d) ? d.AvatarUrl : null,
                    IsBooked = s.BookedCount >= s.MaxCapacity,
                    Price = s.Cost ?? 0
                }).ToList()
            })
            .OrderBy(s => s.Date)
            .ThenBy(s => s.StartTime)
            .ToList();

        var dto = new OrganisationScheduleDto
        {
            Id = organisation?.Id ?? Guid.Empty,
            Name = organisation?.Name ?? "Aura Clinic",
            Address = organisation?.Address ?? "Hệ thống Aura Clinic",
            Description = organisation?.Description,
            RatingAverage = organisation?.RatingAverage ?? 5.0m,
            RatingCount = organisation?.RatingCount ?? 0,
            AggregatedSlots = aggregatedSlots
        };

        return Result<OrganisationScheduleDto>.Success(dto);
    }
}
