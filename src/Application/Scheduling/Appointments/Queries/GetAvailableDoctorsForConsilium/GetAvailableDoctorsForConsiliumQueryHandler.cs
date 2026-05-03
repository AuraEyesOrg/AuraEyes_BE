using Application.Common.Interfaces;
using Application.Common.Models;
using Application.Scheduling.Appointments.Common;
using Domain.Repositories;

namespace Application.Scheduling.Appointments.Queries.GetAvailableDoctorsForConsilium;

public class GetAvailableDoctorsForConsiliumQueryHandler : IQueryHandler<GetAvailableDoctorsForConsiliumQuery, List<AvailableDoctorDto>>
{
    private readonly IOphthalmologistRepository _ophthalmologistRepository;
    private readonly IDateTime _dateTime;

    public GetAvailableDoctorsForConsiliumQueryHandler(
        IOphthalmologistRepository ophthalmologistRepository,
        IDateTime dateTime)
    {
        _ophthalmologistRepository = ophthalmologistRepository;
        _dateTime = dateTime;
    }

    public async Task<Result<List<AvailableDoctorDto>>> Handle(GetAvailableDoctorsForConsiliumQuery request, CancellationToken cancellationToken)
    {
        // 1. Time Window Calculation: 20-minute block + 5-minute buffer = 25 minutes
        var windowStart = _dateTime.UtcNow;
        var windowEnd = windowStart.AddMinutes(25);

        // 2. Fetch available doctors using optimized repository query
        var availableDoctors = await _ophthalmologistRepository.GetAvailableDoctorsForConsiliumAsync(
            windowStart,
            windowEnd,
            cancellationToken);

        // 3. Project to Response DTO
        var result = availableDoctors.Select(d => new AvailableDoctorDto(
            d.Id,
            d.Name,
            d.Avatar,
            d.DegreeLevel
        )).ToList();

        return Result<List<AvailableDoctorDto>>.Success(result);
    }
}
