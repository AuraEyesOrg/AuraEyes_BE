using Application.Common.Interfaces;
using Application.Common.Models;
using Application.Feedback.Common;
using Domain.Common;
using Domain.Entities.Users;
using Domain.Repositories;

namespace Application.Feedback.Queries.ListOrganisationFeedback;

public class ListOrganisationFeedbackQueryHandler
    : IQueryHandler<ListOrganisationFeedbackQuery, PagedResult<OrganisationFeedbackDto>>
{
    private readonly IOrganisationFeedbackRepository _organisationFeedbackRepository;
    private readonly IRepository<Patient> _patientRepository;
    private readonly IIdentityService _identityService;

    public ListOrganisationFeedbackQueryHandler(
        IOrganisationFeedbackRepository organisationFeedbackRepository,
        IRepository<Patient> patientRepository,
        IIdentityService identityService)
    {
        _organisationFeedbackRepository = organisationFeedbackRepository;
        _patientRepository = patientRepository;
        _identityService = identityService;
    }

    public async Task<Result<PagedResult<OrganisationFeedbackDto>>> Handle(
        ListOrganisationFeedbackQuery request,
        CancellationToken cancellationToken)
    {
        var (items, totalCount) = await _organisationFeedbackRepository.GetPagedByOrganisationAsync(
            request.OrganisationId,
            request.PageNumber,
            request.PageSize,
            cancellationToken);

        var dtoList = new List<OrganisationFeedbackDto>();

        foreach (var x in items)
        {
            var patientEntity = await _patientRepository.GetByIdAsync(x.PatientId, cancellationToken);
            var patientUser = patientEntity != null
                ? await _identityService.GetUserByIdAsync(patientEntity.UserId, cancellationToken)
                : null;

            dtoList.Add(new OrganisationFeedbackDto
            {
                Id = x.Id,
                PatientId = x.PatientId,
                PatientFullName = patientUser?.FullName,
                OrganisationId = x.OrganisationId,
                AppointmentId = x.AppointmentId,
                Rating = x.Rating,
                Comment = x.Comment,
                CreatedAt = x.CreatedAt
            });
        }

        var pagedResult = new PagedResult<OrganisationFeedbackDto>(
            dtoList,
            totalCount,
            request.PageNumber,
            request.PageSize);

        return Result<PagedResult<OrganisationFeedbackDto>>.Success(pagedResult);
    }
}
