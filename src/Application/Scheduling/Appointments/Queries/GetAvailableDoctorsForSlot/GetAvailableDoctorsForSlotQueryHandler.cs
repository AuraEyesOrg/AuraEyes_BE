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
    private readonly IOphthalmologistLeaveRequestRepository _leaveRequestRepository;

    public GetAvailableDoctorsForSlotQueryHandler(
        IOphthalmologistRepository ophthalmologistRepository,
        IAppointmentSlotRepository slotRepository,
        IOphthalmologistLeaveRequestRepository leaveRequestRepository)
    {
        _ophthalmologistRepository = ophthalmologistRepository;
        _slotRepository = slotRepository;
        _leaveRequestRepository = leaveRequestRepository;
    }

    public async Task<Result<IReadOnlyList<AvailableDoctorDto>>> Handle(
        GetAvailableDoctorsForSlotQuery request,
        CancellationToken cancellationToken)
    {
        // 1. Get all slots for the day
        var slots = await _slotRepository.GetByDateRangeAsync(
            request.Date, request.Date, cancellationToken);

        // 2. Determine which doctors have scheduled *available* slots (not Blocked/Expired)
        var scheduledDoctorIds = slots
            .Where(s => s.OphthalId.HasValue &&
                        s.Status == ScheduleStatus.Available &&
                        TimeRangesOverlap(s.StartTime, s.EndTime, request.StartTime, request.EndTime))
            .Select(s => s.OphthalId!.Value)
            .ToHashSet();

        // 3. Determine which doctors are fully booked in this time range
        var busyDoctorIds = slots
            .Where(s => s.OphthalId.HasValue &&
                        s.Status == ScheduleStatus.Available &&
                        s.BookedCount >= s.MaxCapacity &&
                        TimeRangesOverlap(s.StartTime, s.EndTime, request.StartTime, request.EndTime))
            .Select(s => s.OphthalId!.Value)
            .ToHashSet();

        // 4. Available = scheduled - fully-booked
        var availableDoctorIds = scheduledDoctorIds
            .Except(busyDoctorIds)
            .ToList();

        if (availableDoctorIds.Count == 0)
            return Result<IReadOnlyList<AvailableDoctorDto>>.Success(new List<AvailableDoctorDto>());

        // 5. Exclude doctors on approved leave for this date
        var doctorsOnLeave = new HashSet<Guid>();
        foreach (var doctorId in availableDoctorIds)
        {
            var leaves = await _leaveRequestRepository.GetApprovedOverlappingAsync(
                doctorId, request.Date, request.Date, cancellationToken);
            if (leaves.Any())
                doctorsOnLeave.Add(doctorId);
        }

        availableDoctorIds = availableDoctorIds.Except(doctorsOnLeave).ToList();

        if (availableDoctorIds.Count == 0)
            return Result<IReadOnlyList<AvailableDoctorDto>>.Success(new List<AvailableDoctorDto>());

        // 6. Fetch doctor details
        var doctorDetails = await _ophthalmologistRepository.GetEnhancedDoctorDetailsByIdsAsync(
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
