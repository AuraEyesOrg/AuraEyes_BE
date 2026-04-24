using Application.Common.Interfaces;
using Application.Scheduling.Appointments.Common;
using Domain.Enums;

namespace Application.Scheduling.Appointments.Commands.CreateClinicAppointment;

public record CreateClinicAppointmentCommand : ICommand<CreateClinicAppointmentResult>
{
    public Guid SlotId { get; init; }
    public Guid? PatientId { get; init; }
    public string? VisitReason { get; init; }
    public PricingType PricingType { get; init; } = PricingType.AutoAssign;
    public Guid? RequestedDoctorId { get; init; }
}
