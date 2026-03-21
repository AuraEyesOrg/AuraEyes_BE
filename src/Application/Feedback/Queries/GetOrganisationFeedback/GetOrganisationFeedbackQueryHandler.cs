using Application.Common.Interfaces;
using Application.Common.Models;
using Application.Feedback.Common;
using Domain.Common;
using Domain.Entities.Users;
using Domain.Repositories;

namespace Application.Feedback.Queries.GetOrganisationFeedback;

public class GetOrganisationFeedbackQueryHandler : IQueryHandler<GetOrganisationFeedbackQuery, OrganisationFeedbackDto>
{
    private readonly IOrganisationFeedbackRepository _organisationFeedbackRepository;
    private readonly IRepository<Patient> _patientRepository;
    private readonly IIdentityService _identityService;

    public GetOrganisationFeedbackQueryHandler(
        IOrganisationFeedbackRepository organisationFeedbackRepository,
        IRepository<Patient> patientRepository,
        IIdentityService identityService)
    {
        _organisationFeedbackRepository = organisationFeedbackRepository;
        _patientRepository = patientRepository;
        _identityService = identityService;
    }

    public async Task<Result<OrganisationFeedbackDto>> Handle(GetOrganisationFeedbackQuery request, CancellationToken cancellationToken)
    {
        var feedback = await _organisationFeedbackRepository.GetByIdForOrganisationAsync(
            request.OrganisationId,
            request.FeedbackId,
            cancellationToken);

        if (feedback is null)
            return Result<OrganisationFeedbackDto>.NotFound($"Organisation feedback '{request.FeedbackId}' not found.");

        var patientEntity = await _patientRepository.GetByIdAsync(feedback.PatientId, cancellationToken);
        var patientUser = patientEntity != null
            ? await _identityService.GetUserByIdAsync(patientEntity.UserId, cancellationToken)
            : null;

        var dto = new OrganisationFeedbackDto
        {
            Id = feedback.Id,
            PatientId = feedback.PatientId,
            PatientFullName = patientUser?.FullName,
            OrganisationId = feedback.OrganisationId,
            AppointmentId = feedback.AppointmentId,
            Rating = feedback.Rating,
            Comment = feedback.Comment,
            CreatedAt = feedback.CreatedAt
        };

        return Result<OrganisationFeedbackDto>.Success(dto);
    }
}
