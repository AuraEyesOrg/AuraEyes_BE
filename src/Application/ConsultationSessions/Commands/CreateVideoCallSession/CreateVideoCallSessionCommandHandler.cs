using Application.Common.Interfaces;
using Application.Common.Models;
using Domain.Common;
using Domain.Entities;
using Domain.Repositories;

namespace Application.ConsultationSessions.Commands.CreateVideoCallSession;

public class CreateVideoCallSessionCommandHandler
    : ICommandHandler<CreateVideoCallSessionCommand, Guid>
{
    private readonly IConsultationSessionRepository _sessionRepository;
    private readonly IUnitOfWork _unitOfWork;

    public CreateVideoCallSessionCommandHandler(
        IConsultationSessionRepository sessionRepository,
        IUnitOfWork unitOfWork)
    {
        _sessionRepository = sessionRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<Guid>> Handle(
        CreateVideoCallSessionCommand request,
        CancellationToken cancellationToken)
    {
        var session = ConsultationSession.CreateVideoCall(
            request.PatientId,
            request.Price,
            request.AppointmentTime,
            request.OphthalmologistId,
            request.MeetingLink);

        await _sessionRepository.AddAsync(session, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result<Guid>.Success(session.Id);
    }
}
