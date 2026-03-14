using Application.Common.Interfaces;

namespace Application.Scheduling.AppointmentSlots.Commands.GenerateSlots;

/// <summary>
/// Command to generate appointment slots from a schedule template for a date range.
/// Automatically creates slots based on the template's day-of-week, times, and slot duration.
/// </summary>
public record GenerateSlotsCommand : ICommand<int>
{
    /// <summary>The schedule template to generate slots from.</summary>
    public Guid ScheduleTemplateId { get; init; }

    /// <summary>Start date of the generation range (inclusive).</summary>
    public DateOnly FromDate { get; init; }

    /// <summary>End date of the generation range (inclusive).</summary>
    public DateOnly ToDate { get; init; }

    /// <summary>Whether to skip dates that already have slots (default: true).</summary>
    public bool SkipExistingDates { get; init; } = true;
}
