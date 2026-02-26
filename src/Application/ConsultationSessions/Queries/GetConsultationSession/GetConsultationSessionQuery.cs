using Application.Common.Interfaces;
using Application.ConsultationSessions.Common;

namespace Application.ConsultationSessions.Queries.GetConsultationSession;

public record GetConsultationSessionQuery(Guid SessionId) : IQuery<ConsultationSessionDto>;
