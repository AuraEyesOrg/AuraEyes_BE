using Application.Common.Interfaces;
using Application.Common.Models;
using Application.Feedback.Common;

namespace Application.Feedback.Queries.ListOrganisationFeedback;

public record ListOrganisationFeedbackQuery : IQuery<PagedResult<OrganisationFeedbackDto>>
{
    public Guid OrganisationId { get; init; }
    public int PageNumber { get; init; } = 1;
    public int PageSize { get; init; } = 10;
}
