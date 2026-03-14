using Application.Common.Constants;
using Application.Common.Interfaces;
using Application.Common.Models;
using Application.ConsultationSessions.Common;
using AutoMapper;
using Domain.Repositories;

namespace Application.ConsultationSessions.Queries.GetConsultationSessions;

public class GetConsultationSessionsQueryHandler
    : IQueryHandler<GetConsultationSessionsQuery, PagedResult<ConsultationSessionListDto>>
{
    private readonly IConsultationSessionRepository _sessionRepository;
    private readonly ICurrentUserService _currentUser;
    private readonly IMapper _mapper;
    private readonly IConsultationParticipantEnrichmentService _participantEnrichmentService;

    public GetConsultationSessionsQueryHandler(
        IConsultationSessionRepository sessionRepository,
        ICurrentUserService currentUser,
        IMapper mapper,
        IConsultationParticipantEnrichmentService participantEnrichmentService)
    {
        _sessionRepository = sessionRepository;
        _currentUser = currentUser;
        _mapper = mapper;
        _participantEnrichmentService = participantEnrichmentService;
    }

    public async Task<Result<PagedResult<ConsultationSessionListDto>>> Handle(
        GetConsultationSessionsQuery request,
        CancellationToken cancellationToken)
    {
        var isAdmin = _currentUser.Roles.Any(r => Roles.Admins.Contains(r));
        Guid? participantProfileId = isAdmin ? null : _currentUser.ProfileId;

        var (items, totalCount) = await _sessionRepository.GetPagedAsync(
            request.PatientId,
            request.OphthalmologistId,
            request.Type,
            request.Status,
            request.ChatStatus,
            participantProfileId,
            request.PageNumber,
            request.PageSize,
            cancellationToken);

        var baseDtos = _mapper.Map<List<ConsultationSessionListDto>>(items);
        var dtoList = await _participantEnrichmentService.EnrichListAsync(
            baseDtos,
            items,
            cancellationToken);

        var pagedResult = new PagedResult<ConsultationSessionListDto>(
            dtoList.ToList(), totalCount, request.PageNumber, request.PageSize);

        return Result<PagedResult<ConsultationSessionListDto>>.Success(pagedResult);
    }
}
