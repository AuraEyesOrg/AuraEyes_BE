using Application.Common.Interfaces;
using Application.Common.Models;
using Application.Feedback.Common;
using Domain.Repositories;

namespace Application.Feedback.Queries.ListOrganisationFeedback;

public class ListOrganisationFeedbackQueryHandler
    : IQueryHandler<ListOrganisationFeedbackQuery, PagedResult<OrganisationFeedbackDto>>
{
    private readonly IOrganisationFeedbackRepository _organisationFeedbackRepository;

    public ListOrganisationFeedbackQueryHandler(IOrganisationFeedbackRepository organisationFeedbackRepository)
    {
        _organisationFeedbackRepository = organisationFeedbackRepository;
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

        var dtoList = items.Select(x => new OrganisationFeedbackDto
        {
            Id = x.Id,
            PatientId = x.PatientId,
            OrganisationId = x.OrganisationId,
            AppointmentId = x.AppointmentId,
            Rating = x.Rating,
            Comment = x.Comment,
            CreatedAt = x.CreatedAt
        }).ToList();

        var pagedResult = new PagedResult<OrganisationFeedbackDto>(
            dtoList,
            totalCount,
            request.PageNumber,
            request.PageSize);

        return Result<PagedResult<OrganisationFeedbackDto>>.Success(pagedResult);
    }
}
