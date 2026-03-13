using Application.Common.Interfaces;
using Application.Common.Models;
using Application.Feedback.Common;
using Domain.Repositories;

namespace Application.Feedback.Queries.GetOphthalmologistFeedback;

public class GetOphthalmologistFeedbackQueryHandler : IQueryHandler<GetOphthalmologistFeedbackQuery, OphthalmologistFeedbackDto>
{
    private readonly IOphthalmologistFeedbackRepository _ophthalmologistFeedbackRepository;

    public GetOphthalmologistFeedbackQueryHandler(IOphthalmologistFeedbackRepository ophthalmologistFeedbackRepository)
    {
        _ophthalmologistFeedbackRepository = ophthalmologistFeedbackRepository;
    }

    public async Task<Result<OphthalmologistFeedbackDto>> Handle(GetOphthalmologistFeedbackQuery request, CancellationToken cancellationToken)
    {
        var feedback = await _ophthalmologistFeedbackRepository.GetByIdForOphthalmologistAsync(
            request.OphthalmologistId,
            request.FeedbackId,
            cancellationToken);

        if (feedback is null)
            return Result<OphthalmologistFeedbackDto>.NotFound($"Ophthalmologist feedback '{request.FeedbackId}' not found.");

        var dto = new OphthalmologistFeedbackDto
        {
            Id = feedback.Id,
            PatientId = feedback.PatientId,
            OphthalmologistId = feedback.OphthalmologistId,
            ConsultationSessionId = feedback.ConsultationSessionId,
            Rating = feedback.Rating,
            Comment = feedback.Comment,
            CreatedAt = feedback.CreatedAt
        };

        return Result<OphthalmologistFeedbackDto>.Success(dto);
    }
}
