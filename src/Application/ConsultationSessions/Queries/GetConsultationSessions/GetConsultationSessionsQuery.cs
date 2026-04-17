using Application.Common.Interfaces;
using Application.Common.Models;
using Application.ConsultationSessions.Common;
using Domain.Enums;

namespace Application.ConsultationSessions.Queries.GetConsultationSessions;

public record GetConsultationSessionsQuery : IQuery<PagedResult<ConsultationSessionListDto>>
{
    public Guid? PatientId { get; init; }
    public Guid? OphthalmologistId { get; init; }
    public Guid? AiScreeningId { get; init; }
    public ConsultationSessionType? Type { get; init; }
    public SessionStatus? Status { get; init; }
    public ChatStatus? ChatStatus { get; init; }
    public int PageNumber { get; init; } = 1;
    public int PageSize { get; init; } = 10;
}
