using Application.Common.Constants;
using Application.Common.Interfaces;
using Application.Common.Models;
using Application.ConsultationSessions.Common;
using Domain.Common;
using Domain.Entities.Users;
using Domain.Repositories;

namespace Application.ConsultationSessions.Queries.GetConsultationSessions;

public class GetConsultationSessionsQueryHandler
    : IQueryHandler<GetConsultationSessionsQuery, PagedResult<ConsultationSessionListDto>>
{
    private readonly IConsultationSessionRepository _sessionRepository;
    private readonly ICurrentUserService _currentUser;
    private readonly IRepository<Patient> _patientRepository;
    private readonly IOphthalmologistRepository _ophthalmologistRepository;
    private readonly IIdentityService _identityService;

    public GetConsultationSessionsQueryHandler(
        IConsultationSessionRepository sessionRepository,
        ICurrentUserService currentUser,
        IRepository<Patient> patientRepository,
        IOphthalmologistRepository ophthalmologistRepository,
        IIdentityService identityService)
    {
        _sessionRepository = sessionRepository;
        _currentUser = currentUser;
        _patientRepository = patientRepository;
        _ophthalmologistRepository = ophthalmologistRepository;
        _identityService = identityService;
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

        var dtoList = await ConsultationSessionMapping.ToListDtosAsync(
            items,
            _patientRepository,
            _ophthalmologistRepository,
            _identityService,
            cancellationToken);

        var pagedResult = new PagedResult<ConsultationSessionListDto>(
            dtoList.ToList(), totalCount, request.PageNumber, request.PageSize);

        return Result<PagedResult<ConsultationSessionListDto>>.Success(pagedResult);
    }
}
