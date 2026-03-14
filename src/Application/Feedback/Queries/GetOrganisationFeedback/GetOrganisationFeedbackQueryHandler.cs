using Application.Common.Interfaces;
using Application.Common.Models;
using Application.Feedback.Common;
using Domain.Repositories;

namespace Application.Feedback.Queries.GetOrganisationFeedback;

public class GetOrganisationFeedbackQueryHandler : IQueryHandler<GetOrganisationFeedbackQuery, OrganisationFeedbackDto>
{
    private readonly IOrganisationFeedbackRepository _organisationFeedbackRepository;

    public GetOrganisationFeedbackQueryHandler(IOrganisationFeedbackRepository organisationFeedbackRepository)
    {
        _organisationFeedbackRepository = organisationFeedbackRepository;
    }

    public async Task<Result<OrganisationFeedbackDto>> Handle(GetOrganisationFeedbackQuery request, CancellationToken cancellationToken)
    {
        var feedback = await _organisationFeedbackRepository.GetByIdForOrganisationAsync(
            request.OrganisationId,
            request.FeedbackId,
            cancellationToken);

        if (feedback is null)
            return Result<OrganisationFeedbackDto>.NotFound($"Organisation feedback '{request.FeedbackId}' not found.");

        var dto = new OrganisationFeedbackDto
        {
            Id = feedback.Id,
            PatientId = feedback.PatientId,
            OrganisationId = feedback.OrganisationId,
            AppointmentId = feedback.AppointmentId,
            Rating = feedback.Rating,
            Comment = feedback.Comment,
            CreatedAt = feedback.CreatedAt
        };

        return Result<OrganisationFeedbackDto>.Success(dto);
    }
}
