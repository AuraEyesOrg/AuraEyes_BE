using Application.Common.Interfaces;

namespace Application.Scheduling.AppointmentSlots.Commands.BlockSlot;

/// <summary>
/// Command for a doctor to block an appointment slot.
/// Blocked slots are not visible to patients.
/// </summary>
public record BlockSlotCommand : ICommand
{
    /// <summary>The appointment slot to block.</summary>
    public Guid AppointmentSlotId { get; init; }

    /// <summary>The ophthalmologist blocking the slot (for authorization).</summary>
    public Guid OphthalmologistId { get; init; }

    /// <summary>Optional reason for blocking.</summary>
    public string? Reason { get; init; }
}
