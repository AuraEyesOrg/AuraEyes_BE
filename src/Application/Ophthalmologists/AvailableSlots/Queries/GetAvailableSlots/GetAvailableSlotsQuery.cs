using Application.Common.Interfaces;
using Application.Common.Models;
using Application.Ophthalmologists.AvailableSlots.Common;

namespace Application.Ophthalmologists.AvailableSlots.Queries.GetAvailableSlots;

/// <summary>
/// Query to get paginated list of available slots.
/// </summary>
public record GetAvailableSlotsQuery : IQuery<PagedResult<AvailableSlotListDto>>
{
    /// <summary>Filter by ophthalmologist ID (optional).</summary>
    public Guid? OphthalmologistId { get; init; }

    /// <summary>Filter by organisation ID (optional).</summary>
    public Guid? OrganisationId { get; init; }

    /// <summary>Filter slots from this date (optional).</summary>
    public DateTime? FromDate { get; init; }

    /// <summary>Filter slots up to this date (optional).</summary>
    public DateTime? ToDate { get; init; }

    /// <summary>Page number (default: 1).</summary>
    public int PageNumber { get; init; } = 1;

    /// <summary>Page size (default: 10).</summary>
    public int PageSize { get; init; } = 10;
}
