using Application.Common.Interfaces;
using Application.Common.Models;
using Domain.Common;
using Domain.Entities.Consultation;
using Domain.Entities.Users;
using Domain.Enums;
using Domain.Repositories;

namespace Application.Feedback.Commands.CreateOphthalmologistFeedback;

public class CreateOphthalmologistFeedbackCommandHandler : ICommandHandler<CreateOphthalmologistFeedbackCommand, Guid>
{
    private readonly ICurrentUserService _currentUser;
    private readonly IConsultationSessionRepository _sessionRepository;
    private readonly IOphthalmologistFeedbackRepository _ophthalmologistFeedbackRepository;
    private readonly IRepository<Ophthalmologist> _ophthalmologistRepository;
    private readonly IUnitOfWork _unitOfWork;

    public CreateOphthalmologistFeedbackCommandHandler(
        ICurrentUserService currentUser,
        IConsultationSessionRepository sessionRepository,
        IOphthalmologistFeedbackRepository ophthalmologistFeedbackRepository,
        IRepository<Ophthalmologist> ophthalmologistRepository,
        IUnitOfWork unitOfWork)
    {
        _currentUser = currentUser;
        _sessionRepository = sessionRepository;
        _ophthalmologistFeedbackRepository = ophthalmologistFeedbackRepository;
        _ophthalmologistRepository = ophthalmologistRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<Guid>> Handle(CreateOphthalmologistFeedbackCommand request, CancellationToken cancellationToken)
    {
        if (!_currentUser.IsAuthenticated || !_currentUser.ProfileId.HasValue)
            return Result<Guid>.Unauthorized("Patient must be logged in.");

        var patientId = _currentUser.ProfileId.Value;

        var session = await _sessionRepository.GetByIdAsync(request.ConsultationSessionId, cancellationToken);
        if (session is null)
            return Result<Guid>.NotFound($"Consultation session '{request.ConsultationSessionId}' not found.");

        if (session.PatientId != patientId)
            return Result<Guid>.Forbidden("You can only submit feedback for your own consultation session.");

        if (session.Status != SessionStatus.Completed)
            return Result<Guid>.Failure("Consultation session must be completed before submitting feedback.");

        if (!session.OphthalmologistId.HasValue || session.OphthalmologistId.Value != request.OphthalmologistId)
            return Result<Guid>.Failure("Consultation session does not belong to the provided ophthalmologist.");

        var isDuplicate = await _ophthalmologistFeedbackRepository.ExistsByPatientAndSessionAsync(
            patientId,
            request.ConsultationSessionId,
            cancellationToken);

        if (isDuplicate)
            return Result<Guid>.Conflict("Feedback already exists for this consultation session.");

        var ophthalmologist = await _ophthalmologistRepository.GetByIdAsync(request.OphthalmologistId, cancellationToken);
        if (ophthalmologist is null)
            return Result<Guid>.NotFound($"Ophthalmologist '{request.OphthalmologistId}' not found.");

        var feedback = new OphthalmologistFeedback(
            patientId,
            request.OphthalmologistId,
            request.ConsultationSessionId,
            request.Rating,
            request.Comment);

        await _ophthalmologistFeedbackRepository.AddAsync(feedback, cancellationToken);

        ophthalmologist.ApplyNewRating(request.Rating);
        await _ophthalmologistRepository.UpdateAsync(ophthalmologist, cancellationToken);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result<Guid>.Success(feedback.Id);
    }
}
