using Application.Common.Interfaces;

namespace Application.Scheduling.Appointments.Commands.CancelLateAndGrantDiscount;

public record CancelLateAndGrantDiscountCommand(Guid AppointmentId) : ICommand<CancelLateAndGrantDiscountResult>;

public record CancelLateAndGrantDiscountResult
{
    public Guid CancelledAppointmentId { get; init; }
    public decimal DiscountRate { get; init; }
    public DateTime DiscountExpiryDate { get; init; }
}
