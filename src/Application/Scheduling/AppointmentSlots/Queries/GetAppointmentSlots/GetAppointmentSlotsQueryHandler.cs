using Application.Common.Constants;
using Application.Common.Interfaces;
using Application.Common.Models;
using Application.Scheduling.AppointmentSlots.Common;
using Domain.Enums;
using Domain.Repositories;

namespace Application.Scheduling.AppointmentSlots.Queries.GetAppointmentSlots;

public class GetAppointmentSlotsQueryHandler : IQueryHandler<GetAppointmentSlotsQuery, PagedResult<AppointmentSlotListDto>>
{
    private readonly IAppointmentSlotRepository _repository;
    private readonly ICurrentUserService _currentUser;

    public GetAppointmentSlotsQueryHandler(
        IAppointmentSlotRepository repository,
        ICurrentUserService currentUser)
    {
        _repository = repository;
        _currentUser = currentUser;
    }

    public async Task<Result<PagedResult<AppointmentSlotListDto>>> Handle(
        GetAppointmentSlotsQuery request,
        CancellationToken cancellationToken)
    {
        Guid? effectiveOphthalId = request.OphthalId;

        if (_currentUser.IsInRole(Roles.Ophthalmologist))
        {
            if (!_currentUser.ProfileId.HasValue)
            {
                return Result<PagedResult<AppointmentSlotListDto>>.Forbidden(
                    "Unable to resolve ophthalmologist profile from current token.");
            }

            if (request.OphthalId.HasValue && request.OphthalId.Value != _currentUser.ProfileId.Value)
            {
                return Result<PagedResult<AppointmentSlotListDto>>.Forbidden(
                    "You are not authorized to view slots of other ophthalmologists.");
            }

            effectiveOphthalId = _currentUser.ProfileId.Value;
        }

        var (items, totalCount) = await _repository.GetPagedAsync(
            request.ScheduleTemplateId,
            effectiveOphthalId,
            request.OrgId,
            request.Status,
            request.FromDate,
            request.ToDate,
            request.PageNumber,
            request.PageSize,
            cancellationToken);

        // Map to DTOs using already-loaded ScheduleTemplate
        var dtoList = items.Select(slot =>
        {
            var template = slot.ScheduleTemplate;
            var availableCapacity = template != null
                ? template.MaxCapacity - slot.BookedCount
                : 0;

            return new AppointmentSlotListDto
            {
                Id = slot.Id,
                ScheduleTemplateId = slot.ScheduleTemplateId,
                OphthalId = template?.OphthalId,
                OrgId = template?.OrgId,
                Date = slot.Date,
                StartTime = slot.StartTime,
                EndTime = slot.EndTime,
                Status = slot.Status.ToString(),
                Cost = slot.Cost,
                MaxCapacity = slot.MaxCapacity,
                BookedCount = slot.BookedCount,
                AvailableCapacity = availableCapacity,
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
