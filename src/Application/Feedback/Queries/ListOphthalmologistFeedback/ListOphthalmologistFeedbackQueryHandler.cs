using Application.Common.Interfaces;
using Application.Common.Models;
using Application.Feedback.Common;
using Domain.Repositories;

namespace Application.Feedback.Queries.ListOphthalmologistFeedback;

public class ListOphthalmologistFeedbackQueryHandler
    : IQueryHandler<ListOphthalmologistFeedbackQuery, PagedResult<OphthalmologistFeedbackDto>>
{
    private readonly IOphthalmologistFeedbackRepository _ophthalmologistFeedbackRepository;

    public ListOphthalmologistFeedbackQueryHandler(IOphthalmologistFeedbackRepository ophthalmologistFeedbackRepository)
    {
        _ophthalmologistFeedbackRepository = ophthalmologistFeedbackRepository;
    }

    public async Task<Result<PagedResult<OphthalmologistFeedbackDto>>> Handle(
        ListOphthalmologistFeedbackQuery request,
        CancellationToken cancellationToken)
    {
        var (items, totalCount) = await _ophthalmologistFeedbackRepository.GetPagedByOphthalmologistAsync(
            request.OphthalmologistId,
            request.PageNumber,
            request.PageSize,
            cancellationToken);

        var dtoList = items.Select(x => new OphthalmologistFeedbackDto
        {
            Id = x.Id,
            PatientId = x.PatientId,
            OphthalmologistId = x.OphthalmologistId,
            ConsultationSessionId = x.ConsultationSessionId,
            Rating = x.Rating,
            Comment = x.Comment,
            CreatedAt = x.CreatedAt
        }).ToList();

        var pagedResult = new PagedResult<OphthalmologistFeedbackDto>(
            dtoList,
            totalCount,
            request.PageNumber,
            request.PageSize);

        return Result<PagedResult<OphthalmologistFeedbackDto>>.Success(pagedResult);
    }
}
