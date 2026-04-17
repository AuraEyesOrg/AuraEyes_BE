using Application.Common.Models;
using MediatR;

namespace Application.SystemAdmin.Ophthalmologists.Commands.NormalizeAllFullTimeSchedules;

/// <summary>
/// Normalizes schedules for all full-time ophthalmologists in one operation.
/// </summary>
public record NormalizeAllFullTimeSchedulesCommand : IRequest<Result<NormalizeAllFullTimeSchedulesResultDto>>
{
    /// <summary>
    /// Optional reconciliation window in days. Defaults to 7 days.
    /// </summary>
    public int? WindowDays { get; init; }
}

public record NormalizeAllFullTimeSchedulesResultDto
{
    public DateOnly FromDate { get; init; }
    public DateOnly ToDate { get; init; }
    public int WindowDays { get; init; }

    public int TotalFullTimeOphthalmologists { get; init; }
    public int Processed { get; init; }
    public int Succeeded { get; init; }
    public int Failed { get; init; }

    public int TotalTemplatesEnsured { get; init; }
    public int TotalTemplatesUpdated { get; init; }
    public int TotalTemplatesDeactivated { get; init; }
    public int TotalSlotsDeleted { get; init; }
    public int TotalSlotsCreated { get; init; }
    public int TotalSlotsProtected { get; init; }

    public IReadOnlyList<NormalizeAllFullTimeSchedulesFailureDto> Failures { get; init; } = Array.Empty<NormalizeAllFullTimeSchedulesFailureDto>();
}

public record NormalizeAllFullTimeSchedulesFailureDto
{
    public Guid OphthalmologistId { get; init; }
    public string Error { get; init; } = string.Empty;
}
