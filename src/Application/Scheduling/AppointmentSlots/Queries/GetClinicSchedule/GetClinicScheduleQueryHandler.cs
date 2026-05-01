using Application.Common.Interfaces;
using Application.Common.Models;
using Application.Scheduling.AppointmentSlots.Common;
using Domain.Repositories;
using Domain.Enums;
using MediatR;

namespace Application.Scheduling.AppointmentSlots.Queries.GetClinicSchedule;

public class GetClinicScheduleQueryHandler : IQueryHandler<GetClinicScheduleQuery, ClinicScheduleDto>
{
    private readonly IAppointmentSlotRepository _appointmentSlotRepository;
    private readonly IOphthalmologistRepository _ophthalmologistRepository;

    public GetClinicScheduleQueryHandler(
        IAppointmentSlotRepository appointmentSlotRepository,
        IOphthalmologistRepository ophthalmologistRepository)
    {
        _appointmentSlotRepository = appointmentSlotRepository;
        _ophthalmologistRepository = ophthalmologistRepository;
    }

    public async Task<Result<ClinicScheduleDto>> Handle(GetClinicScheduleQuery request, CancellationToken cancellationToken)
    {
        var fromDate = request.FromDate ?? DateOnly.FromDateTime(DateTime.Today);
        var toDate = request.ToDate ?? fromDate;

        var slots = await _appointmentSlotRepository.GetByDateRangeAsync(
            fromDate, 
            toDate, 
            cancellationToken);

        var ophthalIds = slots.Where(s => s.OphthalId.HasValue).Select(s => s.OphthalId!.Value).Distinct().ToList();
        var ophthalMap = await _ophthalmologistRepository.GetDoctorDetailsByIdsAsync(ophthalIds, cancellationToken);

        var aggregated = slots
            .Where(s => s.Status != ScheduleStatus.Expired)
            .GroupBy(s => new { s.Date, s.StartTime, s.EndTime })
            .Select(g => new AggregatedSlotDto
            {
                Date = g.Key.Date,
                StartTime = g.Key.StartTime,
                EndTime = g.Key.EndTime,
                TotalMaxCapacity = g.Sum(s => s.MaxCapacity),
                TotalBookedCount = g.Sum(s => s.BookedCount),
                IsAvailable = g.Any(s => s.Status == ScheduleStatus.Available && s.BookedCount < s.MaxCapacity),
                Doctors = g.Select(s =>
                {
                    ophthalMap.TryGetValue(s.OphthalId ?? Guid.Empty, out var ophthalMeta);
                    return new DoctorSlotDetailDto
                    {
                        SlotId = s.Id,
                        DoctorId = s.OphthalId ?? Guid.Empty,
                        DoctorName = ophthalMeta.FullName ?? "Aura Doctor",
                        DoctorAvatar = ophthalMeta.AvatarUrl,
                        IsBooked = s.BookedCount >= s.MaxCapacity,
                        Price = s.Cost ?? 0
                    };
                }).ToList()
            })
            .OrderBy(a => a.Date)
            .ThenBy(a => a.StartTime)
            .ToList();

        var dto = new ClinicScheduleDto
        {
            Id = Guid.Empty,
            Name = "Aura Eyes Clinic",
            Address = "123 Healthcare Ave, Digital City",
            Description = "Premium Retinal Care & AI Screening",
            RatingAverage = 4.8,
            RatingCount = 120,
            AggregatedSlots = aggregated
        };

        return Result<ClinicScheduleDto>.Success(dto);
    }
}
