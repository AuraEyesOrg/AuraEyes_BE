using Application.Common.Interfaces;

namespace Application.Ophthalmologists.Queries.GetOphthalmologist;

public record GetOphthalmologistQuery(Guid Id) : IQuery<OphthalmologistDto>;
