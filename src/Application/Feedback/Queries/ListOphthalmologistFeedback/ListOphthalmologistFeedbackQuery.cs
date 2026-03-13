using Application.Common.Interfaces;
using Application.Common.Models;
using Application.Feedback.Common;

namespace Application.Feedback.Queries.ListOphthalmologistFeedback;

public record ListOphthalmologistFeedbackQuery : IQuery<PagedResult<OphthalmologistFeedbackDto>>
{
    public Guid OphthalmologistId { get; init; }
    public int PageNumber { get; init; } = 1;
    public int PageSize { get; init; } = 10;
}
