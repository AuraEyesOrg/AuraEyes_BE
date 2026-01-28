using Application.Common.Interfaces;
using Application.Ophthalmologists.Common;

namespace Application.Ophthalmologists.Queries.GetOphthalmologist;

/// <summary>
/// Query to get a single ophthalmologist by ID.
/// </summary>
public record GetOphthalmologistQuery(Guid Id) : IQuery<OphthalmologistDto>;
