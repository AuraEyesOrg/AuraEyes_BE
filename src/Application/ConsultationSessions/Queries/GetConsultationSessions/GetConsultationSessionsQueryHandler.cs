using Application.Common.Constants;
using Application.Common.Interfaces;
using Application.Common.Models;
using Application.ConsultationSessions.Common;
using Domain.Repositories;

namespace Application.ConsultationSessions.Queries.GetConsultationSessions;

public class GetConsultationSessionsQueryHandler
    : IQueryHandler<GetConsultationSessionsQuery, PagedResult<ConsultationSessionListDto>>
{
    private readonly IConsultationSessionRepository _sessionRepository;
    private readonly ICurrentUserService _currentUser;

    public GetConsultationSessionsQueryHandler(
        IConsultationSessionRepository sessionRepository,
        ICurrentUserService currentUser)
    {
        _sessionRepository = sessionRepository;
        _currentUser = currentUser;
    }

    public async Task<Result<PagedResult<ConsultationSessionListDto>>> Handle(
        GetConsultationSessionsQuery request,
        CancellationToken cancellationToken)
    {
        bool isAdmin = _currentUser.Roles.Any(r => Roles.Admins.Contains(r));
        Guid? participantScope = isAdmin ? null : _currentUser.UserId;

        var (items, totalCount) = await _sessionRepository.GetPagedAsync(
            request.PatientId,
            request.OphthalmologistId,
            request.Type,
            request.Status,
            request.ChatStatus,
            participantScope,
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
