using Application.Common.Interfaces;

namespace Application.Scheduling.AppointmentSlots.Commands.UnblockSlot;

/// <summary>
/// Command for a doctor to unblock an appointment slot.
/// Makes the slot available for patients again.
/// </summary>
public record UnblockSlotCommand : ICommand
{
    /// <summary>The appointment slot to unblock.</summary>
    public Guid AppointmentSlotId { get; init; }

    /// <summary>The ophthalmologist unblocking the slot (for authorization).</summary>
    public Guid OphthalmologistId { get; init; }
}
