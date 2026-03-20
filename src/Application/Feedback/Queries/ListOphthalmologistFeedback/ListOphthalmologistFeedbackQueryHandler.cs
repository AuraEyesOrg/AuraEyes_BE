using Application.Common.Interfaces;
using Application.Common.Models;
using Application.Feedback.Common;
using Domain.Repositories;

namespace Application.Feedback.Queries.ListOphthalmologistFeedback;

public class ListOphthalmologistFeedbackQueryHandler
    : IQueryHandler<ListOphthalmologistFeedbackQuery, PagedResult<OphthalmologistFeedbackDto>>
{
    private readonly IOphthalmologistFeedbackRepository _ophthalmologistFeedbackRepository;
    private readonly IIdentityService _identityService;

    public ListOphthalmologistFeedbackQueryHandler(
        IOphthalmologistFeedbackRepository ophthalmologistFeedbackRepository,
        IIdentityService identityService)
    {
        _ophthalmologistFeedbackRepository = ophthalmologistFeedbackRepository;
        _identityService = identityService;
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

        var dtoList = new List<OphthalmologistFeedbackDto>();

        foreach (var x in items)
        {
            var patient = await _identityService.GetUserByIdAsync(x.PatientId, cancellationToken);

            dtoList.Add(new OphthalmologistFeedbackDto
            {
                Id = x.Id,
                PatientId = x.PatientId,
                PatientFullName = patient?.FullName,
                OphthalmologistId = x.OphthalmologistId,
                ConsultationSessionId = x.ConsultationSessionId,
                Rating = x.Rating,
                Comment = x.Comment,
                CreatedAt = x.CreatedAt,
            });
        }

        var pagedResult = new PagedResult<OphthalmologistFeedbackDto>(
            dtoList,
            totalCount,
            request.PageNumber,
            request.PageSize);

        return Result<PagedResult<OphthalmologistFeedbackDto>>.Success(pagedResult);
    }
}
