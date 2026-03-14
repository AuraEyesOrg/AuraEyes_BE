using Application.Common.Interfaces;
using Application.Common.Models;
using Domain.Common;
using Domain.Entities.Consultation;
using Domain.Entities.Users;
using Domain.Enums;
using Domain.Repositories;

namespace Application.Feedback.Commands.CreateOrganisationFeedback;

public class CreateOrganisationFeedbackCommandHandler : ICommandHandler<CreateOrganisationFeedbackCommand, Guid>
{
    private readonly ICurrentUserService _currentUser;
    private readonly IAppointmentRepository _appointmentRepository;
    private readonly IOrganisationFeedbackRepository _organisationFeedbackRepository;
    private readonly IRepository<Organisation> _organisationRepository;
    private readonly IUnitOfWork _unitOfWork;

    public CreateOrganisationFeedbackCommandHandler(
        ICurrentUserService currentUser,
        IAppointmentRepository appointmentRepository,
        IOrganisationFeedbackRepository organisationFeedbackRepository,
        IRepository<Organisation> organisationRepository,
        IUnitOfWork unitOfWork)
    {
        _currentUser = currentUser;
        _appointmentRepository = appointmentRepository;
        _organisationFeedbackRepository = organisationFeedbackRepository;
        _organisationRepository = organisationRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<Guid>> Handle(CreateOrganisationFeedbackCommand request, CancellationToken cancellationToken)
    {
        if (!_currentUser.IsAuthenticated || !_currentUser.ProfileId.HasValue)
            return Result<Guid>.Unauthorized("Patient must be logged in.");

        var patientId = _currentUser.ProfileId.Value;

        var appointment = await _appointmentRepository.GetByIdAsync(request.AppointmentId, cancellationToken);
        if (appointment is null)
            return Result<Guid>.NotFound($"Appointment '{request.AppointmentId}' not found.");

        if (appointment.PatientId != patientId)
            return Result<Guid>.Forbidden("You can only submit feedback for your own appointment.");

        if (appointment.Type != AppointmentType.ClinicVisit)
            return Result<Guid>.Failure("Feedback is only allowed for CLINIC_VISIT appointments.");

        if (appointment.Status != AppointmentStatus.Completed)
            return Result<Guid>.Failure("Appointment must be completed before submitting feedback.");

        if (!appointment.OrganisationId.HasValue || appointment.OrganisationId.Value != request.OrganisationId)
            return Result<Guid>.Failure("Appointment does not belong to the provided organisation.");

        var isDuplicate = await _organisationFeedbackRepository.ExistsByPatientAndAppointmentAsync(
            patientId,
            request.AppointmentId,
            cancellationToken);

        if (isDuplicate)
            return Result<Guid>.Conflict("Feedback already exists for this appointment.");

        var organisation = await _organisationRepository.GetByIdAsync(request.OrganisationId, cancellationToken);
        if (organisation is null)
            return Result<Guid>.NotFound($"Organisation '{request.OrganisationId}' not found.");

        var feedback = new OrganisationFeedback(
            patientId,
            request.OrganisationId,
            request.AppointmentId,
            request.Rating,
            request.Comment);

        await _organisationFeedbackRepository.AddAsync(feedback, cancellationToken);

        organisation.ApplyNewRating(request.Rating);
        await _organisationRepository.UpdateAsync(organisation, cancellationToken);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result<Guid>.Success(feedback.Id);
    }
}
