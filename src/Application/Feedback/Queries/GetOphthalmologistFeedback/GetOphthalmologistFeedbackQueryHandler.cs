using Application.Common.Interfaces;
using Application.Common.Models;
using Application.Feedback.Common;
using Domain.Common;
using Domain.Entities.Users;
using Domain.Repositories;

namespace Application.Feedback.Queries.GetOphthalmologistFeedback;

public class GetOphthalmologistFeedbackQueryHandler : IQueryHandler<GetOphthalmologistFeedbackQuery, OphthalmologistFeedbackDto>
{
    private readonly IOphthalmologistFeedbackRepository _ophthalmologistFeedbackRepository;
    private readonly IRepository<Patient> _patientRepository;
    private readonly IIdentityService _identityService;

    public GetOphthalmologistFeedbackQueryHandler(
        IOphthalmologistFeedbackRepository ophthalmologistFeedbackRepository,
        IRepository<Patient> patientRepository,
        IIdentityService identityService)
    {
        _ophthalmologistFeedbackRepository = ophthalmologistFeedbackRepository;
        _patientRepository = patientRepository;
        _identityService = identityService;
    }

    public async Task<Result<OphthalmologistFeedbackDto>> Handle(GetOphthalmologistFeedbackQuery request, CancellationToken cancellationToken)
    {
        var feedback = await _ophthalmologistFeedbackRepository.GetByIdForOphthalmologistAsync(
            request.OphthalmologistId,
            request.FeedbackId,
            cancellationToken);

        if (feedback is null)
            return Result<OphthalmologistFeedbackDto>.NotFound($"Ophthalmologist feedback '{request.FeedbackId}' not found.");

        var patientEntity = await _patientRepository.GetByIdAsync(feedback.PatientId, cancellationToken);
        var patientUser = patientEntity != null 
            ? await _identityService.GetUserByIdAsync(patientEntity.UserId, cancellationToken) 
            : null;

        var dto = new OphthalmologistFeedbackDto
        {
            Id = feedback.Id,
            PatientId = feedback.PatientId,
            PatientFullName = patientUser?.FullName,
            OphthalmologistId = feedback.OphthalmologistId,
            ConsultationSessionId = feedback.ConsultationSessionId,
            Rating = feedback.Rating,
            Comment = feedback.Comment,
            CreatedAt = feedback.CreatedAt
        };

        return Result<OphthalmologistFeedbackDto>.Success(dto);
    }
}
