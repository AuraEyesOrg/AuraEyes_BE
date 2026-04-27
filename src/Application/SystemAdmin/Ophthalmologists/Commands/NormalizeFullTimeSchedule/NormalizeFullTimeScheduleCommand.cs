using Application.Common.Interfaces;
using Application.Common.Models;

namespace Application.SystemAdmin.Ophthalmologists.Commands.NormalizeFullTimeSchedule;

/// <summary>
/// Normalizes full-time ophthalmologist schedules to canonical system-generated weekday templates and reconciles future slots.
/// </summary>
public record NormalizeFullTimeScheduleCommand : ICommand<NormalizeFullTimeScheduleResultDto>
{
    public Guid OphthalmologistId { get; init; }
    public int? WindowDays { get; init; }
}

public record NormalizeFullTimeScheduleResultDto
{
    public Guid OphthalmologistId { get; init; }
    public DateOnly FromDate { get; init; }
    public DateOnly ToDate { get; init; }
    public int WindowDays { get; init; }
    public int TemplatesEnsured { get; init; }
    public int CanonicalTemplates { get; init; }
    public int TemplatesUpdated { get; init; }
    public int TemplatesDeactivated { get; init; }
    public int SlotsDeleted { get; init; }
    public int SlotsCreated { get; init; }
    public int SlotsProtected { get; init; }
}
