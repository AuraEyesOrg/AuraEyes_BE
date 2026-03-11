using Application.Common.Interfaces;
using Application.Ophthalmologists.AvailableSlots.Common;

namespace Application.Ophthalmologists.AvailableSlots.Queries.GetAvailableSlot;

/// <summary>
/// Query to get a specific available slot by ID.
/// </summary>
public record GetAvailableSlotQuery(Guid AvailableSlotId) : IQuery<AvailableSlotDto>;
