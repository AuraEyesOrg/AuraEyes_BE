using Application.Common.Interfaces;
using Application.Common.Models;
using Application.Feedback.Common;
using Domain.Common;
using Domain.Entities.Consultation;

namespace Application.Feedback.Queries.GetWebsiteFeedback;

public class GetWebsiteFeedbackQueryHandler : IQueryHandler<GetWebsiteFeedbackQuery, WebsiteFeedbackDto>
{
    private readonly IRepository<WebsiteFeedback> _websiteFeedbackRepository;

    public GetWebsiteFeedbackQueryHandler(IRepository<WebsiteFeedback> websiteFeedbackRepository)
    {
        _websiteFeedbackRepository = websiteFeedbackRepository;
    }

    public async Task<Result<WebsiteFeedbackDto>> Handle(GetWebsiteFeedbackQuery request, CancellationToken cancellationToken)
    {
        var feedback = await _websiteFeedbackRepository.GetByIdAsync(request.FeedbackId, cancellationToken);
        if (feedback is null)
            return Result<WebsiteFeedbackDto>.NotFound($"Website feedback '{request.FeedbackId}' not found.");

        var dto = new WebsiteFeedbackDto
        {
            Id = feedback.Id,
            PatientId = feedback.PatientId,
            Rating = feedback.Rating,
            Category = feedback.Category,
            Comment = feedback.Comment,
            CreatedAt = feedback.CreatedAt
        };

        return Result<WebsiteFeedbackDto>.Success(dto);
    }
}
