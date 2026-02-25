using Application.Common.Interfaces;
using Application.Common.Models;
using Application.ConsultationSessions.Common;
using Domain.Repositories;

namespace Application.ConsultationSessions.Queries.GetConsultationSessions;

public class GetConsultationSessionsQueryHandler
    : IQueryHandler<GetConsultationSessionsQuery, PagedResult<ConsultationSessionListDto>>
{
    private readonly IConsultationSessionRepository _sessionRepository;

    public GetConsultationSessionsQueryHandler(IConsultationSessionRepository sessionRepository)
    {
        _sessionRepository = sessionRepository;
    }

    public async Task<Result<PagedResult<ConsultationSessionListDto>>> Handle(
        GetConsultationSessionsQuery request,
        CancellationToken cancellationToken)
    {
        var (items, totalCount) = await _sessionRepository.GetPagedAsync(
            request.PatientId,
            request.OphthalmologistId,
            request.Type,
            request.Status,
            request.ChatStatus,
            request.PageNumber,
            request.PageSize,
            cancellationToken);

        var dtoList = items.Select(s => new ConsultationSessionListDto
        {
            Id = s.Id,
            PatientId = s.PatientId,
            OphthalmologistId = s.OphthalmologistId,
            Type = s.Type,
            Status = s.Status,
            ChatStatus = s.ChatStatus,
            Price = s.Price,
            AppointmentTime = s.AppointmentTime,
            LastActivityAt = s.LastActivityAt,
            CreatedAt = s.CreatedAt
        }).ToList();

        var pagedResult = new PagedResult<ConsultationSessionListDto>(
            dtoList, totalCount, request.PageNumber, request.PageSize);

        return Result<PagedResult<ConsultationSessionListDto>>.Success(pagedResult);
    }
}
