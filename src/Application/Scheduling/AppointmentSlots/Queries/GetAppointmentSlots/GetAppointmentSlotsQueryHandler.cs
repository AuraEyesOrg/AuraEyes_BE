using Application.Common.Interfaces;
using Application.Common.Models;
using Application.Scheduling.AppointmentSlots.Common;
using Domain.Repositories;

namespace Application.Scheduling.AppointmentSlots.Queries.GetAppointmentSlots;

public class GetAppointmentSlotsQueryHandler : IQueryHandler<GetAppointmentSlotsQuery, PagedResult<AppointmentSlotListDto>>
{
    private readonly IAppointmentSlotRepository _repository;
    private readonly IOphthalmologistRepository _ophthalmologistRepository;

    public GetAppointmentSlotsQueryHandler(
        IAppointmentSlotRepository repository,
        IOphthalmologistRepository ophthalmologistRepository)
    {
        _repository = repository;
        _ophthalmologistRepository = ophthalmologistRepository;
    }

    public async Task<Result<PagedResult<AppointmentSlotListDto>>> Handle(
        GetAppointmentSlotsQuery request,
        CancellationToken cancellationToken)
    {
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

        return Result<PagedResult<AppointmentSlotListDto>>.Success(resultPage);
    }
}
