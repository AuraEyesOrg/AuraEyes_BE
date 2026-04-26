using Application.Common.Interfaces;
using Application.Common.Models;
using Domain.Common;
using Domain.Entities.Consultation;
using Domain.Repositories;

namespace Application.Feedback.Commands.CreateClinicFeedback;

public class CreateClinicFeedbackCommandHandler : ICommandHandler<CreateClinicFeedbackCommand, Guid>
{
    private readonly ICurrentUserService _currentUser;
    private readonly IAppointmentRepository _appointmentRepository;
    private readonly IClinicFeedbackRepository _clinicFeedbackRepository;
    private readonly IUnitOfWork _unitOfWork;

    public CreateClinicFeedbackCommandHandler(
        ICurrentUserService currentUser,
        IAppointmentRepository appointmentRepository,
        IClinicFeedbackRepository clinicFeedbackRepository,
        IUnitOfWork unitOfWork)
    {
        _currentUser = currentUser;
        _appointmentRepository = appointmentRepository;
        _clinicFeedbackRepository = clinicFeedbackRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<Guid>> Handle(CreateClinicFeedbackCommand request, CancellationToken cancellationToken)
    {
        if (!_currentUser.IsAuthenticated || !_currentUser.ProfileId.HasValue)
            return Result<Guid>.Unauthorized("Patient must be logged in.");

        var patientId = _currentUser.ProfileId.Value;

        var appointment = await _appointmentRepository.GetByIdAsync(request.AppointmentId, cancellationToken);
        if (appointment is null)
            return Result<Guid>.NotFound($"Appointment '{request.AppointmentId}' not found.");

        if (appointment.PatientId != patientId)
            return Result<Guid>.Forbidden("You can only submit feedback for your own appointment.");

        var isDuplicate = await _clinicFeedbackRepository.ExistsByPatientAndAppointmentAsync(
            patientId,
            request.AppointmentId,
            cancellationToken);

        if (isDuplicate)
            return Result<Guid>.Conflict("Feedback already exists for this appointment.");

        var feedback = new ClinicFeedback(
            patientId,
            request.AppointmentId,
            request.Rating,
            request.Comment,
            request.DoctorId,
            request.StaffId,
            request.OrganisationId);

        await _clinicFeedbackRepository.AddAsync(feedback, cancellationToken);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result<Guid>.Success(feedback.Id);
    }
}
