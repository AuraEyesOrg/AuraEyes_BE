using Application.Common.Interfaces;
using Application.Scheduling.AppointmentSlots.Common;

namespace Application.Scheduling.AppointmentSlots.Queries.GetAllowedPriceRange;

public record GetAllowedPriceRangeQuery(Guid OphthalmologistId) : IQuery<AllowedPriceRangeDto>;