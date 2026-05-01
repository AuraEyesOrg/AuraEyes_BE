using Application.Common.Interfaces;
using Application.Common.Models;
using Domain.Enums;
using Domain.Repositories;

namespace Application.Scheduling.Appointments.Queries.GetAvailableDoctorsForSlot;

public class GetAvailableDoctorsForSlotQueryHandler
    : IQueryHandler<GetAvailableDoctorsForSlotQuery, IReadOnlyList<AvailableDoctorDto>>
{
    private readonly IOphthalmologistRepository _ophthalmologistRepository;
    private readonly IAppointmentSlotRepository _slotRepository;

    public GetAvailableDoctorsForSlotQueryHandler(
        IOphthalmologistRepository ophthalmologistRepository,
        IAppointmentSlotRepository slotRepository)
    {
        _ophthalmologistRepository = ophthalmologistRepository;
        _slotRepository = slotRepository;
    }

    public async Task<Result<IReadOnlyList<AvailableDoctorDto>>> Handle(
        GetAvailableDoctorsForSlotQuery request,
        CancellationToken cancellationToken)
    {
        // 1. Get all slots for the day
        var slots = await _slotRepository.GetByDateRangeAsync(
            request.Date, request.Date, cancellationToken);

        // 2. Determine which doctors have scheduled slots (template overlap)
        var scheduledDoctorIds = slots
            .Where(s => s.OphthalId.HasValue &&
                        TimeRangesOverlap(s.StartTime, s.EndTime, request.StartTime, request.EndTime))
            .Select(s => s.OphthalId!.Value)
            .ToHashSet();

        // 3. Determine which doctors are busy (have booked slots in this time range)
        var busyDoctorIds = slots
            .Where(s => s.OphthalId.HasValue &&
                        s.BookedCount > 0 &&
                        TimeRangesOverlap(s.StartTime, s.EndTime, request.StartTime, request.EndTime))
            .Select(s => s.OphthalId!.Value)
            .ToHashSet();

        // 4. Available = scheduled - busy
        var availableDoctorIds = scheduledDoctorIds
            .Except(busyDoctorIds)
            .ToList();

        if (availableDoctorIds.Count == 0)
            return Result<IReadOnlyList<AvailableDoctorDto>>.Success(new List<AvailableDoctorDto>());

        // 5. Fetch doctor details
        var allDoctors = await _ophthalmologistRepository.GetPagedAsync(
            pageNumber: 1,
            pageSize: 1000,
            cancellationToken: cancellationToken);

        // Fetch doctor details (name, avatar) from identity
        var doctorDetails = await _ophthalmologistRepository.GetDoctorDetailsByIdsAsync(
            availableDoctorIds, cancellationToken);

        var availableDoctors = availableDoctorIds
            .Select(id =>
            {
                doctorDetails.TryGetValue(id, out var detail);
                return new AvailableDoctorDto
                {
                    Id = id,
                    FullName = detail.FullName ?? "Doctor",
                    AvatarUrl = detail.AvatarUrl
                };
            })
            .OrderBy(d => d.FullName)
            .ToList();

        return Result<IReadOnlyList<AvailableDoctorDto>>.Success(availableDoctors);
    }

    private static bool TimeRangesOverlap(TimeOnly startA, TimeOnly endA, TimeOnly startB, TimeOnly endB)
    {
        return startA < endB && endA > startB;
    }
}
