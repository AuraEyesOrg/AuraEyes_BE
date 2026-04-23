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
    private readonly IMapper _mapper;

    public GetOrganisationScheduleQueryHandler(
        IAppointmentSlotRepository slotRepository,
        IMapper mapper)
    {
        _slotRepository = slotRepository;
        _mapper = mapper;
    }

    public async Task<Result<OrganisationScheduleDto>> Handle(GetOrganisationScheduleQuery request, CancellationToken cancellationToken)
    {
        var fromDate = request.FromDate ?? DateOnly.FromDateTime(DateTime.UtcNow);
        var toDate = request.ToDate ?? fromDate.AddDays(30);

        // Fetch available slots for the clinic (single clinic model)
        var slots = await _slotRepository.GetAvailableSlotsAsync(
            null, // No specific template ID filter
            fromDate,
            toDate,
            cancellationToken);

        var slotDtos = slots.Select(slot => new AppointmentSlotListDto
        {
            Id = slot.Id,
            ScheduleTemplateId = slot.ScheduleTemplateId,
            Date = slot.Date,
            StartTime = slot.StartTime,
            EndTime = slot.EndTime,
            Status = slot.Status.ToString(),
            MaxCapacity = slot.MaxCapacity,
            BookedCount = slot.BookedCount,
            AvailableCapacity = slot.MaxCapacity - slot.BookedCount,
            CreatedAt = slot.CreatedAt
        }).ToList();

        var dto = new OrganisationScheduleDto
        {
            Id = Guid.Empty, // No specific organisation ID in single-clinic model
            Name = "Aura Eyes Clinic",
            Address = "Medical Center",
            Description = "Premium Eye Care Facility",
            RatingAverage = 5.0m,
            RatingCount = 100,
            AvailableSlots = slotDtos
        };

        return Result<OrganisationScheduleDto>.Success(dto);
    }
}
