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
    private readonly IMemoryCache _cache;

    public GetAppointmentSlotsQueryHandler(
        IAppointmentSlotRepository repository,
        IOphthalmologistRepository ophthalmologistRepository,
        IMemoryCache cache)
    {
        _repository = repository;
        _ophthalmologistRepository = ophthalmologistRepository;
        _cache = cache;
    }

    private static readonly System.Collections.Concurrent.ConcurrentDictionary<string, System.Threading.SemaphoreSlim> _semaphores = new();

    public async Task<Result<PagedResult<AppointmentSlotListDto>>> Handle(
        GetAppointmentSlotsQuery request,
        CancellationToken cancellationToken)
    {
        // Cache Key based on query parameters
        string cacheKey = $"slots_{request.ScheduleTemplateId}_{request.Status}_{request.FromDate}_{request.ToDate}_{request.ExcludePastSlots}_{request.PageNumber}_{request.PageSize}";
        
        // 1. Fast path
        if (_cache.TryGetValue(cacheKey, out PagedResult<AppointmentSlotListDto>? cachedResult))
        {
            return Result<PagedResult<AppointmentSlotListDto>>.Success(cachedResult!);
        }

        // 2. Lock to prevent Cache Stampede
        var semaphore = _semaphores.GetOrAdd(cacheKey, _ => new System.Threading.SemaphoreSlim(1, 1));
        await semaphore.WaitAsync(cancellationToken);

        try
        {
            // Re-check cache
            if (_cache.TryGetValue(cacheKey, out cachedResult))
            {
                return Result<PagedResult<AppointmentSlotListDto>>.Success(cachedResult!);
            }

            var (items, totalCount) = await _repository.GetPagedAsync(
                request.ScheduleTemplateId,
                request.Status,
                request.FromDate,
                request.ToDate,
                request.ExcludePastSlots,
                request.PageNumber,
                request.PageSize,
                cancellationToken);

            // Fetch ophthalmologist metadata for display names
            var ophthalIds = items.Where(i => i.OphthalId.HasValue).Select(i => i.OphthalId!.Value).Distinct().ToList();
            var ophthalMap = await _ophthalmologistRepository.GetDoctorDetailsByIdsAsync(ophthalIds, cancellationToken);

            var dtoList = items.Select(slot =>
            {
                var availableCapacity = slot.MaxCapacity - slot.BookedCount;
                ophthalMap.TryGetValue(slot.OphthalId ?? Guid.Empty, out var ophthalMeta);

                return new AppointmentSlotListDto
                {
                    Id = slot.Id,
                    OphthalId = slot.OphthalId ?? Guid.Empty,
                    OphthalFullName = ophthalMeta.FullName ?? "Clinic Slot",
                    OphthalAvatarUrl = ophthalMeta.AvatarUrl,
                    ScheduleTemplateId = slot.ScheduleTemplateId,
                    Date = slot.Date,
                    StartTime = slot.StartTime,
                    EndTime = slot.EndTime,
                    Status = slot.Status.ToString(),
                    MaxCapacity = slot.MaxCapacity,
                    BookedCount = slot.BookedCount,
                    AvailableCapacity = availableCapacity,
                    Cost = slot.Cost,
                    CreatedAt = slot.CreatedAt
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
            semaphore.Release();
        }
    }
}
