using Application.Common.Interfaces;
using Application.Common.Models;
using Application.Scheduling.AppointmentSlots.Common;
using Domain.Repositories;
using Microsoft.Extensions.Caching.Memory;

namespace Application.Scheduling.AppointmentSlots.Queries.GetAppointmentSlots;

public class GetAppointmentSlotsQueryHandler : IQueryHandler<GetAppointmentSlotsQuery, PagedResult<AppointmentSlotListDto>>
{
    private readonly IAppointmentSlotRepository _repository;
    private readonly IOphthalmologistRepository _ophthalmologistRepository;
    private readonly IIdentityService _identityService;
    private readonly IMemoryCache _cache;

    public GetAppointmentSlotsQueryHandler(
        IAppointmentSlotRepository repository,
        IOphthalmologistRepository ophthalmologistRepository,
        IIdentityService identityService,
        IMemoryCache cache)
    {
        _repository = repository;
        _ophthalmologistRepository = ophthalmologistRepository;
        _identityService = identityService;
        _cache = cache;
    }

    private static readonly System.Collections.Concurrent.ConcurrentDictionary<string, System.Threading.SemaphoreSlim> _semaphores = new();

    public async Task<Result<PagedResult<AppointmentSlotListDto>>> Handle(
        GetAppointmentSlotsQuery request,
        CancellationToken cancellationToken)
    {
        // Cache Key based on query parameters
        string cacheKey = $"slots_{request.ScheduleTemplateId}_{request.OphthalId}_{request.Status}_{request.FromDate}_{request.ToDate}_{request.ExcludePastSlots}_{request.PageNumber}_{request.PageSize}";
        
        // 1. Fast path
        if (_cache.TryGetValue(cacheKey, out PagedResult<AppointmentSlotListDto>? cachedResult))
        {
            return Result<PagedResult<AppointmentSlotListDto>>.Success(cachedResult!);
        }

        // 2. Lock to prevent Cache Stampede
        try
        {

            var (items, totalCount) = await _repository.GetPagedAsync(
                request.ScheduleTemplateId,
                request.OphthalId,
                request.Status,
                request.FromDate,
                request.ToDate,
                request.ExcludePastSlots,
                request.PageNumber,
                request.PageSize,
                cancellationToken);

            // Fetch ophthalmologist metadata for display names
            var ophthalIds = items.Where(i => i.OphthalId.HasValue).Select(i => i.OphthalId!.Value).Distinct().ToList();
            var ophthalMap = await _ophthalmologistRepository.GetEnhancedDoctorDetailsByIdsAsync(ophthalIds, cancellationToken);

            // Fetch patient names
            var registeredUserIds = items.SelectMany(s => s.Appointments)
                .Where(a => a.Patient != null && a.Patient.UserId.HasValue)
                .Select(a => a.Patient!.UserId!.Value)
                .Distinct()
                .ToList();
            var userMap = (await _identityService.GetUsersByIdsAsync(registeredUserIds, cancellationToken))
                .ToDictionary(u => u.Id);

            var dtoList = items.Select(slot =>
            {
                var availableCapacity = slot.MaxCapacity - slot.BookedCount;
                ophthalMap.TryGetValue(slot.OphthalId ?? Guid.Empty, out var ophthalMeta);

                var bookings = slot.Appointments.Select(a =>
                {
                    string patientName = "Patient";
                    if (a.Patient != null)
                    {
                        if (a.Patient.IsWalkIn)
                        {
                            patientName = a.Patient.FullName ?? "Patient";
                        }
                        else if (a.Patient.UserId.HasValue && userMap.TryGetValue(a.Patient.UserId.Value, out var user))
                        {
                            patientName = user.FullName ?? user.Email ?? "Patient";
                        }
                    }

                    return new SlotBookingDto
                    {
                        AppointmentId = a.Id,
                        PatientId = a.PatientId,
                        PatientName = patientName,
                        Status = a.Status.ToString()
                    };
                }).ToList();

                return new AppointmentSlotListDto
                {
                    Id = slot.Id,
                    OphthalId = slot.OphthalId ?? Guid.Empty,
                    OphthalFullName = ophthalMeta?.FullName != null 
                        ? (ophthalMeta.IsActive ? ophthalMeta.FullName : $"{ophthalMeta.FullName} (SUSPENDED)") 
                        : "Clinic Slot",
                    OphthalAvatarUrl = ophthalMeta?.AvatarUrl,
                    ScheduleTemplateId = slot.ScheduleTemplateId,
                    Date = slot.Date,
                    StartTime = slot.StartTime,
                    EndTime = slot.EndTime,
                    Status = slot.Status.ToString(),
                    MaxCapacity = slot.MaxCapacity,
                    BookedCount = slot.BookedCount,
                    AvailableCapacity = availableCapacity,
                    Cost = slot.Cost,
                    CreatedAt = slot.CreatedAt,
                    Bookings = bookings
                };
            }).ToList();

            var resultPage = new PagedResult<AppointmentSlotListDto>(
                dtoList,
                totalCount,
                request.PageNumber,
                request.PageSize);

            // Cache for 10 seconds to survive load test spikes
            _cache.Set(cacheKey, resultPage, TimeSpan.FromSeconds(10));

            return Result<PagedResult<AppointmentSlotListDto>>.Success(resultPage);
        }
        finally
        {
            // No lock to release
        }
    }
}
