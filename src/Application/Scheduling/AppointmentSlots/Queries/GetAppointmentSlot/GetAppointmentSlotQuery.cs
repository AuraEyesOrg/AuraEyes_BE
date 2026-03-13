using Application.Common.Interfaces;
using Application.Scheduling.AppointmentSlots.Common;

namespace Application.Scheduling.AppointmentSlots.Queries.GetAppointmentSlot;

public record GetAppointmentSlotQuery(Guid SlotId) : IQuery<AppointmentSlotDto>;
