using Application.Common.Models;
using MediatR;

namespace Application.SystemAdmin.Ophthalmologists.Commands.BackfillFullTimeSchedule;

/// <summary>
/// Backfills system-generated full-time templates and missing slots for one ophthalmologist.
/// </summary>
public record BackfillFullTimeScheduleCommand : IRequest<Result<BackfillFullTimeScheduleResultDto>>
{
    public Guid OphthalmologistId { get; init; }
    public int? WindowDays { get; init; }
}

public record BackfillFullTimeScheduleResultDto
{
    public Guid OphthalmologistId { get; init; }
    public DateOnly FromDate { get; init; }
    public DateOnly ToDate { get; init; }
    public int WindowDays { get; init; }
    public int TemplatesEnsured { get; init; }
    public int ActiveSystemTemplates { get; init; }
    public int SlotsCreated { get; init; }
    public int SkippedInvalidTemplates { get; init; }
};
