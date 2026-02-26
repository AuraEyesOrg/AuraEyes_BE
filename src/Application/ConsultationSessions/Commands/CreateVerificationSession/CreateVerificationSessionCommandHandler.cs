using Application.Common.Interfaces;
using Application.Common.Models;
using Domain.Common;
using Domain.Entities;
using Domain.Repositories;

namespace Application.ConsultationSessions.Commands.CreateVerificationSession;

public class CreateVerificationSessionCommandHandler
    : ICommandHandler<CreateVerificationSessionCommand, Guid>
{
    private readonly IConsultationSessionRepository _sessionRepository;
    private readonly IUnitOfWork _unitOfWork;

    public CreateVerificationSessionCommandHandler(
        IConsultationSessionRepository sessionRepository,
        IUnitOfWork unitOfWork)
    {
        _sessionRepository = sessionRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<Guid>> Handle(
        CreateVerificationSessionCommand request,
        CancellationToken cancellationToken)
    {
        var session = ConsultationSession.CreateVerification(
            request.PatientId,
            request.AiScreeningId,
            request.Price,
            request.OphthalmologistId);

        await _sessionRepository.AddAsync(session, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result<Guid>.Success(session.Id);
    }
}
