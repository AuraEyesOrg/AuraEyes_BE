using Application.Common.Interfaces;
using Application.Common.Models;
using Application.Scheduling.AppointmentSlots.Common;
using Domain.Enums;

namespace Application.Scheduling.AppointmentSlots.Queries.GetAppointmentSlots;

public record GetAppointmentSlotsQuery : IQuery<PagedResult<AppointmentSlotListDto>>
{
    public Guid? ScheduleTemplateId { get; init; }
    public Guid? OphthalId { get; init; }
    public Guid? OrgId { get; init; }
    public ScheduleStatus? Status { get; init; }
    public DateOnly? FromDate { get; init; }
    public DateOnly? ToDate { get; init; }
    public int PageNumber { get; init; } = 1;
    public int PageSize { get; init; } = 10;
}
